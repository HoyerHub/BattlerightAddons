using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using UnityEngine;

namespace Hoyer.Champions.ShenRao
{
    public static class MenuHandler
    {
        public static Menu HoyerMainMenu;
        public static Menu ShenRaoMenu;
        public static Menu SkillMenu;
        
        public static bool UseCursor;
        public static bool AimUserInput;
        public static bool DrawDebugText;
        public static bool InterruptSpells;

        private static MenuCheckBox _useCursor;
        private static MenuCheckBox _aimUserInput;
        private static MenuCheckBox _enabledBox;
        private static MenuCheckBox _comboToggle;
        private static MenuCheckBox _interruptSpells;
        private static MenuKeybind _comboKey;

        private static Dictionary<string, bool> SkillCheckBoxes;

        public static void Init()
        {
            SkillCheckBoxes = new Dictionary<string, bool>();

            HoyerMainMenu = MainMenu.GetMenu("Hoyer.MainMenu");
            ShenRaoMenu = HoyerMainMenu.Add(new Menu("HoyerShenRao", "Shen Rawr", true));

            ShenRaoMenu.Add(new MenuLabel("ShenRao"));
            _enabledBox = new MenuCheckBox("ShenRao_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Main.Enabled = args.NewValue; };
            ShenRaoMenu.Add(_enabledBox);

            ShenRaoMenu.AddSeparator();

            _comboKey = new MenuKeybind("ShenRao_combokey", "Combo key", KeyCode.V);
            _comboKey.OnValueChange += delegate(ChangedValueArgs<bool> args) { Main.SetMode(args.NewValue); };
            ShenRaoMenu.Add(_comboKey);

            _comboToggle = new MenuCheckBox("ShenRao_combotoggle", "Should Combo key be a toggle", false);
            _comboToggle.OnValueChange += delegate(ChangedValueArgs<bool> args) { _comboKey.IsToggle = args.NewValue; };
            ShenRaoMenu.Add(_comboToggle);

            _aimUserInput = new MenuCheckBox("ShenRao_aimuserinput", "Apply aim logic when combo isn't active");
            _aimUserInput.OnValueChange += delegate(ChangedValueArgs<bool> args) { AimUserInput = args.NewValue; };
            ShenRaoMenu.Add(_aimUserInput);

            _useCursor = new MenuCheckBox("ShenRao_usecursor", "Use cursor pos for target selection");
            _useCursor.OnValueChange += delegate(ChangedValueArgs<bool> args) { UseCursor = args.NewValue; };
            ShenRaoMenu.Add(_useCursor);

            _interruptSpells = new MenuCheckBox("ShenRao_interruptspells", "Interrupt spellcasts if aim logic is active and no valid targets");
            _interruptSpells.OnValueChange += delegate(ChangedValueArgs<bool> args)
            {
                InterruptSpells = args.NewValue;
            };
            ShenRaoMenu.Add(_interruptSpells);
            

            InitSkillMenu();
            FirstRun();

            Base.Main.DelayAction(delegate
            {
                var drawText = HoyerMainMenu.Get<Menu>("Hoyer.Debug").Add(new MenuCheckBox("ShenRao_drawdebug", "Draw ShenRao debug text"));
                drawText.OnValueChange += delegate(ChangedValueArgs<bool> args) { DrawDebugText = args.NewValue; };
                DrawDebugText = drawText.CurrentValue;
            }, 0.8f);
        }

        public static void Unload()
        {
            SkillCheckBoxes = null;
        }

        private static void InitSkillMenu()
        {
            SkillMenu = ShenRaoMenu.Add(new Menu("HoyerShenRao.Skills", "Skills", true));
            AddSkillCheckbox("combo_a1", "Use M1 in combo");
            AddSkillCheckbox("combo_a2", "Use M2 in combo");
            AddSkillCheckbox("safe_a4", "Use Q to push enemies away");
            AddSkillCheckbox("combo_a5", "Use E in combo");
            AddSkillCheckbox("heal_a6", "Use R if life is low");
        }

        public static bool SkillBool(string name)
        {
            return SkillCheckBoxes[name];
        }

        public static bool UseSkill(AbilitySlot slot)
        {
            switch (slot)
            {
                case AbilitySlot.Ability1:
                    return SkillCheckBoxes["combo_a1"];
                case AbilitySlot.Ability2:
                    return SkillCheckBoxes["combo_a2"];
                case AbilitySlot.Ability4:
                    return SkillCheckBoxes["safe_a4"];
                case AbilitySlot.Ability5:
                    return SkillCheckBoxes["combo_a5"];
                case AbilitySlot.Ability6:
                    return SkillCheckBoxes["heal_a6"];
            }

            return false;
        }

        private static void AddSkillCheckbox(string name, string displayname, bool defaultVal = true)
        {
            var skill = SkillMenu.Add(new MenuCheckBox(name, displayname, defaultVal));
            SkillCheckBoxes.Add(name, skill.CurrentValue);
            skill.OnValueChange += delegate(ChangedValueArgs<bool> args) { SkillCheckBoxes[skill.Name] = args.NewValue; };
        }

        private static void FirstRun()
        {
            Main.Enabled = _enabledBox.CurrentValue;
            Main.SetMode(false);
            _comboKey.IsToggle = _comboToggle.CurrentValue;
            UseCursor = _useCursor;
            AimUserInput = _aimUserInput;
            InterruptSpells = _interruptSpells;
    }

        public static void Update()
        {
            if (ShenRaoMenu == null)
            {
                Console.WriteLine("[HoyerShenRao/MenuHandler] Can't find menu, if this message is getting spammed, try F5 and please report this to Hoyer :(");
                return;
            }

            if (!Game.IsInGame)
            {
                if (ShenRaoMenu.Hidden) ShenRaoMenu.Hidden = false;
                return;
            }

            if ((LocalPlayer.Instance.CharName != "Shen Rao" && !string.IsNullOrEmpty(LocalPlayer.Instance.CharName)) && !ShenRaoMenu.Hidden)
            {
                ShenRaoMenu.Hidden = true;
                return;
            }

            if ((LocalPlayer.Instance.CharName != "Shen Rao" && !string.IsNullOrEmpty(LocalPlayer.Instance.CharName)) && ShenRaoMenu.Hidden) ShenRaoMenu.Hidden = false;
        }
    }
}