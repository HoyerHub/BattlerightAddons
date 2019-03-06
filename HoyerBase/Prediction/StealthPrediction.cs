using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Base.Prediction
{
    public static class StealthPrediction
    {
        public static bool ShouldUse;
        public static Dictionary<string, Vector2> Positions;
        public static Dictionary<string, Vector2> LastSeenPositions;
        public static bool DrawStealthed = true;

        private static readonly Dictionary<string, float> Widths = new Dictionary<string, float>();

        public static void Setup()
        {
            Positions = new Dictionary<string, Vector2>();
            LastSeenPositions = new Dictionary<string, Vector2>();

            Game.OnUpdate += OnUpdate;
            Game.OnPreUpdate += Game_OnDraw;
            Game.OnMatchStart += Game_OnMatchStart;
            Game.OnMatchStateUpdate += Game_OnMatchStateUpdate;
        }

        public static void Unload()
        {
            Game.OnUpdate -= OnUpdate;
            Game.OnPreUpdate -= Game_OnDraw;
            Game.OnMatchStart -= Game_OnMatchStart;
            Game.OnMatchStateUpdate -= Game_OnMatchStateUpdate;
            Positions = null;
            LastSeenPositions = null;
        }

        private static void Game_OnMatchStateUpdate(MatchStateUpdate args)
        {
            Positions.Clear();
            LastSeenPositions.Clear();
        }

        private static void Game_OnMatchStart()
        {
            Widths.Clear();
            foreach (var character in EntitiesManager.EnemyTeam)
            {
                Widths.Add(character.CharName, character.MapCollision.MapCollisionRadius);
            }
        }

        private static void Game_OnDraw()
        {
            if (DrawStealthed)
            {
                foreach (var item in Positions)
                {
                    if (Widths.ContainsKey(item.Key))
                    {
                        Drawing.DrawCircle(item.Value, Widths[item.Key], Color.green);
                        Drawing.DrawString(item.Value, item.Key, Color.green);
                    }
                }
            }
        }

        public static void OnUpdate()
        {
            if (!Game.IsInGame || Game.CurrentMatchState != MatchState.InRound) return;
            foreach (var character in EntitiesManager.EnemyTeam)
            {
                if (character.CharName == null) continue;
                if (character.Living.IsDead)
                {
                    if(Positions.ContainsKey(character.CharName)) Positions.Remove(character.CharName);
                    if (LastSeenPositions.ContainsKey(character.CharName)) LastSeenPositions.Remove(character.CharName);
                    continue;
                }
                if (character.CharacterModel.IsModelInvisible)
                {
                    if (Positions.ContainsKey(character.CharName))
                    {
                        if (LastSeenPositions[character.CharName].Distance(character.MapObject.Position) > 0.3f)
                        {
                            LastSeenPositions[character.CharName] = character.MapObject.Position;
                            Positions[character.CharName] = character.MapObject.Position;
                        }
                        else
                        {
                            Positions[character.CharName] = Positions[character.CharName] + character.NetworkMovement.Velocity * Time.deltaTime;
                        }
                    }
                    else if (character.MapObject.Position.Distance(Vector2.Zero) > 0.3f)
                    {
                        LastSeenPositions.Add(character.CharName, character.MapObject.Position);
                        Positions.Add(character.CharName, character.MapObject.Position);
                    }
                }
                else
                {
                    if (LastSeenPositions.ContainsKey(character.CharName)) LastSeenPositions.Remove(character.CharName);
                    if (Positions.ContainsKey(character.CharName)) Positions.Remove(character.CharName);
                }
            }
        }

        public static Vector2 GetPosition(Character character)
        {
            if (character.CharName == null) return character.MapObject.Position;
            if (ShouldUse && character.Team == BattleRight.Core.Enumeration.Team.Enemy &&
                character.CharacterModel.IsModelInvisible &&
                Positions.ContainsKey(character.CharName))
            {
                var pos = Positions[character.CharName];
                if (pos.Distance(Vector2.Zero) > 0.1f) return pos;
            }

            return character.MapObject.Position;
        }
    }
}