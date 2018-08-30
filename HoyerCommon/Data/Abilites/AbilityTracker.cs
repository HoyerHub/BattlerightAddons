using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using BattleRight.SDK.ClipperLib;
using BattleRight.SDK.Events;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;
using UnityEngine;
using CollisionFlags = BattleRight.Core.Enumeration.CollisionFlags;
using Vector2 = BattleRight.Core.Math.Vector2;
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Hoyer.Common.Data.Abilites
{
    public static class AbilityTracker
    {
        internal static event Action<InGameObject> EnemyObjectSpawn = delegate {};

        public static void Setup()
        {
            Enemy.Projectiles.Setup();
            Enemy.CurveProjectiles.Setup();
            Enemy.CircularThrows.Setup();
            Enemy.CircularJumps.Setup();
            Enemy.Dashes.Setup();
            Enemy.Obstacles.Setup();
            Enemy.Cooldowns.Setup();
            Enemy.Casts.Setup();
            InGameObject.OnCreate += OnCreateHandler;
        }

        public static void Unload()
        {
            Enemy.Projectiles.Unload();
            Enemy.CurveProjectiles.Unload();
            Enemy.CircularThrows.Unload();
            Enemy.CircularJumps.Unload();
            Enemy.Dashes.Unload();
            Enemy.Obstacles.Unload();
            Enemy.Cooldowns.Unload();
            Enemy.Casts.Unload();
            InGameObject.OnCreate -= OnCreateHandler;
        }

        private static void OnCreateHandler(InGameObject gameObject)
        {
            var baseTypes = gameObject.GetBaseTypes();
            if (!baseTypes.Contains("BaseObject")) return;
            var baseObj = gameObject.Get<BaseGameObject>();
            if (baseObj != null && baseObj.TeamId != LocalPlayer.Instance.BaseObject.TeamId)
            {
                EnemyObjectSpawn.Invoke(gameObject);
            }
        }

        public static class Enemy
        {
            public static class Casts
            {
                public static List<TrackedCast> TrackedCasts = new List<TrackedCast>();

                public static void Setup()
                {
                }

                public static void Unload()
                {
                    SpellDetector.OnSpellCast -= SpellDetector_OnSpellCast;
                    SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
                    TrackedCasts.Clear();
                }

                private static void SpellDetector_OnSpellStopCast(BattleRight.SDK.EventsArgs.SpellStopArgs args)
                {
                    var tryGetCast = TrackedCasts.FirstOrDefault(t => t.Owner.Name == args.Caster.Name && t.Index == args.AbilityIndex);
                    if (tryGetCast != default(TrackedCast))
                    {
                        TrackedCasts.Remove(tryGetCast);
                    }
                }

                private static void SpellDetector_OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
                {
                    if (args.Caster.Team == BattleRight.Core.Enumeration.Team.Enemy) return;
                    var data = AbilityDatabase.Get(args.Caster.AbilitySystem.CastingAbilityId);
                    if (data == null) return;
                    TrackedCasts.Add(new TrackedCast(args.AbilityIndex, args.Caster, data));
                }
            }

            public static class Cooldowns
            {
                private static readonly Dictionary<string, Dictionary<int, bool>> AbilityStates = new Dictionary<string, Dictionary<int, bool>>();

                public static void Setup()
                {
                }

                public static void Unload()
                {
                    Game.OnMatchStart -= OnMatchStart;
                    Game.OnUpdate -= Game_OnUpdate;
                    AbilityStates.Clear();
                }

                private static void Game_OnUpdate(EventArgs args)
                {
                    return;
                    /*if (Input.GetKeyDown(KeyCode.L))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Console.Write(EntitiesManager.EnemyTeam[0].AbilitySystem.GetAbility(i).Name + " - ");
                            Console.WriteLine(EntitiesManager.EnemyTeam[0].AbilitySystem.GetAbility(i).Cooldown);
                            Console.Write(LocalPlayer.Instance.AbilitySystem.GetAbility(i).Name + " - ");
                            Console.WriteLine(LocalPlayer.Instance.AbilitySystem.GetAbility(i).Cooldown);
                        }
                    }
                    foreach (var character in EntitiesManager.EnemyTeam)
                    {
                        foreach (var abilityState in AbilityStates[character.CharName])
                        {
                            var data = AbilityDatabase.GetDodge(abilityState.Key);
                            if (data.AbilityIndex != -1)
                            {
                            }
                        }
                    }*/
                }

                private static void OnMatchStart(EventArgs args)
                {
                    AbilityStates.Clear();
                    foreach (var character in EntitiesManager.EnemyTeam)
                    {
                        var abilities = AbilityDatabase.GetDodge(character.CharName);
                        var dict = abilities.ToDictionary(abilityInfo => abilityInfo.AbilityId, abilityInfo => true);
                        AbilityStates.Add(character.CharName, dict);
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
                    EnemyObjectSpawn += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                public static void Unload()
                {
                    EnemyObjectSpawn -= InGameObject_OnCreate;
                    InGameObject.OnDestroy -= InGameObject_OnDestroy;
                    TrackedThrows.Clear();
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
                    if (inGameObject.GetBaseTypes().Contains("Throw"))
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

            public static class CircularJumps
            {
                public static event Action<TrackedCircularJump> OnDangerous = delegate { };
                public static event Action<TrackedCircularJump> OnDangerousDestroyed = delegate { };

                public static List<TrackedCircularJump> TrackedCircularJumps = new List<TrackedCircularJump>();

                public static void Setup()
                {
                    EnemyObjectSpawn += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                public static void Unload()
                {
                    EnemyObjectSpawn -= InGameObject_OnCreate;
                    InGameObject.OnDestroy -= InGameObject_OnDestroy;
                    TrackedCircularJumps.Clear();
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryDanger = TrackedCircularJumps.FirstOrDefault(t => t.TravelObject.GameObject == inGameObject);
                    if (tryDanger != default(TrackedCircularJump))
                    {
                        TrackedCircularJumps.Remove(tryDanger);
                        OnDangerousDestroyed.Invoke(tryDanger);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    if (inGameObject.GetBaseTypes().Contains("TravelBuff"))
                    {
                        var travelObj = inGameObject.Get<TravelBuffObject>();
                        var data = travelObj.Data();
                        if (data == null || data.AbilityType != AbilityType.CircleJump)
                        {
                            return;
                        }
                        if (LocalPlayer.Instance.Pos().Distance(travelObj.TargetPosition) > 5)
                        {
                            return;
                        }
                        var tcj = new TrackedCircularJump(travelObj, data);
                        TrackedCircularJumps.Add(tcj);
                        OnDangerous.Invoke(tcj);
                    }
                }
            }

            public static class Obstacles
            {
                public static List<TrackedObstacleObject> TrackedObstacles = new List<TrackedObstacleObject>();

                public static void Setup()
                {
                    EnemyObjectSpawn += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                public static void Unload()
                {
                    EnemyObjectSpawn -= InGameObject_OnCreate;
                    InGameObject.OnDestroy -= InGameObject_OnDestroy;
                    TrackedObstacles.Clear();
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryFind = TrackedObstacles.FirstOrDefault(t =>
                        t.MapObject.GameObject == inGameObject);
                    if (tryFind != default(TrackedObstacleObject))
                    {
                        TrackedObstacles.Remove(tryFind);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                        var data = AbilityDatabase.GetObstacle(inGameObject.ObjectName);
                        if (data == null)
                        {
                            return;
                        }

                        TrackedObstacles.Add(new TrackedObstacleObject(inGameObject.Get<MapGameObject>(), data));
                }
            }

            public static class Projectiles
            {
                public static event Action<TrackedProjectile> OnDangerous = delegate { };

                public static List<TrackedProjectile> TrackedProjectiles = new List<TrackedProjectile>();

                public static void Setup()
                {
                    EnemyObjectSpawn += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                public static void Unload()
                {
                    EnemyObjectSpawn -= InGameObject_OnCreate;
                    InGameObject.OnDestroy -= InGameObject_OnDestroy;
                    TrackedProjectiles.Clear();
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
                    if (projectile != null)
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

            public static class CurveProjectiles
            {
                public static event Action<TrackedCurveProjectile> OnDangerous = delegate { };

                public static List<TrackedCurveProjectile> TrackedProjectiles = new List<TrackedCurveProjectile>();

                public static void Setup()
                {
                    EnemyObjectSpawn += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                public static void Unload()
                {
                    EnemyObjectSpawn -= InGameObject_OnCreate;
                    InGameObject.OnDestroy -= InGameObject_OnDestroy;
                    TrackedProjectiles.Clear();
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryFind = TrackedProjectiles.FirstOrDefault(t =>
                        t.Projectile.GameObject == inGameObject);
                    if (tryFind != default(TrackedCurveProjectile))
                    {
                        TrackedProjectiles.Remove(tryFind);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    var baseTypes = inGameObject.GetBaseTypes().ToArray();
                    if (baseTypes.Contains("CurveProjectile") || baseTypes.Contains("CurveProjectile2"))
                    { 
                        var data = AbilityDatabase.Get(inGameObject.ObjectName);
                        if (data == null)
                        {
                            return;
                        }
                        var pos = LocalPlayer.Instance.Pos();
                        var projectile = inGameObject.Get<CurveProjectileObject>();

                        var closest = GeometryLib.NearestPointOnFiniteLine(projectile.Position,
                            projectile.TargetPosition, pos);
                        if (pos.Distance(closest) > 6)
                        {
                            return;
                        }

                        var tp = new TrackedCurveProjectile(projectile, data);
                        TrackedProjectiles.Add(tp);
                        OnDangerous.Invoke(tp);
                    }
                }
            }

            public static class Dashes
            {
                public static event Action<TrackedDash> OnDangerous = delegate { };

                public static List<TrackedDash> TrackedDashes = new List<TrackedDash>();

                public static void Setup()
                {
                    EnemyObjectSpawn += InGameObject_OnCreate;
                    InGameObject.OnDestroy += InGameObject_OnDestroy;
                }

                public static void Unload()
                {
                    EnemyObjectSpawn -= InGameObject_OnCreate;
                    InGameObject.OnDestroy -= InGameObject_OnDestroy;
                    TrackedDashes.Clear();
                }

                private static void InGameObject_OnDestroy(InGameObject inGameObject)
                {
                    var tryFind = TrackedDashes.FirstOrDefault(t =>
                        t.DashObject.GameObject == inGameObject);
                    if (tryFind != default(TrackedDash))
                    {
                        TrackedDashes.Remove(tryFind);
                    }
                }

                private static void InGameObject_OnCreate(InGameObject inGameObject)
                {
                    var baseTypes = inGameObject.GetBaseTypes().ToArray();
                    if (baseTypes.Contains("Dash"))
                    {
                        var dashObj = inGameObject.Get<DashObject>();
                        var data = dashObj.Data();
                        if (data == null)
                        {
                            return;
                        }
                        var pos = LocalPlayer.Instance.Pos();
                        var closest = GeometryLib.NearestPointOnFiniteLine(dashObj.StartPosition,
                            dashObj.TargetPosition, pos);
                        if (pos.Distance(closest) > 5)
                        {
                            return;
                        }
                        var dash = new TrackedDash(dashObj, data);
                        TrackedDashes.Add(dash);
                        OnDangerous.Invoke(dash);
                    }
                }
            }
        }
    }

    public class TrackedCurveProjectile
    {
        public bool IsDangerous;
        public CurveProjectileObject Projectile;
        public AbilityInfo Data;
        public List<Vector2> Path;
        public float EstimatedImpact;
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public Vector2 ClosestPoint;


        public TrackedCurveProjectile(CurveProjectileObject curveProjectile, AbilityInfo data)
        {
            StartPosition = curveProjectile.Position;
            EndPosition = curveProjectile.TargetPosition;
            Projectile = curveProjectile;
            Data = data;
            Path = new List<Vector2>();
            if (Math.Abs(Projectile.CurveWidth) > 0.1)
            {
                var middleLength = StartPosition.Distance(EndPosition) / 2;
                var middleOfLine = StartPosition.Extend(EndPosition, middleLength);
                var perpendicular = (EndPosition - StartPosition).Normalized.Perpendicular();
                var offset = -perpendicular * Math.Sign(Projectile.CurveWidth) * data.Radius;
                var middleOfArc = middleOfLine + -perpendicular * Projectile.CurveWidth;
                Path.AddRange(GeometryLib.MakeSmoothCurve(new[] { StartPosition, middleOfArc + offset, EndPosition }, 3));
                Path.AddRange(GeometryLib.MakeSmoothCurve(new[] { EndPosition + offset, middleOfArc + offset * 2, StartPosition + offset }, 3));
                Path.Add(StartPosition);
            }
            Update();
        }

        public void Update()
        {
            var pos = LocalPlayer.Instance.Pos();
            ClosestPoint = GeometryLib.NearestPointOnFiniteLine(StartPosition,
                EndPosition, pos);
            EstimatedImpact = Time.time + (pos.Distance(Projectile.GameObject.Get<BaseGameObject>().Owner as Character) -
                                           LocalPlayer.Instance.MapCollision.MapCollisionRadius) /
                              Data.Speed;
            IsDangerous = GetIsDangerous(pos); 
        }

        private bool GetIsDangerous(Vector2 pos)
        {
            if (Projectile.Reversed) return false;
            return Math.Abs(Projectile.CurveWidth) > 0.1 ? 
                GeometryLib.CheckForOverLaps(Path.ToClipperPath(), LocalPlayer.Instance.MapCollision.ToClipperPath()) : 
                IsInsideHitbox(pos);
        }

        private bool IsInsideHitbox(Vector2 pos)
        {
            float num = Vector2.DistanceSquared(ClosestPoint, pos);
            float num2 = LocalPlayer.Instance.MapCollision.MapCollisionRadius + Data.Radius;
            if (num <= num2 * num2)
            {
                Vector2 normalized = (EndPosition - StartPosition).Normalized;
                Vector2 value = pos + normalized * Data.Radius;
                if (Vector2.Dot(normalized, value - StartPosition) > 0f)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class TrackedCast
    {
        public int Index;
        public Character Owner;
        public AbilityInfo Data;

        public TrackedCast(int index, Character owner, AbilityInfo data)
        {
            Index = index;
            Owner = owner;
            Data = data;
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

    public class TrackedDash
    {
        public bool IsDangerous;
        public DashObject DashObject;
        public AbilityInfo Data;
        public float EstimatedImpact;
        public Vector2 ClosestPoint;

        public TrackedDash(DashObject dashObject, AbilityInfo data)
        {
            DashObject = dashObject;
            Data = data;
            Update();
        }

        public void Update()
        {
            var pos = LocalPlayer.Instance.Pos();
            ClosestPoint = GeometryLib.NearestPointOnFiniteLine(DashObject.StartPosition,
                DashObject.TargetPosition, pos);
            EstimatedImpact = Time.time + ((pos.Distance(DashObject.GameObject.Get<BaseGameObject>().Owner as Character) -
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
            var targetCollision = CollisionSolver.CheckThickLineCollision(ClosestPoint, DashObject.StartPosition, LocalPlayer.Instance.MapCollision.MapCollisionRadius);
            return targetCollision != null && targetCollision.IsColliding &&
                   targetCollision.CollisionFlags.HasFlag(CollisionFlags.LowBlock | CollisionFlags.HighBlock);
        }

        private bool IsInsideHitbox(Vector2 pos)
        {
            float num = Vector2.DistanceSquared(ClosestPoint, pos);
            float num2 = LocalPlayer.Instance.MapCollision.MapCollisionRadius + Data.Radius;
            if (num <= num2 * num2)
            {
                Vector2 normalized = (DashObject.TargetPosition - DashObject.StartPosition).Normalized;
                Vector2 value = pos + normalized * Data.Radius;
                if (Vector2.Dot(normalized, value - DashObject.StartPosition) > 0f)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class TrackedObstacleObject
    {
        public MapGameObject MapObject;
        public ObstacleAbilityInfo Data;

        public TrackedObstacleObject(MapGameObject mapObject, ObstacleAbilityInfo data)
        {
            MapObject = mapObject;
            Data = data;
        }

        public bool BlocksProjectileTo(Character character, float projectileRadius)
        {
            return Geometry.CircleVsThickLine(MapObject.Position, Data.Radius, LocalPlayer.Instance.Pos(),
                character.Pos(), projectileRadius, true);
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
            EstimatedImpact = Data.FixedDelay - ThrowObject.GameObject.Get<AgeObject>().Age + Time.time;
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

    public class TrackedCircularJump
    {
        public bool IsDangerous;
        public TravelBuffObject TravelObject;
        public AbilityInfo Data;
        public float EstimatedImpact;
        public Vector2 ClosestOutsidePoint;

        public TrackedCircularJump(TravelBuffObject travelObject, AbilityInfo data)
        {
            TravelObject = travelObject;
            Data = data;
            Update();
        }

        public void Update()
        {
            var pos = LocalPlayer.Instance.Pos();
            ClosestOutsidePoint = LocalPlayer.Instance.GetClosestExitPointFromCircle(TravelObject.TargetPosition, Data.Radius);
            EstimatedImpact = Data.FixedDelay - TravelObject.GameObject.Get<AgeObject>().Age + Time.time;
            IsDangerous = GetIsDangerous(pos);
        }

        private bool GetIsDangerous(Vector2 pos)
        {
            return IsInsideHitbox(pos);
        }

        private bool IsInsideHitbox(Vector2 pos)
        {
            return pos.Distance(TravelObject.TargetPosition) < Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius;

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