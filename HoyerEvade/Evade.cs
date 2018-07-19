using System;
using System.Linq;
using BattleRight.Core;
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
        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
        }

        public static void OnUpdate(EventArgs args)
        {
            EvadeLogic();
        }

        private static void EvadeLogic()
        {
            var dangerousProjectiles = AbilityTracker.Enemy.Projectiles.Active.Where(p => p.WillCollideWithPlayer(LocalPlayer.Instance, p.Radius/2)).ToArray();
            if (dangerousProjectiles.Any())
            {
                var mostDangerous = dangerousProjectiles.OrderByDescending(p => p.Data().Danger).First();
                DodgeAbility(mostDangerous);
                return;
            }

            var dangerousCasts = AbilityTracker.Enemy.Projectiles.Casting.Where(p => p.WillCollideWithPlayer).ToArray();
            if (dangerousCasts.Any())
            {
                var mostDangerous = dangerousCasts.OrderByDescending(p => p.Data.Danger).First();
                DodgeAbility(mostDangerous);
                return;
            }

            LocalPlayer.BlockAllInput = false;
        }

        private static void DodgeAbility(Projectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var closestPointOnLine = Geometry.ClosestPointOnLine(projectile.StartPosition, projectile.CalculatedEndPosition, LocalPlayer.Instance.Pos());
            var dir = closestPointOnLine.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static void DodgeAbility(CastingProjectile projectile)
        {
            LocalPlayer.BlockAllInput = true;
            var closestPointOnLine = Geometry.ClosestPointOnLine(projectile.Caster.Pos(), projectile.EndPos, LocalPlayer.Instance.Pos());
            var dir = closestPointOnLine.Extend(LocalPlayer.Instance.Pos(), 10).Normalized;
            LocalPlayer.Move(dir);
        }

        private static float _shouldDodgeAgainTime = 0;

        private static void DodgeWithAbilities(Projectile projectile)
        {
            var timeToImpact = (LocalPlayer.Instance.Distance(projectile.MapObject.Position) -
                                LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                               projectile.Data().ProjectileSpeed;
            if (_shouldDodgeAgainTime > Time.time) return;
            foreach (var ability in AbilityDatabase.GetDodge(LocalPlayer.Instance.CharName).OrderBy(a => a.Priority))
            {
                if (ability.MinDanger <= projectile.Data().Danger &&
                    timeToImpact > ability.CastTime + 0.1 &&
                    LocalPlayer.GetAbilityHudData(ability.AbilitySlot).CooldownLeft <= 0.01 &&
                    !LocalPlayer.Instance.PhysicsCollision.IsImmaterial && !LocalPlayer.Instance.IsCountering)
                {
                    LocalPlayer.PressAbility(ability.AbilitySlot, true);
                    _shouldDodgeAgainTime = Time.time + ability.WaitAfter;
                    return;
                }
            }
        }
    }
}