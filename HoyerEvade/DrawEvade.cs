using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Evade
{
    public static class DrawEvade
    {
        public static bool Active = false;


        public static void Setup()
        {
            Game.OnDraw += Game_OnDraw;
        }

        private static void DrawRectangle(Vector2 start, Vector2 end, float radius, Color color)
        {
            var w = end.X - start.X;
            var h = end.Y - start.Y;
            var l = Math.Sqrt(w * w + h * h);
            var xS = (radius * h / l);
            var yS = (radius * w / l);

            var rightStartPos = new Vector2((float) (start.X - xS), (float) (start.Y + yS));
            var leftStartPos = new Vector2((float)(start.X + xS), (float)(start.Y - yS));
            var rightEndPos = new Vector2((float)(end.X - xS), (float)(end.Y + yS));
            var leftEndPos = new Vector2((float)(end.X + xS), (float)(end.Y - yS));

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
            if(!Game.IsInGame || !Active) return;
            foreach (var projectile in AbilityTracker.Enemy.Projectiles.TrackedProjectiles)
            {
                DrawRectangle(projectile.Projectile.StartPosition, projectile.Projectile.CalculatedEndPosition, projectile.Data.Radius, projectile.IsDangerous ? Color.red : Color.white);
                Drawing.DrawCircle(projectile.Projectile.LastPosition, projectile.Data.Radius / 2, projectile.IsDangerous ? Color.red : Color.white);
            }
            foreach (var throwObj in AbilityTracker.Enemy.CircularThrows.TrackedThrows)
            {
                Drawing.DrawCircle(throwObj.ThrowObject.TargetPosition, throwObj.Data.Radius, throwObj.IsDangerous ? Color.red : Color.white);
                Drawing.DrawString(throwObj.ThrowObject.TargetPosition, (throwObj.EstimatedImpact - Time.time).ToString(), Color.white);
            }
        }
    }

    
}