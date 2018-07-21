using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Sandbox;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Champions.Alysia.Modes;
using Hoyer.Common.Local;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.Alysia
{
    public class Alysia : IAddon
    {
        public static bool Enabled;
        private static IMode _mode;

        public void OnInit()
        {
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
            Skills.Initialize += SpellInit;
            Game.OnUpdate += OnUpdate;
        }

        private void MenuInit()
        {
            Console.WriteLine("init menu");
            var mainMenu = MainMenu.GetMenu("HoyerMain");
            //var characterMenu = MainMenu.AddMenu("HoyerAlysia", "Alysia");

            mainMenu.Add(new MenuLabel("Alysia"));
            var enabledCheckBox = new MenuCheckBox("alysia_enabled", "Enabled");
            enabledCheckBox.OnValueChange += delegate (ChangedValueArgs<bool> args) { /*characterMenu.Hidden = !args.NewValue;*/ Enabled = args.NewValue; };
            mainMenu.Add(enabledCheckBox);

            var slider = new MenuIntSlider("alysia_mode", "Mode: Aim logic", 0, 1);
            slider.OnValueChange += delegate (ChangedValueArgs<int> args)
            {
                if (args.NewValue == 0)
                    slider.DisplayName = "Mode: Aim logic";
                else if (args.NewValue == 1)
                    slider.DisplayName = "Mode: Aim and cast spells";
                /*else if (args.NewValue == 2)
                    slider.DisplayName = "Mode: Full auto????";*/

                SetMode(args.NewValue);
            };
            mainMenu.Add(slider);

            mainMenu.AddSeparator();
            Enabled = enabledCheckBox.CurrentValue;
            SetMode(slider.CurrentValue);
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName != "Alysia") return;
            Skills.AddFromDatabase(AbilitySlot.Ability1);
            Skills.AddFromDatabase(AbilitySlot.Ability2);
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability5, SkillType.Circle, 6.8f, 12, 1));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability7, SkillType.Line, 7.8f, 13.5f, 0.3f));
        }

        private void OnUpdate(EventArgs eventArgs)
        {
            if (!Enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || LocalPlayer.Instance.CharName != "Alysia")
            {
                LocalPlayer.EditAimPosition = false;
                return;
            }
            
            _mode.Update();
        }

        public static void SetMode(int index)
        {
            if (index == 0) _mode = new AimOnly();
            else if (index == 1)
                _mode = new AimAndCast();
        }

        public void OnUnload()
        {
        }
    }
}