using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.Events;
using BattleRight.SDK.EventsArgs;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Extensions;
using Hoyer.Base.Menus;
using Hoyer.Champions.Varesh.Systems;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.Varesh
{
    public class Program
    {
        private static readonly Main Addon = new Main();

        public static void Main(string[] args)
        {
            Base.Main.OnInit();
            Addon.OnInit();
        }
    }

    public class Main
    {
        public static bool Enabled;

        internal static string DebugOutput = "";

        private static bool _combo;
        private static readonly string[] UltBuffs = {"PowersCombinedFly1", "PowersCombinedFly2"};


        public void OnInit()
        {
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
            ActiveSkills.Initialize += SpellInit;
            SpellDetector.OnSpellStopCast += SpellDetector_OnSpellStopCast;
            Game.OnUpdate += OnUpdate;
            Game.OnPreUpdate += Game_OnDraw;
        }

        public void OnUnload()
        {
            Console.WriteLine("Unload Varesh Started");
            MenuHandler.Unload();
            MenuEvents.Initialize -= MenuHandler.Init;
            MenuEvents.Update -= MenuHandler.Update;
            ActiveSkills.Initialize -= SpellInit;
            SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
            Game.OnUpdate -= OnUpdate;
            Game.OnPreUpdate -= Game_OnDraw;
            Console.WriteLine("Unload Varesh Ended");
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName != "Varesh") return;
            ActiveSkills.AddFromDatabase();
        }

        private void OnUpdate()
        {
            Console.WriteLine("Update");
            if (!Enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || LocalPlayer.Instance.CharName != "Varesh") return;

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
            return LocalPlayer.Instance.HasBuff(UltBuffs);
        }

        public static void SetMode(bool combo)
        {
            _combo = combo;
        }

        private void Game_OnDraw()
        {
            if (MenuHandler.DrawDebugText)
                Drawing.DrawString(new Vector2(Screen.width / 2f, 200), DebugOutput, Color.green, ViewSpace.ScreenSpacePixels);
        }

        private void SpellDetector_OnSpellStopCast(SpellStopArgs args)
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
                    if (buff.ObjectName == "HandOfJudgementBuff")
                        judgement = true;
                    else if (buff.ObjectName == "HandOfCorruptionBuff")
                        corruption = true;
                if (judgement && corruption) break;
            }
        }
    }
}