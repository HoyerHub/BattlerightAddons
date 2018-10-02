using System;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.Helper;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;

namespace Hoyer.Base.Prediction
{
    namespace TestPrediction2NS
    {
        public enum TestSkilltype
        {
            Line,
            Circle,
        }
        
        public static class TestPrediction
        {
            private const float VeryLowHc = 0f;
            private const float LowHc = 15f;
            private const float MediumHc = 35f;
            private const float HighHc = 55f;
            private const float VeryHighHc = 75f;

            public static Prediction.Output GetNormalLinePrediction(Vector2 fromPos, Character targetUnit, float range, float speed, float radius = 0f,
                bool checkCollision = false)
            {
                return GetPrediction(fromPos, targetUnit, range, speed, radius, 0f, 1.75f, checkCollision,
                    CollisionFlags.NPCBlocker | CollisionFlags.Bush);
            }

            /// <summary>
            /// Gets the prediction.
            /// </summary>
            /// <param name="fromPos">Position where projectile gets fired from.</param>
            /// <param name="targetUnit">The target unit.</param>
            /// <param name="range">The ability range.</param>
            /// <param name="speed">The ability speed.</param>
            /// <param name="radius">The ability radius (for collision).</param>
            /// <param name="fixedDelay">The fixed delay. If greater than 0, will use this fixed delay for calculations instead of getting normal best position prediction</param>
            /// <param name="maxEnemyReactionTime">The maximum enemy reaction time in seconds to calculate HitChance.</param>
            /// <param name="checkCollision">If set to <c>true</c> [check collision].</param>
            /// <param name="ignoreFlags">The ignore flags for collision calculations.</param>
            /// <returns>TestOutput</returns>
            public static Prediction.Output GetPrediction(Vector2 fromPos, Character targetUnit, float range, float speed,
                float radius = 0f,
                float fixedDelay = 0f,
                float maxEnemyReactionTime = 1.75f,
                bool checkCollision = false,
                CollisionFlags ignoreFlags = CollisionFlags.Bush | CollisionFlags.NPCBlocker)
            {
                MapGameObject mapGameObject = targetUnit.Get<MapGameObject>();
                NetworkMovementObject networkMovementObject = targetUnit.Get<NetworkMovementObject>();

                if (mapGameObject == null)
                {
                    Logs.Error("TestPrediction: Object of name " + targetUnit.ObjectName + " has no MapGameObject model");
                    return new Prediction.Output()
                    {
                        CanHit = false,
                        Hitchance = HitChance.Unknown,
                        CastPosition = Vector2.Zero,
                    };
                }

                var targetPos = mapGameObject.Position;

                if (networkMovementObject == null)
                {
                    Logs.Error("TestPrediction: Object of name " + targetUnit.ObjectName + " has no NetworkMovementObject model");
                    return new Prediction.Output()
                    {
                        CanHit = targetPos.Distance(fromPos) <= range,
                        Hitchance = targetPos.Distance(fromPos) <= range ? HitChance.VeryHigh : HitChance.OutOfRange,
                        HitchancePercentage = targetPos.Distance(fromPos) <= range ? 100f : 0f,
                        CastPosition = targetPos,
                    };
                }

                var targetVelocity = networkMovementObject.Velocity;
                var targetRadius = targetUnit.Get<SpellCollisionObject>().SpellCollisionRadius; //TODO: Check if MapCollisionRadius is better

                if (fixedDelay < float.Epsilon) //No fixed delay
                {
                    var predPos = GetStandardPrediction(fromPos, targetPos, speed, targetVelocity);
                    if (predPos == Vector2.Zero)
                    {
                        return new Prediction.Output()
                        {
                            CanHit = false,
                            Hitchance = HitChance.Unknown,
                            CastPosition = Vector2.Zero,
                        };
                    }


                    var solution = new Prediction.Output()
                    {
                        CanHit = true,
                        CastPosition = predPos,
                        Target = targetUnit
                    };

                    var targetCollision = CollisionSolver.CheckThickLineCollision(targetPos, solution.CastPosition, targetRadius);
                    if (targetCollision != null && targetCollision.IsColliding)
                    {
                        solution.CastPosition = targetCollision.CollisionPoint;
                    }

                    if (solution.CastPosition.Distance(fromPos) > range)
                    {
                        solution.CanHit = false;
                        solution.Hitchance = HitChance.OutOfRange;
                    }

                    if (checkCollision)
                    {
                        solution.CollisionResult = CollisionSolver.CheckThickLineCollision(fromPos, solution.CastPosition,
                            radius < float.Epsilon ? 0.01f : radius, ignoreFlags);

                        if (solution.CollisionResult.IsColliding)
                        {
                            solution.CanHit = false;
                            solution.Hitchance = HitChance.Collision;
                        }
                    }

                    solution.HitchancePercentage = GetHitchance(fromPos, solution.CastPosition, speed, maxEnemyReactionTime, false);
                    solution.Hitchance = GetHitchanceEnum(solution.HitchancePercentage);
                    return solution;
                }
                else //WITH fixed delay
                {
                    var predPos = GetFixedDelayPrediction(targetPos, fixedDelay, targetVelocity);

                    Prediction.Output solution = new Prediction.Output()
                    {
                        CanHit = true,
                        CastPosition = predPos,
                    };

                    var targetCollision = CollisionSolver.CheckThickLineCollision(targetPos, solution.CastPosition, targetRadius);
                    if (targetCollision != null && targetCollision.IsColliding)
                    {
                        solution.CastPosition = targetCollision.CollisionPoint;
                    }

                    if (solution.CastPosition.Distance(fromPos) > range)
                    {
                        solution.CanHit = false;
                        solution.Hitchance = HitChance.OutOfRange;
                    }

                    solution.HitchancePercentage = GetHitchance(fromPos, solution.CastPosition, fixedDelay, maxEnemyReactionTime, true);
                    solution.Hitchance = GetHitchanceEnum(solution.HitchancePercentage);
                    return solution;
                }
            }

