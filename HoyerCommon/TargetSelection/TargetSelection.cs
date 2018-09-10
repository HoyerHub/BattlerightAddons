using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using static Hoyer.Common.Prediction.Prediction;

namespace Hoyer.Common.TargetSelection
{
    public static class TargetSelection
    {
        public static Output GetTargetPrediction(SkillBase castingSpell, AbilityInfo data)
        {
            var isProjectile = data.AbilityType == AbilityType.LineProjectile;
            var useOnIncaps = data.Danger >= 3;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e =>
                    e != null && !e.Living.IsDead && e.Pos().Distance(Vector2.Zero) > 0.1f &&
                    e.Distance(LocalPlayer.Instance) < castingSpell.Range * CancelRangeModifier)
                .ToList();

            var output = Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                var tryGetTarget = TargetSelector.GetTarget(possibleTargets, TargetingMode.NearMouse, float.MaxValue);
                if (data.AbilityType == AbilityType.CircleThrowObject || data.AbilityType == AbilityType.CircleJump)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        output = Basic(tryGetTarget, castingSpell);
                        output.CanHit = true;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
                {
                    var pred = tryGetTarget.GetPrediction(castingSpell);
                    if (pred.CanHit)
                    {
                        output = pred;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else
                {
                    possibleTargets.Remove(tryGetTarget);
                }
            }

            return output;
        }

        public static Output GetTargetPrediction(SkillBase castingSpell, AbilityInfo data, bool useOnIncaps)
        {
            var isProjectile = data.AbilityType == AbilityType.LineProjectile;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e =>
                    e != null && !e.Living.IsDead && e.Pos().Distance(Vector2.Zero) > 0.1f &&
                    e.Distance(LocalPlayer.Instance) < castingSpell.Range * CancelRangeModifier)
                .ToList();

            var output = Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                var tryGetTarget = TargetSelector.GetTarget(possibleTargets, TargetingMode.NearMouse, float.MaxValue);
                if (data.AbilityType == AbilityType.CircleThrowObject || data.AbilityType == AbilityType.CircleJump)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        output = Basic(tryGetTarget, castingSpell);
                        output.CanHit = true;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
                {
                    var pred = tryGetTarget.GetPrediction(castingSpell);
                    if (pred.CanHit)
                    {
                        output = pred;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else
                {
                    possibleTargets.Remove(tryGetTarget);
                }
            }

            return output;
        }
    }
}