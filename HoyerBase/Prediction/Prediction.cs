using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.Core.Models;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Base.Extensions;
using Hoyer.Base.Prediction.TestPrediction2NS;
using Hoyer.Base.Utilities.Geometry;

namespace Hoyer.Base.Prediction
{
    public static class Prediction
    {
        public static int Mode = 1;
        public static int WallCheckMode = 2;
        public static float CastingRangeModifier;
        public static float CancelRangeModifier;
        public static bool UseClosestPointOnLine;

        private static CollisionFlags IgnoreFlags
        {
            get
            {
                if (WallCheckMode == 0) return CollisionFlags.Bush | CollisionFlags.NPCBlocker | CollisionFlags.LowBlock | CollisionFlags.HighBlock;

                if (WallCheckMode == 1) return CollisionFlags.Bush | CollisionFlags.NPCBlocker | CollisionFlags.LowBlock;

                return CollisionFlags.NPCBlocker | CollisionFlags.Bush;
            }
        }

        public static Output Get(Character target, SkillBase spell)
        {
            if (Mode == 0) return Basic(target, spell);
            if (Mode == 1) return SDK(target, spell);
            if (Mode == 2) return TestPred(target, spell);
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

            output.Target = target;
            if (spell.SkillType == SkillType.Line && UseClosestPointOnLine)
                output.CastPosition = GeometryLib.NearestPointOnFiniteLine(LocalPlayer.Instance.Pos().Extend(output.CastPosition, 0.6f),
                    output.CastPosition, Main.MouseWorldPos);
            return output;
        }

        private static Output SDK(Character target, SkillBase spell)
        {
            if (spell.SkillType == SkillType.Line)
            {
                var sdkOutput = new PredictionInput(LocalPlayer.Instance, target, spell.Range, spell.Speed, spell.SpellCollisionRadius,
                    SkillType.Line, 0, IgnoreFlags).GetLinePrediction();

                var output = new Output
                {
                    CanHit = sdkOutput.HitChancePercent > 1,
                    CastPosition = sdkOutput.PredictedPosition,
                    CollisionResult = sdkOutput.CollisionResult,
                    Hitchance = sdkOutput.HitChance,
                    HitchancePercentage = sdkOutput.HitChancePercent,
                    Target = target
                };
                if (UseClosestPointOnLine)
                    output.CastPosition = GeometryLib.NearestPointOnFiniteLine(LocalPlayer.Instance.Pos().Extend(output.CastPosition, 0.6f),
                        output.CastPosition, Main.MouseWorldPos);
                return output;
            }
            else
            {
                var output = new PredictionInput(LocalPlayer.Instance, target, spell.Range, spell.Speed, spell.SpellCollisionRadius, SkillType.Circle,
                    spell.FixedDelay).GetCirclePrediction();
                return new Output
                {
                    CanHit = output.HitChancePercent > 1,
                    CastPosition = output.PredictedPosition,
                    CollisionResult = output.CollisionResult,
                    Hitchance = output.HitChance,
                    HitchancePercentage = output.HitChancePercent,
                    Target = target
                };
            }
        }

        private static Output TestPred(Character target, SkillBase spell)
        {
            if (spell.SkillType == SkillType.Line)
            {
                var output = TestPrediction.GetPrediction(LocalPlayer.Instance.Pos(), target, spell.Range, spell.Speed, spell.SpellCollisionRadius,
                    spell.FixedDelay, 1.75f, true, IgnoreFlags);
                if (UseClosestPointOnLine)
                    output.CastPosition = GeometryLib.NearestPointOnFiniteLine(LocalPlayer.Instance.Pos().Extend(output.CastPosition, 0.6f),
                        output.CastPosition, Main.MouseWorldPos);
                return output;
            }

            return TestPrediction.GetPrediction(LocalPlayer.Instance.Pos(), target, spell.Range, spell.Speed, spell.SpellCollisionRadius,
                spell.FixedDelay);
        }

        public class Output
        {
            public bool CanHit;
            public Vector2 CastPosition;
            public CollisionResult CollisionResult;
            public HitChance Hitchance = HitChance.Unknown;
            public float HitchancePercentage;
            public Character Target;

            public static Output None => new Output {CastPosition = Vector2.Zero, Hitchance = HitChance.Impossible, CanHit = false};
        }
    }
}