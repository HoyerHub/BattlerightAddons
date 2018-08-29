using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Events;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Projectile = BattleRight.Core.GameObjects.Projectile;

namespace Hoyer.Common.Debug
{
    public static class DebugHelper
    {
        private static bool _onlyLocal;
        private static bool _enabled;
        
        public static void Setup()
        {
            SpellDetector.OnSpellCast += SpellDetector_OnSpellCast;
            InGameObject.OnCreate += InGameObject_OnCreate;
            InGameObject.OnDestroy += InGameObject_OnDestroy;
            MenuEvents.Initialize += MenuEvents_Initialize;
        }

        public static void Unload()
        {
            SpellDetector.OnSpellCast -= SpellDetector_OnSpellCast;
            InGameObject.OnCreate -= InGameObject_OnCreate;
            InGameObject.OnDestroy -= InGameObject_OnDestroy;
            MenuEvents.Initialize -= MenuEvents_Initialize;
        }

        private static void MenuEvents_Initialize()
        {
            Main.DelayAction(delegate
            {
                var hoyerMainMenu = MainMenu.GetMenu("Hoyer.MainMenu");
                var debugMenu = hoyerMainMenu.Add(new Menu("Hoyer.DebugMenu", "Debug", true));

                var enabled = debugMenu.Add(new MenuCheckBox("debug_enabled", "Enable Debug logging", false));
                enabled.OnValueChange += delegate(ChangedValueArgs<bool> args)
                {
                    _enabled = args.NewValue;
                };
                var onlyLocalPlayer = debugMenu.Add(new MenuCheckBox("debug_onlyLocal", "Only log LocalPlayer"));
                onlyLocalPlayer.OnValueChange += args => _onlyLocal = args.NewValue;
                _enabled = enabled;
                _onlyLocal = onlyLocalPlayer;
            }, 0.1f);
        }

        private static void InGameObject_OnDestroy(InGameObject inGameObject)
        {
            if(!_enabled) return;
            var baseTypes = inGameObject.GetBaseTypes().ToArray();
            if (!baseTypes.Contains("BaseObject")) return;
            var baseObj = inGameObject.Get<BaseGameObject>();
            if (_onlyLocal && baseObj.Owner != LocalPlayer.Instance) return;
            
            if (inGameObject is Projectile)
            {
                var projectile = (Projectile) inGameObject;
                Console.WriteLine("Name: " + inGameObject.ObjectName);
                Console.WriteLine("Range: " + projectile.Range);
                Console.WriteLine("Speed: " + projectile.StartPosition.Distance(projectile.LastPosition)/projectile.Get<AgeObject>().Age);
                Console.WriteLine("MapColRadius: " + projectile.Radius);
                Console.WriteLine("----");
            }

            if (baseTypes.Contains("TravelBuff"))
            {
                var startPos = inGameObject.Get<TravelBuffObject>().StartPosition;
                Console.WriteLine("Name: " + inGameObject.ObjectName);
                Console.WriteLine("Range: " + LocalPlayer.Instance.Pos().Distance(startPos));
                Console.WriteLine("Duration: " + inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("Speed: " + LocalPlayer.Instance.Pos().Distance(startPos) / inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("----");
            }
            if (baseTypes.Contains("Dash"))
            {
                var startPos = inGameObject.Get<DashObject>().StartPosition;
                Console.WriteLine("Name: " + inGameObject.ObjectName);
                Console.WriteLine("Range: " + LocalPlayer.Instance.Pos().Distance(startPos));
                Console.WriteLine("Duration: " + inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("Speed: " + LocalPlayer.Instance.Pos().Distance(startPos) / inGameObject.Get<AgeObject>().Age);
                Console.WriteLine("----");
            }
        }

        private static void InGameObject_OnCreate(InGameObject inGameObject)
        {
            if (!_enabled) return;
            var baseTypes = inGameObject.GetBaseTypes().ToArray();
            if (!baseTypes.Contains("BaseObject")) return;
            var baseObj = inGameObject.Get<BaseGameObject>();
            if (_onlyLocal && baseObj != null && baseObj.Owner != LocalPlayer.Instance) return;
            Console.WriteLine("New Object:");
            Console.WriteLine("Name: " + inGameObject.ObjectName);
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
        }

        private static void SpellDetector_OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
        {
            if (!_enabled) return;
            if (_onlyLocal && args.Caster.Name != LocalPlayer.Instance.Name) return;

            var absys = args.Caster.AbilitySystem;
            Console.WriteLine("New Cast:");
            Console.WriteLine("Owner: " + args.Caster.CharName);
            Console.WriteLine("Name: " + args.Caster.AbilitySystem.CastingAbilityName);
            Console.WriteLine("Id: " + absys.CastingAbilityId);
            Console.WriteLine("Index: " + args.AbilityIndex);
            Console.WriteLine("----"); 
        }
    }
}