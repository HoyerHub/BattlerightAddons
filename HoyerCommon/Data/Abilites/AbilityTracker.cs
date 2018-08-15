using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK.Events;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Common.Data.Abilites
{
    public static class AbilityTracker
    {
        public static void Setup()
        {
            Enemy.Projectiles.Setup();
            Enemy.CircularThrows.Setup();
            Enemy.Cooldowns.Setup();
        }

        public static class Enemy
        {
            public static class Cooldowns
            {
                private static Dictionary<string, Dictionary<int, bool>> _abilityStates = new Dictionary<string, Dictionary<int, bool>>();

                public static void Setup()
                {
                    Game.OnMatchStart += OnMatchStart;
                    Game.OnUpdate += Game_OnUpdate;
                }

                private static void Game_OnUpdate(EventArgs args)
                {
                    return;
                    foreach (var character in EntitiesManager.EnemyTeam)
                    {
                        foreach (var abilityState in _abilityStates[character.CharName])
                        {
                            var data = AbilityDatabase.GetDodge(abilityState.Key);
                            if (data.AbilityIndex != -1)
                            {
                                //Console.WriteLine(character.AbilitySystem.GetAbility(data.AbilityIndex).CooldownEndTime);
                            }
                        }
                    }
                }

                private static void OnMatchStart(EventArgs args)
                {
                    _abilityStates.Clear();
                    foreach (var character in EntitiesManager.EnemyTeam)
                    {
                        var abilities = AbilityDatabase.GetDodge(character.CharName);
                        var dict = abilities.ToDictionary(abilityInfo => abilityInfo.AbilityId, abilityInfo => true);
                        _abilityStates.Add(character.CharName, dict);
                    }
                }
            }

            public static class CircularThrows
            {
                public static List<ThrowObject> Active = new List<ThrowObject>();

                public static void Setup()
                {
                    InGameObject.OnCreate += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var active = Active.FirstOrDefault(t => t.GameObject.Id == inGameObject.Id);
                    if (active != default(ThrowObject))
                    {
                        Active.Remove(active);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    if (inGameObject.GetBaseTypes().Contains("Throw"))
                    {
                        var throwObj = inGameObject.Get<ThrowObject>();
                        if (throwObj.Data() != null) Active.Add(throwObj);
                    }

                    /*DEBUG
                    foreach (var baseType in inGameObject.GetBaseTypes())
                    {
                        Console.WriteLine(baseType);
                        if (baseType == "Throw")
                        {
                            var throwObj = inGameObject.Get<ThrowObject>();
                            Console.WriteLine("Name: " + inGameObject.ObjectName);
                            Console.WriteLine("Start: " + throwObj.StartPosition);
                            Console.WriteLine("Target: " + throwObj.TargetPosition);
                            Console.WriteLine("Distance: " + throwObj.StartPosition.Distance(throwObj.TargetPosition));
                            Console.WriteLine("Duration: " + throwObj.Duration);
                            Console.WriteLine("MapColRadius: " + throwObj.MapCollisionRadius);
                            Console.WriteLine("SpellRadius: " + throwObj.SpellCollisionRadius);
                            Drawing.DrawCircleOneShot(throwObj.TargetPosition, throwObj.SpellCollisionRadius, Color.green, throwObj.Duration);
                            Drawing.DrawLineOneShot(throwObj.StartPosition, throwObj.TargetPosition, Color.green, throwObj.Duration);
                        }
                    }
                    Console.WriteLine("----");
                    */
                }
            }

            public static class Projectiles
            {
                private static readonly List<CastingProjectile> Casting = new List<CastingProjectile>();
                public static List<Projectile> Active = new List<Projectile>();

                public static void Setup()
                {
                    Game.OnUpdate += OnUpdate;
                    //SpellDetector.OnSpellCast += OnSpellCast;
                }

                private static void OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
                {
                    if (args.Caster.Id == 25) return;
                    if (args.Caster.Team == BattleRight.Core.Enumeration.Team.Enemy)
                    {
                        //Console.WriteLine(args.Caster.AbilitySystem.CastingAbilityName + ": " + args.Caster.AbilitySystem.CastingAbilityId);
                        var abilityInfo = AbilityDatabase.Get(args.Caster.AbilitySystem.CastingAbilityId);
                        if (abilityInfo != null)
                        {
                            //Console.WriteLine("AbilityDatabase: Data found for spell: " + abilityInfo.ObjectName);
                            Casting.Add(new CastingProjectile(abilityInfo, args.Caster));
                        }
                    }
                }

                private static void OnUpdate(EventArgs args)
                {
                    Active.Clear();
                    Active.AddRange(EntitiesManager.ActiveProjectiles.Where(p => p.BaseObject.TeamId != LocalPlayer.Instance.BaseObject.TeamId));
                    //CheckForCasts();
                    //UpdateCasts();
                }

                private static void UpdateCasts()
                {
                    foreach (var cast in Casting)
                    {
                        cast.Update();
                    }
                }

                private static void CheckForCasts()
                {
                    var toRemove = new List<CastingProjectile>();
                    foreach (var cast in Casting)
                    {
                        if (!cast.Caster.AbilitySystem.IsCasting || cast.Caster.AbilitySystem.IsPostCasting) toRemove.Add(cast);
                    }

                    foreach (var projectile in toRemove)
                    {
                        Casting.Remove(projectile);
                    }
                }
            }
        }
    }

    public class CastingProjectile
    {
        public Character Caster;
        public AbilityInfo Data;
        public Vector2 EndPos;

        public CastingProjectile(AbilityInfo ability, Character caster)
        {
            Caster = caster;
            Data = ability;
            Update();
        }

        public void Update()
        {
            UpdateEndPos();
        }

        private void UpdateEndPos()
        {
            EndPos = Caster.Pos().Extend(InputManager.MousePosition, Data.Range);
        }

        public bool WillCollideWithPlayer
        {
            get
            {
                return Geometry.CircleVsThickLine(LocalPlayer.Instance.Pos(),
                    LocalPlayer.Instance.MapCollision.MapCollisionRadius, Caster.Pos(),
                    EndPos, Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius + 0.1f, true);
            }
        }
    }
}