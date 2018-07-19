using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Projectile = BattleRight.Core.GameObjects.Projectile;

namespace Hoyer.Common.Debug
{
    public static class LogProjectiles
    {
        public static void Init()
        {
            //InGameObject.OnCreate += InGameObject_OnCreate;
            //Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var activeProjectile in EntitiesManager.ActiveProjectiles)
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
            }
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