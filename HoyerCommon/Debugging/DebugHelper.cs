using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Events;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Projectile = BattleRight.Core.GameObjects.Projectile;

namespace Hoyer.Common.Debug
{
    public static class DebugHelper
    {
        public static Dictionary<int, Vector2> JumpStartPosDictionary = new Dictionary<int, Vector2>();
        public static Dictionary<int, Vector2> DashStartPosDictionary = new Dictionary<int, Vector2>();
        
        public static void Setup()
        {
            CommonEvents.Update += Game_OnUpdate;
            SpellDetector.OnSpellCast += SpellDetector_OnSpellCast;
            InGameObject.OnCreate += InGameObject_OnCreate;
            InGameObject.OnDestroy += InGameObject_OnDestroy;
        }

        private static void InGameObject_OnDestroy(InGameObject inGameObject)
        {
            var baseObj = inGameObject.Get<BaseGameObject>();
            if (baseObj.Owner != LocalPlayer.Instance) return;
            
            if (inGameObject is Projectile)
            {
                var projectile = (Projectile) inGameObject;
                Console.WriteLine("Name: " + inGameObject.ObjectName);
                Console.WriteLine("Range: " + projectile.Range);
                Console.WriteLine("Speed: " + projectile.StartPosition.Distance(projectile.LastPosition)/projectile.Get<AgeObject>().Age);
                Console.WriteLine("MapColRadius: " + projectile.Radius);
                Console.WriteLine("----");
            }
            var baseTypes = inGameObject.GetBaseTypes().ToArray();

            if (baseTypes.Contains("TravelBuff"))
            {
                var startPos = JumpStartPosDictionary[inGameObject.Id];
                JumpStartPosDictionary.Remove(inGameObject.Id);
                Console.WriteLine("Name: " + inGameObject.ObjectName);
                Console.WriteLine("Range: " + LocalPlayer.Instance.Pos().Distance(startPos));
                Console.WriteLine("Duration: " + inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("Speed: " + LocalPlayer.Instance.Pos().Distance(startPos) / inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("----");
            }

            if (baseTypes.Contains("Dash"))
            {
                var startPos = DashStartPosDictionary[inGameObject.Id];
                DashStartPosDictionary.Remove(inGameObject.Id);
                Console.WriteLine("Name: " + inGameObject.ObjectName);
                Console.WriteLine("Range: " + LocalPlayer.Instance.Pos().Distance(startPos));
                Console.WriteLine("Duration: " + inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("Speed: " + LocalPlayer.Instance.Pos().Distance(startPos) / inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("----");
            }
        }

        private static void InGameObject_OnCreate(InGameObject inGameObject)
        {
            var baseObj = inGameObject.Get<BaseGameObject>();
            if (baseObj.Owner != LocalPlayer.Instance) return;
            Console.WriteLine("New Object:");
            Console.WriteLine("Name: " + inGameObject.ObjectName);
            var baseTypes = inGameObject.GetBaseTypes().ToArray();
            foreach (var baseType in baseTypes)
            {
                Console.WriteLine(baseType);
            }

            Console.WriteLine("----");
            if (baseTypes.Contains("Throw"))
            {
                var throwObj = inGameObject.Get<ThrowObject>();
                Console.WriteLine("Distance: " + throwObj.StartPosition.Distance(throwObj.TargetPosition));
                Console.WriteLine("Duration: " + throwObj.Duration);
                Console.WriteLine("MapColRadius: " + throwObj.MapCollisionRadius);
                Console.WriteLine("SpellRadius: " + throwObj.SpellCollisionRadius);
                Console.WriteLine("----");
            }

            if (baseTypes.Contains("TravelBuff"))
            {
                JumpStartPosDictionary.Add(inGameObject.Id, LocalPlayer.Instance.Pos());
            }

            if (baseTypes.Contains("Dash"))
            {
                DashStartPosDictionary.Add(inGameObject.Id, LocalPlayer.Instance.Pos());
            }
        }

        private static void SpellDetector_OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
        {
            if(args.Caster.Name != LocalPlayer.Instance.Name) return;

            var absys = args.Caster.AbilitySystem;
            Console.WriteLine("New Cast:");
            Console.WriteLine("Owner: " + args.Caster.CharName);
            Console.WriteLine("Name: " + args.Caster.AbilitySystem.CastingAbilityName);
            Console.WriteLine("Id: " + absys.CastingAbilityId);
            Console.WriteLine("Index: " + args.AbilityIndex);
            Console.WriteLine("----"); 
        }

        private static void Game_OnUpdate()
        {
            /*foreach (var activeProjectile in EntitiesManager.ActiveProjectiles)
            {
                if (activeProjectile.MapObject.Position.Distance(activeProjectile.CalculatedEndPosition) < 0.2)
                {
                    if (Math.Abs(activeProjectile.Age.Age) > 0.001)
                    {
                        Console.WriteLine(activeProjectile.ObjectName);
                        Console.WriteLine(activeProjectile.Range);
                        Console.WriteLine(activeProjectile.Radius);
                        Console.WriteLine(activeProjectile.SpellCollision.SpellCollisionRadius);
                        Console.WriteLine(activeProjectile.MapObject.Position.Distance(activeProjectile.StartPosition) / activeProjectile.Age.Age);
                        Console.WriteLine("----");
                    }
                }
            }*/
        }

        public static readonly List<Projectile> ActiveProjectiles = new List<Projectile>();

        public static void CheckProjectiles()
        {
            foreach (var activeGameObject in EntitiesManager.ActiveProjectiles)
            {
                if (ActiveProjectiles.Any(p => p.ObjectName == activeGameObject.ObjectName))
                {
                    return;
                }
                var info = AbilityDatabase.Get(activeGameObject.ObjectName);
                if (info != null)
                {
                    Console.WriteLine("Found " + info.ObjectName + " as " + info.Champion + "'s " + info.AbilitySlot + " in the Database!");
                    ActiveProjectiles.Add(activeGameObject);
                }
                else Console.WriteLine("Didn't find " + activeGameObject.ObjectName + " in the Database!");
            }
        }
    }
}