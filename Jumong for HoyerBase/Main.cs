using System;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Sandbox;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.Events;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Local;
using Hoyer.Champions.Jumong.Modes;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.Jumong
{
    public class Main : IAddon
    {
        public static bool Enabled;
        public static bool AimUserInput;
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

        private void Game_OnDraw(EventArgs args)
        {
            if (MenuHandler.DrawDebugText)
                Drawing.DrawString(new Vector2(Screen.width / 2f, 200), DebugOutput, Color.green, ViewSpace.ScreenSpacePixels);
        }

        private void SpellDetector_OnSpellStopCast(BattleRight.SDK.EventsArgs.SpellStopArgs args)
        {
            if (Game.IsInGame && LocalPlayer.Instance.CharName == "Jumong") LocalPlayer.EditAimPosition = false;
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName != "Jumong") return;
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability1, SkillType.Line, 7.9f, 17, 0.3f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.Ability1));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability2, SkillType.Line, 10.25f, 26.5f, 0.3f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.Ability2));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability3, SkillType.Line, 7.8f, 13.5f, 0.3f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.Ability3));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability4, SkillType.Circle, 10, 0, 2, 0.5f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.Ability4));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability5, SkillType.Circle, 6.8f, 12, 1));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.Ability5));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability7, SkillType.Line, 7.8f, 13.5f, 0.3f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.Ability7));
            Skills.Active.Add(new SkillBase(AbilitySlot.EXAbility1, SkillType.Line, 10.5f, 24.5f, 0.3f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.EXAbility1));
            Skills.Active.Add(new SkillBase(AbilitySlot.EXAbility2, SkillType.Line, 8.8f, 26.5f, 0.3f));
            Skills.ActiveInfos.AddRange(AbilityDatabase.Get("Jumong", AbilitySlot.EXAbility2));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || LocalPlayer.Instance.CharName != "Jumong")
            {
                DebugOutput = "";
                return;
            }
            if (LocalPlayer.Instance.AbilitySystem.IsCasting && !LocalPlayer.Instance.AbilitySystem.IsPostCasting)
            {
                if (_combo || MenuHandler.AimUserInput) Aiming.GetTargetAndAim();
            }
            else if (_combo && (!LocalPlayer.Instance.AbilitySystem.IsCasting || LocalPlayer.Instance.AbilitySystem.IsPostCasting))
            {
                Casting.CastLogic();
            }
        }

        public static void SetMode(bool combo)
        {
            _combo = combo;
        }

        public void OnUnload()
        {
            Console.WriteLine("Unload Jumong Started");
            MenuHandler.Unload();
            MenuEvents.Initialize -= MenuHandler.Init;
            MenuEvents.Update -= MenuHandler.Update;
            Skills.Initialize -= SpellInit;
            SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
            Game.OnUpdate -= OnUpdate;
            Game.OnPreUpdate -= Game_OnDraw;
            Console.WriteLine("Unload Jumong Ended");
        }
    }
}