            private static Vector2 GetStandardPrediction(Vector2 pos1, Vector2 pos2, float speed1, Vector2 vel2)
            {
                Vector2 sol = Vector2.Zero;
                double[] quadSol = null;

                var a = Math.Pow(vel2.X, 2) + Math.Pow(vel2.Y, 2) - Math.Pow(speed1, 2);
                var b = 2 * (vel2.X * (pos2.X - pos1.X) + vel2.Y * (pos2.Y - pos1.Y));
                var c = Math.Pow((pos2.X - pos1.X), 2) + Math.Pow((pos2.Y - pos1.Y), 2);

                if (Math.Abs(a) < double.Epsilon)
                {
                    if (Math.Abs(b) < double.Epsilon)
                    {
                        if (Math.Abs(c) < double.Epsilon)
                        {
                            quadSol = new double[] {0d, 0d};
                        }
                    }
                    else
                    {
                        quadSol = new double[] {(-c / b), (-c / b)};
                    }
                }
                else
                {
                    var discriminant = Math.Pow(b, 2) - 4 * a * c;
                    if (discriminant >= 0)
                    {
                        var sqrtDisc = Math.Sqrt(discriminant);
                        quadSol = new double[] {((-b - sqrtDisc) / (a * 2)), ((-b + sqrtDisc) / (a * 2))};
                    }
                }

                if (quadSol != null)
                {
                    double t0 = quadSol[0];
                    double t1 = quadSol[1];

                    var t = Math.Min(t0, t1);
                    if (t < 0)
                    {
                        t = Math.Max(t0, t1);
                    }

                    if (t > 0)
                    {
                        sol.X = (float) (pos2.X + vel2.X * t);
                        sol.Y = (float) (pos2.Y + vel2.Y * t);
                    }
                }

                return sol;
            }

            private static float GetHitchance(Vector2 fromPos, Vector2 predPos, float variable, float maxEnemyReactionTime = 1.75f,
                bool delayInsteadOfSpeed = false)
            {
                float time = 0f;
                if (!delayInsteadOfSpeed)
                {
                    var speed = variable;
                    var distance = predPos.Distance(fromPos);
                    time = distance / speed;
                }
                else
                {
                    time = variable;
                }

                time = Math.Min(maxEnemyReactionTime, time);

                var hitchance = ((time - maxEnemyReactionTime) / (0.1f - maxEnemyReactionTime)) * 100f; //TODO: Check float.Epsilon
                hitchance = (float) Math.Round(hitchance);
                hitchance = Math.Max(0f, hitchance);
                hitchance = Math.Min(100f, hitchance);
                return hitchance;
            }

            private static HitChance GetHitchanceEnum(float hitchance)
            {
                return hitchance > VeryLowHc && hitchance < LowHc ? HitChance.VeryLow :
                    hitchance >= LowHc && hitchance < MediumHc ? HitChance.Low :
                    hitchance >= MediumHc && hitchance < HighHc ? HitChance.Medium :
                    hitchance >= HighHc && hitchance < VeryHighHc ? HitChance.High :
                    hitchance >= VeryHighHc ? HitChance.VeryHigh : HitChance.VeryLow;
            }

            private static Vector2 GetFixedDelayPrediction(Vector2 targetPos, float fixedDelay, Vector2 targetVelocity)
            {
                return targetPos + targetVelocity * fixedDelay;
            }
        }
    }
}