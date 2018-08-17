using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Evade
{
    public static class Evade
    {
        public static bool UseWalk;
        public static bool UseSkills;

        private static DodgeAbilityInfo _castingLastFrame = null;

        public static void Init()
        {
            CommonEvents.PostUpdate += OnUpdate;
            AbilityTracker.Enemy.Projectiles.OnDangerous += Projectiles_OnDangerous;
        }

        private static void Projectiles_OnDangerous(TrackedProjectile obj)
        {
            Console.WriteLine("tracked obj " + obj.Projectile.ObjectName);
        }

        public static void OnUpdate()
        {
            EvadeLogic();
        }

        private static void EvadeLogic()
        {
            var trackedProjectiles = AbilityTracker.Enemy.Projectiles.TrackedProjectiles;
            var trackedCircularThrows = AbilityTracker.Enemy.CircularThrows.TrackedThrows;

            if (UseSkills)
            {
                var casting = CastingEvadeSpell();
                if (casting != null && casting.UseInEvade && casting.ShouldUse())
                {
                    _castingLastFrame = casting;
                    casting.SetStatus(true);

                    if (casting.OverrideValue() != 0) return;

                    if (casting.AbilityType == DodgeAbilityType.Jump && casting.UsesMousePos)
                    {
                        LocalPlayer.EditAimPosition = true;
                        LocalPlayer.Aim(casting.GetJumpPos());
                    }

                    if (casting.AbilityType == DodgeAbilityType.Shield && casting.UsesMousePos)
                    {
                        LocalPlayer.EditAimPosition = true;
                        LocalPlayer.Aim(trackedProjectiles
                            .OrderByDescending(p => p.Data.Danger)
                            .First().Projectile.LastPosition);
                    }
                    else if (casting.NeedsSelfCast)
                    {
                        LocalPlayer.EditAimPosition = true;
                        LocalPlayer.Aim(LocalPlayer.Instance.MapObject.Position);
                    }

                    return;
                }
                else if (!LocalPlayer.Instance.AbilitySystem.IsCasting && _castingLastFrame != null)
                {
                    _castingLastFrame.SetStatus(false);
                    _castingLastFrame = null;
                    LocalPlayer.EditAimPosition = false;
                }
            }

            if (trackedProjectiles.Any())
            {
                foreach (var trackedProjectile in trackedProjectiles)
                {
                    trackedProjectile.Update();
                }

                var dangerousProjectiles = trackedProjectiles.Where(p => p.IsDangerous).ToArray();
                if (dangerousProjectiles.Any())
                {
                    var mostDangerous = dangerousProjectiles.OrderByDescending(p => p.Data.Danger).First();
                    if (UseWalk && CanDodge(mostDangerous)) DodgeWithWalk(mostDangerous);
                    else if (UseSkills)
                    {
                        DodgeWithAbilities(mostDangerous);
                    }

                    return;
                }
            }

            if (trackedCircularThrows.Any())
            {
                foreach (var trackedThrow in trackedCircularThrows)
                {
                    trackedThrow.Update();   
                }

                var dangerousThrows = trackedCircularThrows.Where(t => t.IsDangerous).ToArray();
                if (dangerousThrows.Any())
                {
                    var mostDangerous = dangerousThrows.OrderByDescending(p => p.Data.Danger).First();
                    if (UseWalk)
                    {
                        if (CanDodge(mostDangerous)) DodgeWithWalk(mostDangerous.ThrowObject.TargetPosition.Extend(mostDangerous.ClosestOutsidePoint, 10).Normalized);
                        else if (UseSkills)
                        {
                            DodgeWithAbilities(mostDangerous);
                        }
                    }
                    else if (UseSkills)
                    {
                        DodgeWithAbilities(mostDangerous);
                    }
                }
                return;
            }
        }

        private static bool CanDodge(TrackedThrowObject throwObj)
        {
            return throwObj.EstimatedImpact - Time.time < LocalPlayer.Instance.Distance(throwObj.ClosestOutsidePoint) / 3.4f;
        }

        private static DodgeAbilityInfo CastingEvadeSpell()
        {
            if (LocalPlayer.Instance.AbilitySystem.IsCasting)
            {
                var dodge = AbilityDatabase.GetDodge(LocalPlayer.Instance.AbilitySystem.CastingAbilityId);
                if (dodge != null && LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex != -1)
                {
                    //Console.WriteLine(LocalPlayer.Instance.AbilitySystem.CastingAbilityId);
                    //Console.WriteLine(LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex);
                }

                return dodge;
            }

            return null;
        }

        private static void DodgeWithWalk(Vector2 towards)
        {
            LocalPlayer.BlockAllInput = true;
            LocalPlayer.Move(towards.Normalized);
        }

        private static bool CanDodge(TrackedProjectile projectile)
        {
            var timeToDodge = ((projectile.Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius -
                                LocalPlayer.Instance.Distance(projectile.ClosestPoint)) / 3.4f);

            return projectile.EstimatedImpact - Time.time < timeToDodge;
        }

        private static void DodgeWithWalk(TrackedProjectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var dir = projectile.ClosestPoint.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
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

            if (timeToImpact > 0.3f || PlayerIsSafe()) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (ability.ShouldUse() && ability.AbilityType != DodgeAbilityType.Counter &&
                    ability.AbilityType != DodgeAbilityType.Shield && ability.GetDanger() <= throwObj.Data.GetDanger())
                {
                    if (ability.NeedsSelfCast)
                    {
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    }
                    else
                    {
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    }

                    return;
                }
            }
        }

        private static void DodgeWithAbilities(TrackedProjectile projectile)
        {
            var timeToImpact = projectile.EstimatedImpact - Time.time;

            if (timeToImpact > 0.3f || PlayerIsSafe(timeToImpact)) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (ability.ShouldUse() && ability.GetDanger() <= projectile.Data.GetDanger())
                {
                    if (ability.NeedsSelfCast)
                    {
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    }
                    else
                    {
                        LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    }

                    return;
                }
            }
        }

        private static bool PlayerIsSafe(float time = 0, bool isProjectile = true)
        {
            if (LocalPlayer.Instance.PhysicsCollision.IsImmaterial || LocalPlayer.Instance.CharacterModel.IsModelInvisible) return true;
            if (!isProjectile) return false;

            foreach (var buff in LocalPlayer.Instance.Buffs)
            {
                if (buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" ||
                    buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam")
                {
                    if (buff.TimeToExpire > time) return true;
                }
            }

            return false;
        }
    }
}