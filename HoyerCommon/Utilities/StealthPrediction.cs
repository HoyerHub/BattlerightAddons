using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Local;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Common.Utilities
{
    public static class StealthPrediction
    {
        public static Dictionary<string, Vector2> Positions = new Dictionary<string, Vector2>();
        public static List<string> HasSeen = new List<string>();
        public static bool DrawStealthed = true;

        private static readonly Dictionary<string, float> Widths = new Dictionary<string, float>();

        public static void Setup()
        {
            Game.OnUpdate += OnUpdate;
            Game.OnDraw += Game_OnDraw;
            Game.OnMatchStart += Game_OnMatchStart;
            Game.OnMatchStateUpdate += Game_OnMatchStateUpdate;
        }

        private static void Game_OnMatchStateUpdate(MatchStateUpdate args)
        {
            if (args.NewMatchState == MatchState.InRound)
            {
                HasSeen.Clear();
            }
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            Widths.Clear();
            foreach (var character in EntitiesManager.EnemyTeam)
            {
                Widths.Add(character.CharName, character.MapCollision.MapCollisionRadius);
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (DrawStealthed)
            {
                foreach (var item in Positions)
                {
                    Drawing.DrawCircle(item.Value, Widths[item.Key], Color.green);
                    Drawing.DrawString(item.Value, item.Key, Color.green);
                }
            }
        }

        public static void OnUpdate(EventArgs eventArgs)
        {
            if (!Game.IsInGame || Game.CurrentMatchState != MatchState.InRound) return;
            foreach (var character in EntitiesManager.EnemyTeam)
            {
                if (character.Living.IsDead)
                {
                    if(Positions.ContainsKey(character.CharName)) Positions.Remove(character.CharName);
                    continue;
                }
                if (character.CharacterModel.IsModelInvisible)
                {
                    if (!HasSeen.Contains(character.CharName)) continue;
                    if (Positions.ContainsKey(character.CharName))
                    {
                        Positions[character.CharName] = Positions[character.CharName] + character.NetworkMovement.Velocity * Time.deltaTime;
                    }
                    else
                    {
                        Positions.Add(character.CharName, character.MapObject.Position);
                    }
                }
                else
                {
                    if(!HasSeen.Contains(character.CharName)) HasSeen.Add(character.CharName);
                    if (Positions.ContainsKey(character.CharName)) Positions.Remove(character.CharName);
                }
            }
        }
    }
}