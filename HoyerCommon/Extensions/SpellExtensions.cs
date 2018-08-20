using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;

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

        public static AbilityInfo Data(this ThrowObject throwObject)
        {
            return AbilityDatabase.Get(throwObject.GameObject.ObjectName);
        }

        public static AbilityInfo Data(this DashObject dashObject)
        {
            return AbilityDatabase.Get(dashObject.GameObject.ObjectName);
        }

        public static AbilityInfo Data(this TravelBuffObject travelObject)
        {
            return AbilityDatabase.Get(travelObject.GameObject.ObjectName);
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

        public static Vector2 GetClosestExitPointFromCircle(this Character player, Vector2 circleCenter, float circleRadius)
        {
            return circleCenter.Extend(player.Pos(), circleRadius);
        }

        public static bool IsReady(this AbilitySlot slot)
        {
            var ability = LocalPlayer.GetAbilityHudData(slot);
            return ability != null && ability.CooldownLeft <= 0 && ability.EnergyCost <= LocalPlayer.Instance.Energized.Energy;
        }

        public static bool InRange(this AbilitySlot slot, float distance)
        {
            return Skills.Active.Get(slot).Range * Prediction.CastingRangeModifier > distance;
        }

        public static SkillType ToSkillType(this AbilityType type)
        {
            if (type == AbilityType.LineProjectile) return SkillType.Line;
            if (type == AbilityType.CircleThrowObject) return SkillType.Circle;
            return SkillType.Line;
        }
    }
}