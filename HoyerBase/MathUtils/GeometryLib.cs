using System.Collections.Generic;
using System.Linq;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using BattleRight.SDK.ClipperLib;
using Hoyer.Base.Extensions;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Base.MathUtils
{
    public static class GeometryLib
    {
        public static Vector2 NearestPointOnFiniteLine(Vector2 start, Vector2 end, Vector2 pnt)
        {
            var line = (end - start);
            var len = Mathf.Sqrt(line.X * line.X + line.Y * line.Y);
            line = line.Normalized;
            var v = pnt - start;
            var d = Vector2.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return start + line * d;
        }
        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var cx = point.X;
            var cy = point.Y;
            var ax = segmentStart.X;
            var ay = segmentStart.Y;
            var bx = segmentEnd.X;
            var by = segmentEnd.Y;
            var rL = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay)) / ((bx - ax) * (bx - ax) + (by - ay) * (by - ay));
            var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (by - ay));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = rS.CompareTo(rL) == 0;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (by - ay));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        public struct ProjectionInfo
        {
            public bool IsOnSegment;
            public Vector2 LinePoint;
            public Vector2 SegmentPoint;

            public ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
            {
                IsOnSegment = isOnSegment;
                SegmentPoint = segmentPoint;
                LinePoint = linePoint;
            }
        }

        public static bool IsInsidePolygon(this Vector2 point, List<Vector2> points)
        {
            return Clipper.PointInPolygon(new IntPoint(point.X, point.Y), points.ToClipperPath()) == 1;
        }

        public static List<IntPoint> ToClipperPath(this List<Vector2> points)
        {
            var result = new List<IntPoint>(points.Count);
            result.AddRange(points.Select(point => new IntPoint(point.X, point.Y)));
            return result;
        }

        public static List<IntPoint> ToClipperPath(this MapCollisionObject obj, int quality = 20)
        {
            var points = new List<IntPoint>();
            var outRadius = obj.MapCollisionRadius / (float)System.Math.Cos(2 * System.Math.PI / quality);
            for (var i = 1; i <= quality; i++)
            {
                var angle = i * 2 * System.Math.PI / quality;
                var point = new IntPoint(
                    obj.LastPosition.X + outRadius * (float)System.Math.Cos(angle), obj.LastPosition.Y + outRadius * (float)System.Math.Sin(angle));
                points.Add(point);
            }
            return points;
        }

        public static bool IsCollidingWithPlayer(this Projectile projectile, Character player)
        {
            return BattleRight.Core.Geometry.CircleVsThickLine(new Vector2(player.Pos().X, player.Pos().Y), player.MapCollision.MapCollisionRadius + 0.1f,
                projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius, true);
        }
       
        public static bool IsCollidingWithPlayer(this Projectile projectile, Character player, float time)
        {
            var predict = player.PredictPosition(time);
            return BattleRight.Core.Geometry.CircleVsThickLine(new Vector2(predict.X, predict.Y), player.MapCollision.MapCollisionRadius + 0.1f,
                projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius, true);
        }

        public static Vector2 GetClosestOutsidePoint(this Character from, List<Vector2> points)
        {
            var result = new List<Vector2>();
            for (var i = 0; i <= points.Count - 1; i++)
            {
                var sideStart = points[i];
                var sideEnd = points[i == points.Count - 1 ? 0 : i + 1];

                result.Add(from.Pos().ProjectOn(sideStart, sideEnd).SegmentPoint);
            }
            return result.OrderBy(vector2 => vector2.Distance(from.Pos())).FirstOrDefault();
        }

        public static Vector2 Perpendicular(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector2 GetClosestOutsidePoint(this Vector2 from, List<Vector2> points)
        {
            var result = new List<Vector2>();
            for (var i = 0; i <= points.Count - 1; i++)
            {
                var sideStart = points[i];
                var sideEnd = points[i == points.Count - 1 ? 0 : i + 1];

                result.Add(from.ProjectOn(sideStart, sideEnd).SegmentPoint);
            }
            return result.OrderBy(vector2 => vector2.Distance(from)).FirstOrDefault();
        }

        public static List<Vector2> GetBounds(this Projectile projectile)
        {
            var w = projectile.CalculatedEndPosition.X - projectile.StartPosition.X;
            var h = projectile.CalculatedEndPosition.Y - projectile.StartPosition.Y;
            var l = System.Math.Sqrt(w * w + h * h);
            var xS = projectile.Radius * h / l;
            var yS = projectile.Radius * w / l;

            return new List<Vector2>
            {
                new Vector2((float)(projectile.StartPosition.X - xS), (float)(projectile.StartPosition.Y + yS)),
                new Vector2((float)(projectile.StartPosition.X + xS), (float)(projectile.StartPosition.Y - yS)),
                new Vector2((float)(projectile.CalculatedEndPosition.X - xS), (float)(projectile.CalculatedEndPosition.Y + yS)),
                new Vector2((float)(projectile.CalculatedEndPosition.X + xS), (float)(projectile.CalculatedEndPosition.Y - yS))
            };
        }

        public static bool CheckForOverLaps(List<IntPoint> shape1, List<IntPoint> shape2)
        {
            var clipper = Main.Clipper;
            var solution = new List<List<IntPoint>>();
            clipper.Clear();
            solution.Clear();

            clipper.AddPath(shape1, PolyType.ptSubject, true);
            clipper.AddPath(shape2, PolyType.ptClip, true);
            clipper.Execute(ClipType.ctIntersection, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            return solution.Count != 0;
        }

        public static Vector2[] MakeSmoothCurve(Vector2[] arrayToCurve, float smoothness)
        {
            if (smoothness < 1.0f) smoothness = 1.0f;

            var pointsLength = arrayToCurve.Length;
            var curvedLength = pointsLength * Mathf.RoundToInt(smoothness) - 1;
            var curvedPoints = new List<Vector2>(curvedLength);

            for (var pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                var t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                var points = new List<Vector2>(arrayToCurve);

                for (var j = pointsLength - 1; j > 0; j--)
                {
                    for (var i = 0; i < j; i++)
                    {
                        points[i] = (1 - t) * points[i] + t * points[i + 1];
                    }
                }

                curvedPoints.Add(points[0]);
            }

            return curvedPoints.ToArray();
        }
    }
}