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
using UnityEngine;
using CollisionFlags = BattleRight.Core.Enumeration.CollisionFlags;
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
                public static event Action<TrackedThrowObject> OnDangerous = delegate { };
                public static event Action<TrackedThrowObject> OnDangerousDestroyed = delegate { };

                public static List<TrackedThrowObject> TrackedThrows = new List<TrackedThrowObject>();

                public static void Setup()
                {
                    InGameObject.OnCreate += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryDanger = TrackedThrows.FirstOrDefault(t => t.ThrowObject.GameObject == inGameObject);
                    if (tryDanger != default(TrackedThrowObject))
                    {
                        TrackedThrows.Remove(tryDanger);
                        OnDangerousDestroyed.Invoke(tryDanger);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    if (inGameObject.GetBaseTypes().Contains("Throw") &&
                        inGameObject.Get<BaseGameObject>().TeamId != LocalPlayer.Instance.BaseObject.TeamId)
                    {
                        var throwObj = inGameObject.Get<ThrowObject>();
                        var data = throwObj.Data();
                        if (data == null)
                        {
                            return;
                        }
                        if (LocalPlayer.Instance.Pos().Distance(throwObj.TargetPosition) > 5)
                        {
                            return;
                        }
                        var tto = new TrackedThrowObject(throwObj, data);
                        TrackedThrows.Add(tto);
                        OnDangerous.Invoke(tto);
                    }
                }
            }

            public static class Projectiles
            {
                public static event Action<TrackedProjectile> OnDangerous = delegate { };

                public static List<TrackedProjectile> TrackedProjectiles = new List<TrackedProjectile>();

                public static void Setup()
                {
                    InGameObject.OnCreate += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryFind = TrackedProjectiles.FirstOrDefault(t =>
                        t.Projectile == inGameObject);
                    if (tryFind != default(TrackedProjectile))
                    {
                        TrackedProjectiles.Remove(tryFind);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    var projectile = inGameObject as Projectile;
                    if (projectile != null && inGameObject.Get<BaseGameObject>().TeamId != LocalPlayer.Instance.BaseObject.TeamId)
                    {
                        var data = AbilityDatabase.Get(inGameObject.ObjectName);
                        if (data == null)
                        {
                            return;
                        }
                        var pos = LocalPlayer.Instance.Pos();

                        var closest = GeometryLib.NearestPointOnFiniteLine(projectile.StartPosition,
                            projectile.CalculatedEndPosition, pos);
                        if (pos.Distance(closest) > 5)
                        {
                            return;
                        }

                        var tp = new TrackedProjectile(projectile, data);
                        TrackedProjectiles.Add(tp);
                        OnDangerous.Invoke(tp);
                    }
                }
            }
        }
    }

    public class TrackedProjectile
    {
        public bool IsDangerous;
        public Projectile Projectile;
        public AbilityInfo Data;
        public float EstimatedImpact;
        public Vector2 ClosestPoint;

        public TrackedProjectile(Projectile projectile, AbilityInfo data)
        {
            Projectile = projectile;
            Data = data;
            Update();
        }

        public void Update()
        {
            var pos = LocalPlayer.Instance.Pos();
            ClosestPoint = GeometryLib.NearestPointOnFiniteLine(Projectile.StartPosition,
                Projectile.CalculatedEndPosition, pos);
            EstimatedImpact = Time.time +((pos.Distance(Projectile.LastPosition) -
                                            LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                                           Data.Speed);
            IsDangerous = GetIsDangerous(pos);
        }

        private bool GetIsDangerous(Vector2 pos)
        {
            return IsInsideHitbox(pos) && !CheckForCollision(pos);
        }

        private bool CheckForCollision(Vector2 pos)
        {
            var targetCollision = CollisionSolver.CheckThickLineCollision(ClosestPoint, Projectile.StartPosition, LocalPlayer.Instance.MapCollision.MapCollisionRadius);
            return targetCollision != null && targetCollision.IsColliding &&
                   targetCollision.CollisionFlags.HasFlag(CollisionFlags.LowBlock | CollisionFlags.HighBlock) ;
        }

        private bool IsInsideHitbox(Vector2 pos)
        {
            float num = Vector2.DistanceSquared(ClosestPoint, pos);
            float num2 = LocalPlayer.Instance.MapCollision.MapCollisionRadius + Data.Radius;
            if (num <= num2 * num2)
            {
                Vector2 normalized = (Projectile.CalculatedEndPosition - Projectile.StartPosition).Normalized;
                Vector2 value = pos + normalized * Data.Radius;
                if (Vector2.Dot(normalized, value - Projectile.StartPosition) > 0f)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class TrackedThrowObject
    {
        public bool IsDangerous;
        public ThrowObject ThrowObject;
        public AbilityInfo Data;
        public float EstimatedImpact;
        public Vector2 ClosestOutsidePoint;

        public TrackedThrowObject(ThrowObject throwObject, AbilityInfo data)
        {
            ThrowObject = throwObject;
            Data = data;
            Update();
        }

        public void Update()
        {
            var pos = LocalPlayer.Instance.Pos();
            ClosestOutsidePoint = LocalPlayer.Instance.GetClosestExitPointFromCircle(ThrowObject.TargetPosition, Data.Radius);
            EstimatedImpact = Data.Duration - ThrowObject.GameObject.Get<AgeObject>().Age + Time.time;
            IsDangerous = GetIsDangerous(pos);
        }

        private bool GetIsDangerous(Vector2 pos)
        {
            return IsInsideHitbox(pos);
        }

        private bool IsInsideHitbox(Vector2 pos)
        {
            return pos.Distance(ThrowObject.TargetPosition) < Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius;

        }
    }

    public class CastingProjectile
    {
        public string[] Elements { get; set; }
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