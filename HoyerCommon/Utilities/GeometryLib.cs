﻿using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.ClipperLib;
using Hoyer.Common.Extensions;

namespace Hoyer.Common.Utilities
{
    public static class GeometryLib
    {
        private static Clipper _clipper = new Clipper();
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

        public static bool IsCollidingWithPlayer(this Projectile projectile, Character player)
        {
            return Geometry.CircleVsThickLine(new Vector2(player.Pos().X, player.Pos().Y), player.MapCollision.MapCollisionRadius + 0.1f,
                projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius, true);
        }
       
        public static bool IsCollidingWithPlayer(this Projectile projectile, Character player, float time)
        {
            var predict = player.PredictPosition(time);
            return Geometry.CircleVsThickLine(new Vector2(predict.X, predict.Y), player.MapCollision.MapCollisionRadius + 0.1f,
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
            var l = Math.Sqrt(w * w + h * h);
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

        public static Vector2 Extend(this Vector2 source, Vector2 target, float range)
        {
            return source + range * (target - source).Normalized;
        }
    }
}