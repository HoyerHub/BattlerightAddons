using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;
using UnityEngine;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Evade
{
    public static class Evade
    {
        public static bool UseWalk;
        public static bool UseSkills;

        public static void Init()
        {
            CommonEvents.PostUpdate += OnUpdate;
            InGameObject.OnCreate += InGameObject_OnCreate;
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
        }

        private static void InGameObject_OnCreate(InGameObject inGameObject)
        {
        }

        public static void OnUpdate()
        {
            EvadeLogic();
        }

        private static void EvadeLogic()
        {
            var casting = CastingEvadeSpell();
            if (casting != null && casting.UseInEvade && casting.ShouldUse())
            {
                if (casting.AbilityType == DodgeAbilityType.Jump && casting.UsesMousePos)
                {
                    LocalPlayer.EditAimPosition = true;
                    LocalPlayer.Aim(casting.GetSafeJumpPos());
                }
                if (casting.AbilityType == DodgeAbilityType.Shield && casting.UsesMousePos)
                {
                    LocalPlayer.EditAimPosition = true;
                    var projectiles = AbilityTracker.Enemy.Projectiles.Active
                        .Where(p => p.WillCollideWithPlayer(LocalPlayer.Instance, p.Radius / 2)).ToArray();
                    if (projectiles.Any())
                    {
                        LocalPlayer.Aim(projectiles
                            .OrderByDescending(p => p.Data().Danger)
                            .First().MapObject.Position);
                    }
                    else
                    {
                        LocalPlayer.Aim(EntitiesManager.EnemyTeam.OrderBy(e => e.Distance(LocalPlayer.Instance)).First().MapObject.Position);
                    }
                }
                else if (casting.NeedsSelfCast)
                {
                    LocalPlayer.EditAimPosition = true;
                    LocalPlayer.Aim(LocalPlayer.Instance.MapObject.Position);
                }
                return;
            }
            else if (!LocalPlayer.Instance.AbilitySystem.IsCasting) LocalPlayer.EditAimPosition = false;

            var dangerousProjectiles = AbilityTracker.Enemy.Projectiles.Active.Where(p => p.WillCollideWithPlayer(LocalPlayer.Instance, p.Radius / 2))
                .ToArray();
            if (dangerousProjectiles.Any())
            {
                var mostDangerous = dangerousProjectiles.OrderByDescending(p => p.Data().Danger).First();
                if (UseWalk && CanDodge(mostDangerous)) DodgeWithWalk(mostDangerous);
                else if (UseSkills)
                    DodgeWithAbilities(mostDangerous);
                return;
            }

            var dangerousCasts = AbilityTracker.Enemy.Projectiles.Casting.Where(p => p.WillCollideWithPlayer).ToArray();
            if (dangerousCasts.Any())
            {
                var mostDangerous = dangerousCasts.OrderByDescending(p => p.Data.Danger).First();
                if (UseWalk) DodgeWithWalk(mostDangerous);
                return;
            }
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

        private static bool CanDodge(Projectile projectile)
        {
            var timeToImpact = (LocalPlayer.Instance.Distance(projectile.MapObject.Position) -
                                LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                               projectile.Data().ProjectileSpeed;
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

        private static void DodgeWithAbilities(Projectile projectile)
        {
            var timeToImpact = (LocalPlayer.Instance.Distance(projectile.MapObject.Position) -
                                LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                               projectile.Data().ProjectileSpeed;

            if (PlayerIsSafe(timeToImpact)) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (ability.ShouldUse() && ability.GetDanger() <= projectile.Data().Danger &&
                    LocalPlayer.GetAbilityHudData(ability.AbilitySlot).CooldownLeft <= 0.01 &&
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

        private static bool PlayerIsSafe(float time = 0)
        {
            if (LocalPlayer.Instance.PhysicsCollision.IsImmaterial) return true;
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