using System;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Sandbox;
using BattleRight.SDK.Events;
using Hoyer.Common.Local;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.Varesh
{
    public class Varesh : IAddon
    {
        public static bool Enabled;
        public static bool AimUserInput;
        private static bool _combo;

        public void OnInit()
        {
            Console.WriteLine("Sorry boi, but Varesh isn't ready for use");
            if (1.ToString() == "1") return;
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
            Skills.Initialize += SpellInit;
            SpellDetector.OnSpellStopCast += SpellDetector_OnSpellStopCast;
            CommonEvents.Update += OnUpdate;
        }

        private void SpellDetector_OnSpellStopCast(BattleRight.SDK.EventsArgs.SpellStopArgs args)
        {
            if (Game.IsInGame && LocalPlayer.Instance.CharName == "Varesh") LocalPlayer.EditAimPosition = false;
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName != "Varesh") return;
            Skills.AddFromDatabase();
        }

        private void OnUpdate()
        {
            if (!Enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || LocalPlayer.Instance.CharName != "Varesh" || LocalPlayer.Instance.HasBuff("SpellBlock"))
            {
                return;
            }
            
            if (_combo && (!LocalPlayer.Instance.AbilitySystem.IsCasting || LocalPlayer.Instance.AbilitySystem.IsPostCasting))
            {

            }
            else if (LocalPlayer.Instance.AbilitySystem.IsCasting)
            {
            }
        }

        public static void SetMode(bool combo)
        {
            _combo = combo;
        }

        public void OnUnload()
        {
        }
    }
}