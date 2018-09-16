using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;

namespace Hoyer.Common.Prediction
{
    public static class TargetSelection
    {
        public static bool UseMaxCursorDist;
        public static float MaxCursorDist;

        public static bool CursorDistCheck(Vector2 position)
        {
            if (UseMaxCursorDist) return position.Distance(Main.MouseWorldPos) <= MaxCursorDist;
            return true;
        }

        public static Prediction.Output GetTargetPrediction(SkillBase castingSpell, AbilityInfo data)
        {
            var isProjectile = data.AbilityType == AbilityType.LineProjectile;
            var useOnIncaps = data.Danger >= 3;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e =>
                    e != null && e.IsValid && e.IsActiveObject && !e.Living.IsDead && e.Pos().Distance(Vector2.Zero) > 0.1f &&
                    e.Distance(LocalPlayer.Instance) < castingSpell.Range * Prediction.CancelRangeModifier)
                .ToList();

            var output = Prediction.Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                var tryGetTarget = TargetSelector.GetTarget(possibleTargets, TargetingMode.NearMouse, float.MaxValue);
                if (data.AbilityType == AbilityType.CircleThrowObject || data.AbilityType == AbilityType.CircleJump)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        var pred = tryGetTarget.GetPrediction(castingSpell);
                        if (pred.CanHit && (CursorDistCheck(pred.CastPosition) || CursorDistCheck(pred.Target.Pos())))
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
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
                {
                    var pred = tryGetTarget.GetPrediction(castingSpell);
                    if (pred.CanHit && CursorDistCheck(pred.CastPosition))
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

        public static Prediction.Output GetTargetPrediction(SkillBase castingSpell, AbilityInfo data, bool useOnIncaps)
        {
            var isProjectile = data.AbilityType == AbilityType.LineProjectile;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e =>
                    e != null && !e.Living.IsDead && e.Pos().Distance(Vector2.Zero) > 0.1f &&
                    e.Distance(LocalPlayer.Instance) < castingSpell.Range * Prediction.CancelRangeModifier)
                .ToList();

            var output = Prediction.Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                var tryGetTarget = TargetSelector.GetTarget(possibleTargets, TargetingMode.NearMouse, float.MaxValue);
                if (data.AbilityType == AbilityType.CircleThrowObject || data.AbilityType == AbilityType.CircleJump)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        var pred = tryGetTarget.GetPrediction(castingSpell);
                        if (pred.CanHit && (CursorDistCheck(pred.CastPosition) || CursorDistCheck(pred.Target.Pos())))
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
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
                {
                    var pred = tryGetTarget.GetPrediction(castingSpell);
                    if (pred.CanHit && CursorDistCheck(pred.CastPosition))
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