using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.Core.Models;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Common.Extensions;
using Hoyer.Common.TestPrediction2NS;

namespace Hoyer.Common
{
    public static class Prediction
    {
        public static int Mode = 1;
        public static float CastingRangeModifier;
        public static float CancelRangeModifier;

        public static Output Get(Character target, SkillBase spell)
        {
            if (Mode == 0)
            {
                return Basic(target, spell);
            }
            if (Mode == 1)
            {
                return SDK(target, spell);
            }
            if (Mode == 2)
            {
                return TestPred(target, spell);
            }
            return Output.None;
        }

        public static Output Basic(Character target, SkillBase spell)
        {
            var output = new Output();
            if (spell.SkillType == SkillType.Line && LocalPlayer.Instance.CheckCollisionToTarget(target, spell.SpellCollisionRadius))
            {
                output.CanHit = false;
                output.Hitchance = HitChance.Collision;
                output.HitchancePercentage = 0;
                output.CastPosition = Vector2.Zero;
            }
            else
            {
                var distance = target.Distance(LocalPlayer.Instance);
                output.CanHit = true;
                output.Hitchance = HitChance.High;
                output.HitchancePercentage = 75;
                output.CastPosition = spell.FixedDelay > 0
                    ? new Vector2(target.Pos().X + target.NetworkMovement.Velocity.X * spell.FixedDelay,
                        target.Pos().Y + target.NetworkMovement.Velocity.Y * spell.FixedDelay)
                    : new Vector2(target.Pos().X + target.NetworkMovement.Velocity.X * (distance / spell.Speed),
                        target.Pos().Y + target.NetworkMovement.Velocity.Y * (distance / spell.Speed));
            }
            return output;
        }

        private static Output SDK(Character target, SkillBase spell)
        {
            if (spell.SkillType == SkillType.Line)
            {
                var output = new PredictionInput(LocalPlayer.Instance, target, spell.Range, spell.Speed, spell.SpellCollisionRadius, SkillType.Line).GetLinePrediction();
                return new Output
                {
                    CanHit = output.HitChancePercent > 1,
                    CastPosition = output.PredictedPosition,
                    CollisionResult = output.CollisionResult,
                    Hitchance = output.HitChance,
                    HitchancePercentage = output.HitChancePercent
                };
            }
            else
            {
                var output = new PredictionInput(LocalPlayer.Instance, target, spell.Range, spell.Speed, spell.SpellCollisionRadius, SkillType.Circle, spell.FixedDelay).GetCirclePrediction();
                return new Output
                {
                    CanHit = output.HitChancePercent > 1,
                    CastPosition = output.PredictedPosition,
                    CollisionResult = output.CollisionResult,
                    Hitchance = output.HitChance,
                    HitchancePercentage = output.HitChancePercent
                };
            }
        }

        private static Output TestPred(Character target, SkillBase spell)
        {
            if (spell.SkillType == SkillType.Line)
            {
                return TestPrediction.GetPrediction(LocalPlayer.Instance.Pos(), target, spell.Range, spell.Speed, spell.SpellCollisionRadius, spell.FixedDelay, 1.75f, true);
            }
            return TestPrediction.GetPrediction(LocalPlayer.Instance.Pos(), target, spell.Range, spell.Speed, spell.SpellCollisionRadius, spell.FixedDelay);

        }

        public class Output
        {
            public Vector2 CastPosition;
            public bool CanHit;
            public HitChance Hitchance = HitChance.Unknown;
            public float HitchancePercentage;
            public CollisionResult CollisionResult;

            public static Output None
            {
                get
                {
                    return new Output { CastPosition = Vector2.Zero, Hitchance = HitChance.Unknown, CanHit = false };
                }
            }
        }
    }
}