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
using Hoyer.Champions.Jumong.Modes;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;

// ReSharper disable ArrangeAccessorOwnerBody

namespace Hoyer.Champions.Jumong
{
    public class Jumong : IAddon
    {
        private IMode _mode;
        private bool _enabled;

        public void OnInit()
        {
            MenuEvents.Initialize += MenuInit;
            MenuEvents.Update += MenuUpdate;
            Skills.Initialize += SpellInit;
            Game.OnUpdate += OnUpdate;
        }

        private void MenuUpdate()
        {
            var menu = MainMenu.GetMenu("HoyerJumong");
            if (menu == null || LocalPlayer.Instance == null)
            {
                return;
            }
            menu.Hidden = LocalPlayer.Instance.CharName != "Jumong";
            var menuItems = MainMenu.GetMenu("HoyerMain").Children.Where(o => o.DisplayName == "Jumong" || o.Name == "jumong_enabled");
            foreach (var menuItem in menuItems)
            {
                menuItem.Hidden = LocalPlayer.Instance.CharName != "Jumong";
            }
        }

        private void MenuInit()
        {
            Console.WriteLine("init menu");
            var mainMenu = MainMenu.GetMenu("HoyerMain");
            var characterMenu = MainMenu.AddMenu("HoyerJumong", "Jumong");

            mainMenu.Add(new MenuLabel("Jumong"));
            var enabledCheckBox = new MenuCheckBox("jumong_enabled", "Enabled");
            enabledCheckBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { characterMenu.Hidden = !args.NewValue; _enabled = args.NewValue;};
            mainMenu.Add(enabledCheckBox);
            mainMenu.AddSeparator();

            var slider = new MenuIntSlider("mode", "Mode: Aim logic", 0, 1);
            slider.OnValueChange += delegate(ChangedValueArgs<int> args)
            {
                if (args.NewValue == 0)
                    slider.DisplayName = "Mode: Aim logic";
                else if (args.NewValue == 1)
                    slider.DisplayName = "Mode: Aim and cast spells";
                /*else if (args.NewValue == 2)
                    slider.DisplayName = "Mode: Full auto????";*/

                SetMode(args.NewValue);
            };
            characterMenu.Add(slider);
            
            _enabled = enabledCheckBox.CurrentValue;
            SetMode(slider.CurrentValue);
        }

        private void SpellInit()
        {
            if (LocalPlayer.Instance.CharName != "Jumong") return;
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability1, SkillType.Line, 7.9f, 17, 0.3f));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability2, SkillType.Line, 10.5f, 26.5f, 0.3f));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability3, SkillType.Line, 7.8f, 13.5f, 0.3f));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability4, SkillType.Circle, 10, 0, 2, 0.5f));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability5, SkillType.Circle, 6.8f, 12, 1));
            Skills.Active.Add(new SkillBase(AbilitySlot.Ability7, SkillType.Line, 7.8f, 13.5f, 0.3f));
            Skills.Active.Add(new SkillBase(AbilitySlot.EXAbility1, SkillType.Line, 10.5f, 24.5f, 0.3f));
            Skills.Active.Add(new SkillBase(AbilitySlot.EXAbility2, SkillType.Line, 8.8f, 26.5f, 0.3f));
        }

        private void OnUpdate(EventArgs eventArgs)
        {
            if (!_enabled || !Game.IsInGame || Game.CurrentMatchState != MatchState.InRound || LocalPlayer.Instance.CharName != "Jumong")
            {
                LocalPlayer.EditAimPosition = false;
                return;
            }
            _mode.Update();
        }

        private void SetMode(int index)
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