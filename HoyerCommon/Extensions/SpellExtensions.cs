using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Local;

namespace Hoyer.Common.Extensions
{
    public static class SpellExtensions
    {
        public static bool CheckCollisionToTarget(this Character localPlayer, Character target, float radius)
        {
            if (localPlayer != null && target != null)
            {
                var heading = target.Pos() - localPlayer.Pos();
                var direction = heading.Normalized;

                var colsolver = CollisionSolver.CheckThickLineCollision(localPlayer.Pos(),
                    target.Pos() + direction, radius);
                return colsolver.IsColliding;
            }

            return false;
        }

        public static AbilityInfo Data(this Projectile projectile)
        {
            return AbilityDatabase.Get(projectile.ObjectName);
        }

        public static SkillBase Get(this List<SkillBase> skills, AbilitySlot slot)
        {
            return skills.FirstOrDefault(skill => skill.Slot == slot);
        }

        public static bool WillCollideWithPlayer(this Projectile projectile, Character player)
        {
            return Geometry.CircleVsThickLine(new Vector2(player.Pos().X, player.Pos().Y), player.MapCollision.MapCollisionRadius,
                projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius, true);
        }

        public static bool WillCollideWithPlayer(this Projectile projectile, Character player, float extraWidth)
        {
            return Geometry.CircleVsThickLine(new Vector2(player.Pos().X, player.Pos().Y), player.MapCollision.MapCollisionRadius,
                projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius + extraWidth, true);
        }

        public static bool IsReady(this AbilitySlot slot)
        {
            var ability = LocalPlayer.GetAbilityHudData(slot);
            return ability.CooldownLeft <= 0 && ability.EnergyCost <= LocalPlayer.Instance.Energized.Energy;
        }

        public static bool InRange(this AbilitySlot slot, float distance)
        {
            return Skills.Active.Get(slot).Range * Prediction.CastingRangeModifier > distance;
        }
    }
}