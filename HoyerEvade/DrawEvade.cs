using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Trackers;
using Hoyer.Common.Utilities;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Evade
{
    public static class DrawEvade
    {
        public static bool Active = false;


        public static void Setup()
        {
            Game.OnPreUpdate += Game_OnDraw;
        }

        public static void Unload()
        {
            Game.OnPreUpdate -= Game_OnDraw;
        }

        private static void DrawRectangle(Vector2 start, Vector2 end, float radius, Color color)
        {
            var w = end.X - start.X;
            var h = end.Y - start.Y;
            var l = Math.Sqrt(w * w + h * h);
            var xS = (radius * h / l);
            var yS = (radius * w / l);

            var rightStartPos = new Vector2((float) (start.X - xS), (float) (start.Y + yS));
            var leftStartPos = new Vector2((float) (start.X + xS), (float) (start.Y - yS));
            var rightEndPos = new Vector2((float) (end.X - xS), (float) (end.Y + yS));
            var leftEndPos = new Vector2((float) (end.X + xS), (float) (end.Y - yS));

            Drawing.DrawLine(rightStartPos, rightEndPos, color);
            Drawing.DrawLine(leftStartPos, leftEndPos, color);
            Drawing.DrawLine(rightStartPos, leftStartPos, color);
            Drawing.DrawLine(leftEndPos, rightEndPos, color);
        }

        public static UnityEngine.Vector2 ConvertToUnity2D(this Vector2 source)
        {
            return new UnityEngine.Vector2(source.X, Screen.height - source.Y);
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (!Game.IsInGame || !Active) return;
            foreach (var projectile in ObjectTracker.Enemy.Projectiles.TrackedObjects)
            {
                DrawRectangle(projectile.Projectile.StartPosition, projectile.Projectile.CalculatedEndPosition, projectile.Data.Radius,
                    projectile.IsDangerous ? Color.red : Color.white);
                Drawing.DrawCircle(projectile.Projectile.LastPosition, projectile.Data.Radius / 2, projectile.IsDangerous ? Color.red : Color.white);
            }

            foreach (var dash in ObjectTracker.Enemy.Dashes.TrackedObjects)
            {
                DrawRectangle(dash.DashObject.StartPosition, dash.DashObject.TargetPosition, dash.Data.Radius,
                    dash.IsDangerous ? Color.red : Color.white);
            }

            foreach (var throwObj in ObjectTracker.Enemy.CircularThrows.TrackedObjects)
            {
                Drawing.DrawCircle(throwObj.ThrowObject.TargetPosition, throwObj.Data.Radius, throwObj.IsDangerous ? Color.red : Color.white);
                Drawing.DrawString(throwObj.ThrowObject.TargetPosition, (throwObj.EstimatedImpact - Time.time).ToString(), Color.white);
            }

            foreach (var jumpObj in ObjectTracker.Enemy.CircularJumps.TrackedObjects)
            {
                Drawing.DrawCircle(jumpObj.TravelObject.TargetPosition, jumpObj.Data.Radius, jumpObj.IsDangerous ? Color.red : Color.white);
                Drawing.DrawString(jumpObj.TravelObject.TargetPosition, (jumpObj.EstimatedImpact - Time.time).ToString(), Color.white);
            }

            foreach (var projectile in ObjectTracker.Enemy.CurveProjectiles.TrackedObjects)
            {
                Drawing.DrawCircle(projectile.Projectile.LastPosition, projectile.Data.Radius / 2, projectile.IsDangerous ? Color.red : Color.white);
                if (!projectile.Projectile.Reversed)
                {
                    if (Math.Abs(projectile.Projectile.CurveWidth) > 0.1)
                    {
                        for (int i = 1; i < projectile.Path.Count; i++)
                        {
                            Drawing.DrawLine(projectile.Path[i-1], projectile.Path[i], projectile.IsDangerous ? Color.red : Color.white);
                        }
                    }
                    else
                    {
                        DrawRectangle(projectile.StartPosition, projectile.EndPosition, projectile.Data.Radius, projectile.IsDangerous ? Color.red : Color.white);
                    }
                }
                else
                {
                    /*if (Math.Abs(projectile.Projectile.CurveWidth) > 0.1)
                    {
                        for (int i = 1; i < projectile.Path.Count; i++)
                        {
                            Drawing.DrawLine(projectile.Path[i - 1], projectile.Path[i], Color.white);
                        }
                    }
                    else
                    {
                        DrawRectangle(projectile.EndPosition, LocalPlayer.Instance.Pos(), projectile.Data.Radius, Color.white);
                    }*/
                }
            }
        }
    }
}