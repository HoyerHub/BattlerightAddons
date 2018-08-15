using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using BattleRight.SDK.Events;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using UnityEngine;
using Projectile = BattleRight.Core.GameObjects.Projectile;

namespace Hoyer.Common.Debug
{
    public static class DebugHelper
    {
        public static void Setup()
        {
            CommonEvents.Update += Game_OnUpdate;
            SpellDetector.OnSpellCast += SpellDetector_OnSpellCast;
            InGameObject.OnCreate += InGameObject_OnCreate;
        }

        private static void InGameObject_OnCreate(InGameObject inGameObject)
        {
            Console.WriteLine("New Object:");
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
        }

        private static void SpellDetector_OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
        {
            if(args.Caster.Name != LocalPlayer.Instance.Name) return;

            var absys = args.Caster.AbilitySystem;
            Console.WriteLine("New Cast:");
            Console.WriteLine("Owner: " + args.Caster.CharName);
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