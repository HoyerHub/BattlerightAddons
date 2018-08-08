using System;
using BattleRight.Core;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Common.Debug
{
    public static class DrawProjectiles
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
            foreach (var projectile in EntitiesManager.ActiveProjectiles)
            {
                if (projectile.BaseObject.TeamId == 2)
                {
                    DrawRectangle(projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius, Color.red);
                    Drawing.DrawCircle(projectile.MapObject.Position, projectile.Radius / 2, Color.red);
                }
                else if (projectile.BaseObject.TeamId == 1)
                {
                    DrawRectangle(projectile.StartPosition, projectile.CalculatedEndPosition, projectile.Radius, Color.green);
                    Drawing.DrawCircle(projectile.MapObject.Position, projectile.Radius / 2, Color.green);
                }
            }

            foreach (var character in EntitiesManager.EnemyTeam)
            {
                if (character.CharacterModel.IsModelInvisible)
                {
                    Drawing.DrawCircle(character.Pos(), character.MapCollision.MapCollisionRadius, Color.green);
                    Drawing.DrawString(character.Pos(), character.CharName, Color.green);
                }
            }
                
            foreach (var cast in AbilityTracker.Enemy.Projectiles.Casting)
            {
                DrawRectangle(cast.Caster.Pos(), cast.EndPos, cast.Data.Radius, Color.blue);
            }
            /*var guiStyle = GUI.skin.GetStyle("label");
            guiStyle.fontSize = 15;
            guiStyle.normal.textColor = Color.green;

            var i = 0;
            GUI.Label(new Rect(new UnityEngine.Vector2(Screen.width - 200, 50), new UnityEngine.Vector2(100, 50)), EntitiesManager.LocalPlayer.CharName, guiStyle);
            foreach (var buff in EntitiesManager.LocalPlayer.Buffs)
            {
                i++;
                GUI.Label(new Rect(new UnityEngine.Vector2(Screen.width - 200, i*70), new UnityEngine.Vector2(100, 50)), buff.ObjectName, guiStyle);
                i++;
                GUI.Label(new Rect(new UnityEngine.Vector2(Screen.width - 200, i * 70), new UnityEngine.Vector2(100, 50)), buff.BuffType.ToString(), guiStyle);
            }*/
        }
    }

    
}