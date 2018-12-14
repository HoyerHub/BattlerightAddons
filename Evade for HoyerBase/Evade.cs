using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK;
using BattleRight.SDK.Events;
using BattleRight.SDK.EventsArgs;
using Hoyer.Base;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Extensions;
using Hoyer.Base.MathUtils;
using Hoyer.Base.Trackers;
using Hoyer.Evade;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Evade
{
    public static class Evade
    {
        public static bool UseWalk;
        public static bool UseSkills;

        private static DodgeAbilityInfo _castingLastFrame;

        public static void Setup()
        {
            Game.OnUpdate += OnUpdate;
            SpellDetector.OnSpellCast += SpellDetector_OnSpellCast;
        }

        public static void Unload()
        {
            Game.OnUpdate -= OnUpdate;
            SpellDetector.OnSpellCast -= SpellDetector_OnSpellCast;
            _castingLastFrame = null;
        }

        private static void SpellDetector_OnSpellCast(SpellCastArgs args)
        {
            var casting = CastingEvadeSpell();
            if (casting != null && casting.UseInEvade && casting.ShouldUse()) _castingLastFrame = casting;
        }

        public static void OnUpdate(EventArgs args)
        {
            EvadeLogic();
        }

        private static void EvadeLogic()
        {
            if (UseSkills && SkillAimLogic()) return;

            if (ObjectTracker.Enemy.Projectiles.TrackedObjects.Any())
            {
                foreach (var trackedProjectile in ObjectTracker.Enemy.Projectiles.TrackedObjects) trackedProjectile.Update();

                var dangerousProjectiles = ObjectTracker.Enemy.Projectiles.TrackedObjects.Where(p => p.IsDangerous).ToArray();
                if (dangerousProjectiles.Any())
                {
                    var mostDangerous = dangerousProjectiles.OrderByDescending(p => p.Data.Danger).First();
                    if (UseWalk && CanDodge(mostDangerous)) DodgeWithWalk(mostDangerous);
                    else if (UseSkills)
                        DodgeWithAbilities(mostDangerous);

                    return;
                }
            }

            if (ObjectTracker.Enemy.Dashes.TrackedObjects.Any())
            {
                foreach (var trackedDash in ObjectTracker.Enemy.Dashes.TrackedObjects) trackedDash.Update();

                var dangerousDashes = ObjectTracker.Enemy.Dashes.TrackedObjects.Where(d => d.IsDangerous).ToArray();
                if (dangerousDashes.Any())
                {
                    var mostDangerous = dangerousDashes.OrderByDescending(p => p.Data.Danger).First();
                    if (UseWalk && CanDodge(mostDangerous)) DodgeWithWalk(mostDangerous);
                    else if (UseSkills)
                        DodgeWithAbilities(mostDangerous);

                    return;
                }
            }

            if (ObjectTracker.Enemy.CircularThrows.TrackedObjects.Any())
            {
                foreach (var trackedThrow in ObjectTracker.Enemy.CircularThrows.TrackedObjects) trackedThrow.Update();

                var dangerousThrows = ObjectTracker.Enemy.CircularThrows.TrackedObjects.Where(t => t.IsDangerous).ToArray();
                if (dangerousThrows.Any())
                {
                    var mostDangerous = dangerousThrows.OrderByDescending(p => p.Data.Danger).First();
                    if (UseWalk)
                    {
                        if (CanDodge(mostDangerous))
                            DodgeWithWalk(mostDangerous.ThrowObject.TargetPosition.Extend(mostDangerous.ClosestOutsidePoint, 10).Normalized);
                        else if (UseSkills)
                            DodgeWithAbilities(mostDangerous);
                    }
                    else if (UseSkills)
                    {
                        DodgeWithAbilities(mostDangerous);
                    }
                }
            }

            if (ObjectTracker.Enemy.CircularJumps.TrackedObjects.Any())
            {
                foreach (var trackedJump in ObjectTracker.Enemy.CircularJumps.TrackedObjects) trackedJump.Update();

                var dangerousJumps = ObjectTracker.Enemy.CircularJumps.TrackedObjects.Where(t => t.IsDangerous).ToArray();
                if (dangerousJumps.Any())
                {
                    var mostDangerous = dangerousJumps.OrderByDescending(p => p.Data.Danger).First();
                    if (UseWalk)
                    {
                        if (CanDodge(mostDangerous))
                            DodgeWithWalk(mostDangerous.TravelObject.TargetPosition.Extend(mostDangerous.ClosestOutsidePoint, 10).Normalized);
                        else if (UseSkills)
                            DodgeWithAbilities(mostDangerous);
                    }
                    else if (UseSkills)
                    {
                        DodgeWithAbilities(mostDangerous);
                    }
                }
            }

            if (ObjectTracker.Enemy.CurveProjectiles.TrackedObjects.Any())
            {
                foreach (var trackedCurveProjectile in ObjectTracker.Enemy.CurveProjectiles.TrackedObjects) trackedCurveProjectile.Update();

                var dangerousProjectiles = ObjectTracker.Enemy.CurveProjectiles.TrackedObjects.Where(p => p.IsDangerous).ToArray();
                if (dangerousProjectiles.Any())
                {
                    var mostDangerous = dangerousProjectiles.OrderByDescending(p => p.Data.Danger).First();
                    if (UseSkills)
                        DodgeWithAbilities(mostDangerous);

                    return;
                }
            }
        }

        private static bool SkillAimLogic()
        {
            var dangerousProjectiles = new List<TrackedProjectile>();
            dangerousProjectiles.AddRange(ObjectTracker.Enemy.Projectiles.TrackedObjects);
            dangerousProjectiles = dangerousProjectiles.Where(p => p.IsDangerous).ToList();
            if (_castingLastFrame != null && LocalPlayer.Instance.AbilitySystem.IsCasting &&
                LocalPlayer.Instance.AbilitySystem.CastingAbilityId == _castingLastFrame.AbilityId)
            {
                var casting = _castingLastFrame;
                casting.SetStatus(true);

                if (casting.OverrideValue() != 0) return false;

                if (casting.AbilityType == DodgeAbilityType.Jump && casting.UsesMousePos)
                {
                    LocalPlayer.EditAimPosition = true;
                    LocalPlayer.Aim(casting.GetJumpPos());
                }
                else if (casting.AbilityType == DodgeAbilityType.Obstacle && dangerousProjectiles.Any())
                {
                    LocalPlayer.EditAimPosition = true;
                    var projectile = dangerousProjectiles
                        .OrderByDescending(p => p.Data.Danger)
                        .First().Projectile;
                    var pos = LocalPlayer.Instance.Pos().Extend(projectile.MapObject.Position, 1.5f);
                    LocalPlayer.Aim(pos);
                }
                else if (casting.AbilityType == DodgeAbilityType.Shield && casting.UsesMousePos && dangerousProjectiles.Any())
                {
                    var projectile = dangerousProjectiles
                        .OrderByDescending(p => p.Data.Danger)
                        .First().Projectile;
                    var pos = GeometryLib.NearestPointOnFiniteLine(projectile.MapObject.Position,
                        LocalPlayer.Instance.Pos(), Main.MouseWorldPos);
                    if (LocalPlayer.Instance.Distance(pos) < 1)
                    {
                        pos = LocalPlayer.Instance.Pos().Extend(projectile.MapObject.Position, 1);
                    }
                    LocalPlayer.Aim(pos);
                }
                else if (casting.NeedsSelfCast)
                {
                    LocalPlayer.EditAimPosition = true;
                    LocalPlayer.Aim(LocalPlayer.Instance.Pos());
                }

                return true;
            }

            if (!LocalPlayer.Instance.AbilitySystem.IsCasting && _castingLastFrame != null)
            {
                _castingLastFrame.SetStatus(false);
                _castingLastFrame = null;
                LocalPlayer.EditAimPosition = false;
            }

            return false;
        }

        private static bool CanDodge(TrackedThrowObject throwObj)
        {
            return throwObj.EstimatedImpact - Time.time < LocalPlayer.Instance.Distance(throwObj.ClosestOutsidePoint) / 3.4f;
        }

        private static bool CanDodge(TrackedCircularJump jumpObj)
        {
            return jumpObj.EstimatedImpact - Time.time < LocalPlayer.Instance.Distance(jumpObj.ClosestOutsidePoint) / 3.4f;
        }

        private static DodgeAbilityInfo CastingEvadeSpell()
        {
            return LocalPlayer.Instance.AbilitySystem.IsCasting
                ? AbilityDatabase.GetDodge(LocalPlayer.Instance.AbilitySystem.CastingAbilityId)
                : null;
        }

        private static void DodgeWithWalk(Vector2 towards)
        {
            LocalPlayer.BlockAllInput = true;
            LocalPlayer.Move(towards.Normalized);
        }

        private static bool CanDodge(TrackedProjectile projectile)
        {
            var timeToDodge = (projectile.Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius -
                               LocalPlayer.Instance.Distance(projectile.ClosestPoint)) / 3.4f;

            return projectile.EstimatedImpact - Time.time < timeToDodge;
        }

        private static bool CanDodge(TrackedCurveProjectile projectile)
        {
            var timeToDodge = (projectile.Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius -
                               LocalPlayer.Instance.Distance(projectile.ClosestPoint)) / 3.4f;

            return projectile.EstimatedImpact - Time.time < timeToDodge;
        }

        private static bool CanDodge(TrackedDash dash)
        {
            var timeToDodge = (dash.Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius -
                               LocalPlayer.Instance.Distance(dash.ClosestPoint)) / 3.4f;

            return dash.EstimatedImpact - Time.time < timeToDodge;
        }

        private static void DodgeWithWalk(TrackedProjectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var dir = projectile.ClosestPoint.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static void DodgeWithWalk(TrackedDash dash)
        {
            LocalPlayer.BlockAllInput = true;
            var dir = dash.ClosestPoint.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static void DodgeWithWalk(CastingProjectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var closestPointOnLine = Geometry.ClosestPointOnLine(projectile.Caster.Pos(), projectile.EndPos, LocalPlayer.Instance.Pos());
            var dir = closestPointOnLine.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static void DodgeWithAbilities(TrackedThrowObject throwObj)
        {
            var timeToImpact = throwObj.EstimatedImpact - Time.time;

            if (PlayerIsSafe()) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (!ability.UseInEvade || timeToImpact > ability.CastTime + 0.25f || timeToImpact < ability.CastTime + 0.05f) continue;
                if (!ability.ShouldUse() && ability.IsReady() || ability.AbilityType == DodgeAbilityType.Counter || ability.AbilityType == DodgeAbilityType.Shield ||
                    ability.AbilityType == DodgeAbilityType.Obstacle || ability.GetDanger() > throwObj.Data.GetDanger()) continue;

                LocalPlayer.PressAbility(ability.AbilitySlot, true);
                return;
            }
        }

        private static void DodgeWithAbilities(TrackedCircularJump jumpObj)
        {
            var timeToImpact = jumpObj.EstimatedImpact - Time.time;

            if (PlayerIsSafe()) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (!ability.UseInEvade || timeToImpact > ability.CastTime + 0.25f || timeToImpact < ability.CastTime + 0.05f) continue;
                if (ability.ShouldUse() && ability.IsReady() && ability.AbilityType != DodgeAbilityType.Counter &&
                    ability.AbilityType != DodgeAbilityType.Shield && ability.AbilityType != DodgeAbilityType.Obstacle && ability.GetDanger() <= jumpObj.Data.GetDanger())
                {
                    LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    return;
                }
            }
        }

        private static void DodgeWithAbilities(TrackedCurveProjectile projectile)
        {
            var timeToImpact = projectile.EstimatedImpact - Time.time;
            if (PlayerIsSafe(timeToImpact)) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (!ability.UseInEvade || timeToImpact > ability.CastTime + 0.25f || timeToImpact < ability.CastTime + 0.05f) continue;
                if (ability.ShouldUse() && ability.IsReady() && ability.GetDanger() <= projectile.Data.GetDanger())
                {
                    if (ability.NeedsSelfCast)
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    else
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);

                    return;
                }
            }
        }

        private static void DodgeWithAbilities(TrackedProjectile projectile)
        {
            var timeToImpact = projectile.EstimatedImpact - Time.time;

            if (PlayerIsSafe(timeToImpact)) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (!ability.UseInEvade || timeToImpact > ability.CastTime + 0.25f || timeToImpact < ability.CastTime + 0.05f) continue;
                if (ability.ShouldUse() && ability.IsReady() && ability.GetDanger() <= projectile.Data.GetDanger())
                {
                    if (ability.NeedsSelfCast)
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    else
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);

                    return;
                }
            }
        }

        private static void DodgeWithAbilities(TrackedDash dash)
        {
            var timeToImpact = dash.EstimatedImpact - Time.time;

            if (PlayerIsSafe(timeToImpact)) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (!ability.UseInEvade || timeToImpact > ability.CastTime + 0.25f || timeToImpact < ability.CastTime + 0.05f) continue;
                if (ability.ShouldUse() && ability.IsReady() && ability.GetDanger() <= dash.Data.GetDanger() &&
                    (dash.Data.CanCounter || ability.AbilityType != DodgeAbilityType.Counter) &&
                    (dash.Data.CanCounter || ability.AbilityType != DodgeAbilityType.Shield) &&
                    (ability.AbilityType != DodgeAbilityType.KnockAway || dash.DashObject.GameObject.ObjectName == "Rush"))
                {
                    if (ability.NeedsSelfCast)
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    else
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);

                    return;
                }
            }
        }

        private static bool PlayerIsSafe(float time = 0, bool isProjectile = true)
        {
            if (LocalPlayer.Instance.PhysicsCollision.IsImmaterial || LocalPlayer.Instance.CharacterModel.IsModelInvisible) return true;
            if (!isProjectile) return false;

            foreach (var buff in LocalPlayer.Instance.Buffs)
                if (buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" ||
                    buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam")
                    if (buff.TimeToExpire > time)
                        return true;

            return false;
        }
    }
}