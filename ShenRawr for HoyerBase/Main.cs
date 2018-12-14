using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Sandbox;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.Events;
using BattleRight.SDK.EventsArgs;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Extensions;
using Hoyer.Base.Menus;
using Hoyer.Champions.ShenRao.Systems;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.ShenRao
{
    public class Main : IAddon
    {
        public static bool Enabled;

        internal static string DebugOutput = "";
        internal static SkillBase AscendedM1;
        internal static AbilityInfo AM1Info;
        
        private static bool _combo;
        private static readonly string UltBuff = "DragonStormBuff";


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
            Console.WriteLine("Unload ShenRao Started");
            MenuHandler.Unload();
            MenuEvents.Initialize -= MenuHandler.Init;
            MenuEvents.Update -= MenuHandler.Update;
            ActiveSkills.Initialize -= SpellInit;
            SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
            Game.OnUpdate -= OnUpdate;
            Game.OnPreUpdate -= Game_OnDraw;
            Console.WriteLine("Unload ShenRao Ended");
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName == "Shen Rao" || string.IsNullOrEmpty(LocalPlayer.Instance.CharName))
            {
                ActiveSkills.AddFromDatabase();
                AM1Info = AbilityDatabase.Get("LightningBolt");
                AscendedM1 = new SkillBase(AbilitySlot.Ability1, SkillType.Circle, AM1Info.Range, AM1Info.Speed, AM1Info.Radius, AM1Info.FixedDelay);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || (LocalPlayer.Instance.CharName != "Shen Rao" && !string.IsNullOrEmpty(LocalPlayer.Instance.CharName))) return;

            if (MenuHandler.UseSkill(AbilitySlot.Ability6) && AbilitySlot.Ability6.IsReady() &&
                LocalPlayer.Instance.Energized.Energy >= 25 && LocalPlayer.Instance.Living.MaxRecoveryHealth - LocalPlayer.Instance.Living.Health >= 22)
            {
                LocalPlayer.PressAbility(AbilitySlot.Ability6, true);
                return;
            }

            if (LocalPlayer.Instance.AbilitySystem.IsCasting && !LocalPlayer.Instance.AbilitySystem.IsPostCasting)
            {
                if (_combo || MenuHandler.AimUserInput) Aiming.GetTargetAndAim();
            }
            else if (_combo && HasUltBuff())
            {
                //ult logic?
            }
            else if (_combo && (!LocalPlayer.Instance.AbilitySystem.IsCasting || LocalPlayer.Instance.AbilitySystem.IsPostCasting))
            {
                Casting.CastLogic();
            }
        }

        private bool HasUltBuff()
        {
            return LocalPlayer.Instance.HasBuff(UltBuff);
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

        private void SpellDetector_OnSpellStopCast(SpellStopArgs args)
        {
            if (Game.IsInGame && (LocalPlayer.Instance.CharName == "Shen Rao" || string.IsNullOrEmpty(LocalPlayer.Instance.CharName))) LocalPlayer.EditAimPosition = false;
        }
    }
}