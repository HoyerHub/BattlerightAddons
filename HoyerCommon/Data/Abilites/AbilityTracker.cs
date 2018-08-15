using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
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
                public static event Action<ThrowObject> OnDangerous = delegate { };
                public static event Action<ThrowObject> OnDangerousDestroyed = delegate { };

                public static List<ThrowObject> Dangerous = new List<ThrowObject>();
                public static List<ThrowObject> NonDangerous = new List<ThrowObject>();

                public static void Setup()
                {
                    InGameObject.OnCreate += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryNonDanger = NonDangerous.FirstOrDefault(t => t.GameObject.Id == inGameObject.Id);
                    if (tryNonDanger != default(ThrowObject))
                    {
                        NonDangerous.Remove(tryNonDanger);
                    }
                    var tryDanger = Dangerous.FirstOrDefault(t => t.GameObject.Id == inGameObject.Id);
                    if (tryDanger != default(ThrowObject))
                    {
                        Dangerous.Remove(tryDanger);
                        OnDangerousDestroyed.Invoke(tryDanger);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    if (inGameObject.GetBaseTypes().Contains("Throw") && inGameObject.Get<BaseGameObject>().TeamId != LocalPlayer.Instance.BaseObject.TeamId)
                    {
                        var throwObj = inGameObject.Get<ThrowObject>();
                        var data = throwObj.Data();
                        if (data != null)
                        {
                            if (throwObj.TargetPosition.Distance(LocalPlayer.Instance) < data.Radius)
                            {
                                Dangerous.Add(throwObj);
                                OnDangerous.Invoke(throwObj);
                            }
                            else NonDangerous.Add(throwObj);
                        }
                    }
                }
            }

            public static class Projectiles
            {
                public static event Action<Projectile> OnDangerous = delegate { };
                public static event Action<Projectile> OnDangerousDestroyed = delegate { };

                public static List<Projectile> Dangerous = new List<Projectile>();
                public static List<Projectile> NonDangerous = new List<Projectile>();

                public static void Setup()
                {
                    InGameObject.OnCreate += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryNonDanger = NonDangerous.FirstOrDefault(t => t.Id == inGameObject.Id);
                    if (tryNonDanger != default(Projectile))
                    {
                        NonDangerous.Remove(tryNonDanger);
                    }
                    var tryDanger = Dangerous.FirstOrDefault(t => t.Id == inGameObject.Id);
                    if (tryDanger != default(Projectile))
                    {
                        Dangerous.Remove(tryDanger);
                        OnDangerousDestroyed.Invoke(tryDanger);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    var data = AbilityDatabase.Get(inGameObject.ObjectName);
                    if (data != null && data.AbilityType == AbilityType.LineProjectile && inGameObject.Get<BaseGameObject>().TeamId != LocalPlayer.Instance.BaseObject.TeamId)
                    {
                        var projectile = inGameObject as Projectile;
                        if (projectile.WillCollideWithPlayer(LocalPlayer.Instance, data.Radius / 2))
                        {
                            Dangerous.Add(projectile);
                            OnDangerous.Invoke(projectile);
                        }
                        else NonDangerous.Add(projectile);
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