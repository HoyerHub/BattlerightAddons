using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Sandbox;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.Events;
using Hoyer.Champions.Varesh.Systems;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.Varesh
{
    public class Varesh : IAddon
    {
        public static bool Enabled;
        private static bool _combo;

        internal static string DebugOutput = "";

        public void OnInit()
        {
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
            Skills.Initialize += SpellInit;
            SpellDetector.OnSpellStopCast += SpellDetector_OnSpellStopCast;
            Game.OnUpdate += OnUpdate;
            Game.OnPreUpdate += Game_OnDraw;
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName != "Varesh") return;
            Skills.AddFromDatabase();
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || LocalPlayer.Instance.CharName != "Varesh")
            {
                return;
            }
            
            if (LocalPlayer.Instance.AbilitySystem.IsCasting && !LocalPlayer.Instance.AbilitySystem.IsPostCasting)
            {
                if (_combo || MenuHandler.AimUserInput) Aiming.GetTargetAndAim();
            }
            else if (_combo && HasUltBuff())
            {
                Aiming.AimUlt();
            }
            else if (_combo && (!LocalPlayer.Instance.AbilitySystem.IsCasting || LocalPlayer.Instance.AbilitySystem.IsPostCasting))
            {
                Casting.CastLogic();
            }
        }

        private bool HasUltBuff()
        {
            return LocalPlayer.Instance.HasBuff(new[] { "PowersCombinedFly1", "PowersCombinedFly2" });
        }

        public static void SetMode(bool combo)
        {
            _combo = combo;
        }

        private void Game_OnDraw(EventArgs args)
        {
            if (MenuHandler.DrawDebugText)
                Drawing.DrawString(new Vector2(Screen.width / 2f, 200), DebugOutput, Color.green, ViewSpace.ScreenSpacePixels);
        }

        private void SpellDetector_OnSpellStopCast(BattleRight.SDK.EventsArgs.SpellStopArgs args)
        {
            if (Game.IsInGame && LocalPlayer.Instance.CharName == "Varesh") LocalPlayer.EditAimPosition = false;
        }

        public static void BuffCheck(List<Character> validEnemies, out bool judgement, out bool corruption)
        {
            judgement = false;
            corruption = false;
            foreach (var enemy in validEnemies)
            {
                foreach (var buff in enemy.Buffs)
                {
                    if (buff.ObjectName == "HandOfJudgementBuff") judgement = true;
                    else if (buff.ObjectName == "HandOfCorruptionBuff") corruption = true;
                }
                if (judgement && corruption) break;
            }
        }

        public void OnUnload()
        {
            Console.WriteLine("Unload Varesh Started");
            MenuHandler.Unload();
            MenuEvents.Initialize -= MenuHandler.Init;
            MenuEvents.Update -= MenuHandler.Update;
            Skills.Initialize -= SpellInit;
            SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
            Game.OnUpdate -= OnUpdate;
            Game.OnPreUpdate -= Game_OnDraw;
            Console.WriteLine("Unload Varesh Ended");
        }
    }
}