using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Evade
{
    public static class Evade
    {
        public static bool UseWalk;
        public static bool UseSkills;

        private static DodgeAbilityInfo _castingLastFrame = null;

        private static List<Projectile> _dangerousProjectiles;
        private static List<ThrowObject> _dangerousCircularThrows;

        public static void Init()
        {
            CommonEvents.PostUpdate += OnUpdate;
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
        }

        public static void OnUpdate()
        {
            EvadeLogic();
        }

        private static void EvadeLogic()
        {
            if (UseSkills)
            {
                var casting = CastingEvadeSpell();
                if (casting != null && casting.UseInEvade && casting.ShouldUse())
                {
                    var projectiles = AbilityTracker.Enemy.Projectiles.Active
                        .Where(p => p.WillCollideWithPlayer(LocalPlayer.Instance, p.Radius / 2)).ToArray();
                    if (!projectiles.Any()) return;
                    var circularThrows = AbilityTracker.Enemy.CircularThrows.Active
                        .Where(t => t.TargetPosition.Distance(LocalPlayer.Instance) < t.SpellCollisionRadius).ToArray();
                    if (!circularThrows.Any()) return;

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
                        LocalPlayer.Aim(projectiles
                            .OrderByDescending(p => p.Data().Danger)
                            .First().MapObject.Position);
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

            var dangerousProjectiles = AbilityTracker.Enemy.Projectiles.Active.Where(p => p.WillCollideWithPlayer(LocalPlayer.Instance, p.Radius / 2))
                .ToArray();
            if (dangerousProjectiles.Any())
            {
                var mostDangerous = dangerousProjectiles.OrderByDescending(p => p.Data().Danger).First();
                var data = mostDangerous.Data();
                if (UseWalk && CanDodge(mostDangerous, data)) DodgeWithWalk(mostDangerous);
                else if (UseSkills)
                {
                    try
                    {
                        DodgeWithAbilities(mostDangerous, data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                return;
            }

            var dangerousCircularThrows = AbilityTracker.Enemy.CircularThrows.Active
                .Where(t => t.TargetPosition.Distance(LocalPlayer.Instance) < t.SpellCollisionRadius).ToArray();
            if (dangerousCircularThrows.Any())
            {
                var mostDangerous = dangerousCircularThrows.OrderByDescending(p => p.Data().Danger).First();
                var data = mostDangerous.Data();
                var closestPoint = LocalPlayer.Instance.GetClosestExitPointFromCircle(mostDangerous.TargetPosition, data.Radius);
                if (UseWalk && CanDodge(mostDangerous, data, closestPoint)) DodgeWithWalk(closestPoint);
                else if (UseSkills)
                {
                    try
                    {
                        DodgeWithAbilities(mostDangerous, data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                return;
            }
        }

        private static bool CanDodge(ThrowObject throwObj, AbilityInfo data, Vector2 closestExit)
        {
            return data.Duration - throwObj.GameObject.Get<AgeObject>().Age < LocalPlayer.Instance.Distance(closestExit) / 3.4f;
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

        private static bool CanDodge(Projectile projectile, AbilityInfo data)
        {
            var timeToImpact = (LocalPlayer.Instance.Distance(projectile.MapObject.Position) -
                                LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                               data.ProjectileSpeed;
            var closestPointOnLine =
                Geometry.ClosestPointOnLine(projectile.StartPosition, projectile.CalculatedEndPosition, LocalPlayer.Instance.Pos());
            var timeToDodge = (projectile.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius -
                               LocalPlayer.Instance.Distance(closestPointOnLine)) / 3.4f;

            return timeToImpact < timeToDodge;
        }

        private static void DodgeWithWalk(Projectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var closestPointOnLine =
                Geometry.ClosestPointOnLine(projectile.StartPosition, projectile.CalculatedEndPosition, LocalPlayer.Instance.Pos());
            var dir = closestPointOnLine.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static void DodgeWithWalk(CastingProjectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var closestPointOnLine = Geometry.ClosestPointOnLine(projectile.Caster.Pos(), projectile.EndPos, LocalPlayer.Instance.Pos());
            var dir = closestPointOnLine.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static void DodgeWithAbilities(ThrowObject throwObj, AbilityInfo data)
        {
            var timeToImpact = data.Duration - throwObj.GameObject.Get<AgeObject>().Age;

            if (PlayerIsSafe() || timeToImpact > 0.3f) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (ability.ShouldUse() && ability.AbilityType != DodgeAbilityType.Counter && 
                    ability.AbilityType != DodgeAbilityType.Shield && ability.GetDanger() <= data.GetDanger())
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

        private static void DodgeWithAbilities(Projectile projectile, AbilityInfo data)
        {
            if (data == null)
            {
                Console.WriteLine("[HoyerEvade] DEBUG: Unknown Projectile Detected: " + projectile.ObjectName);
                return;
            }
            var timeToImpact = (LocalPlayer.Instance.Distance(projectile.MapObject.Position) -
                                LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                               data.ProjectileSpeed;
            if (PlayerIsSafe(timeToImpact)) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (ability.ShouldUse() && ability.GetDanger() <= data.GetDanger() &&
                    !LocalPlayer.Instance.PhysicsCollision.IsImmaterial && !LocalPlayer.Instance.IsCountering)
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