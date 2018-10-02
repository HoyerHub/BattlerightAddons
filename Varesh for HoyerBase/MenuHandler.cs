using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using UnityEngine;

namespace Hoyer.Champions.Varesh
{
    public static class MenuHandler
    {
        public static Menu HoyerMainMenu;
        public static Menu VareshMenu;
        public static Menu SkillMenu;
        
        public static bool UseCursor;
        public static bool AimUserInput;
        public static bool DrawDebugText;
        public static bool InterruptSpells;
        public static bool NeverInterruptE;

        private static MenuCheckBox _useCursor;
        private static MenuCheckBox _aimUserInput;
        private static MenuCheckBox _enabledBox;
        private static MenuCheckBox _comboToggle;
        private static MenuCheckBox _interruptSpells;
        private static MenuCheckBox _neverInterruptE;
        private static MenuKeybind _comboKey;

        private static readonly Dictionary<string, bool> SkillCheckBoxes = new Dictionary<string, bool>();

        public static void Init()
        {
            HoyerMainMenu = MainMenu.GetMenu("Hoyer.MainMenu");
            VareshMenu = HoyerMainMenu.Add(new Menu("HoyerVaresh", "Varesh", true));

            VareshMenu.Add(new MenuLabel("Varesh"));
            _enabledBox = new MenuCheckBox("Varesh_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Main.Enabled = args.NewValue; };
            VareshMenu.Add(_enabledBox);

            VareshMenu.AddSeparator();

            _comboKey = new MenuKeybind("Varesh_combokey", "Combo key", KeyCode.V);
            _comboKey.OnValueChange += delegate(ChangedValueArgs<bool> args) { Main.SetMode(args.NewValue); };
            VareshMenu.Add(_comboKey);

            _comboToggle = new MenuCheckBox("Varesh_combotoggle", "Should Combo key be a toggle", false);
            _comboToggle.OnValueChange += delegate(ChangedValueArgs<bool> args) { _comboKey.IsToggle = args.NewValue; };
            VareshMenu.Add(_comboToggle);

            _aimUserInput = new MenuCheckBox("Varesh_aimuserinput", "Apply aim logic when combo isn't active");
            _aimUserInput.OnValueChange += delegate(ChangedValueArgs<bool> args) { AimUserInput = args.NewValue; };
            VareshMenu.Add(_aimUserInput);

            _useCursor = new MenuCheckBox("Varesh_usecursor", "Use cursor pos for target selection");
            _useCursor.OnValueChange += delegate(ChangedValueArgs<bool> args) { UseCursor = args.NewValue; };
            VareshMenu.Add(_useCursor);

            _interruptSpells = new MenuCheckBox("Varesh_interruptspells", "Interrupt spellcasts if aim logic is active and no valid targets");
            _interruptSpells.OnValueChange += delegate(ChangedValueArgs<bool> args)
            {
                InterruptSpells = args.NewValue;
                _neverInterruptE.Hidden = !args.NewValue;
            };
            VareshMenu.Add(_interruptSpells);

            _neverInterruptE = new MenuCheckBox("Varesh_neverinterrupte", "Never interrupt E");
            _neverInterruptE.OnValueChange += delegate (ChangedValueArgs<bool> args) { NeverInterruptE = args.NewValue; };
            VareshMenu.Add(_neverInterruptE);

            InitSkillMenu();
            FirstRun();

            Base.Main.DelayAction(delegate
            {
                var drawText = HoyerMainMenu.Get<Menu>("Hoyer.Debug").Add(new MenuCheckBox("Varesh_drawdebug", "Draw Varesh debug text"));
                drawText.OnValueChange += delegate(ChangedValueArgs<bool> args) { DrawDebugText = args.NewValue; };
                DrawDebugText = drawText.CurrentValue;
            }, 0.8f);
        }

        public static void Unload()
        {
            SkillCheckBoxes.Clear();
        }

        private static void InitSkillMenu()
        {
            SkillMenu = VareshMenu.Add(new Menu("HoyerVaresh.Skills", "Skills", true));
            AddSkillCheckbox("combo_a1", "Use M1 in combo");
            AddSkillCheckbox("combo_a2", "Use M2 in combo");
            AddSkillCheckbox("close_a3", "Use Space on self if in melee range");
            AddSkillCheckbox("combo_a5", "Use E in combo");
            AddSkillCheckbox("save_a6", "Save energy for R in combo");
            AddSkillCheckbox("combo_a7", "Use F in combo");
            AddSkillCheckbox("combo_ex1", "Use EX1 in combo");
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
                case AbilitySlot.Ability5:
                    return SkillCheckBoxes["combo_a5"];
                case AbilitySlot.EXAbility1:
                    return SkillCheckBoxes["combo_ex1"];
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
            NeverInterruptE = _neverInterruptE;
    }

        public static void Update()
        {
            if (VareshMenu == null)
            {
                Console.WriteLine("[HoyerVaresh/MenuHandler] Can't find menu, please report this to Hoyer :(");
                return;
            }

            if (!Game.IsInGame)
            {
                if (VareshMenu.Hidden) VareshMenu.Hidden = false;
                return;
            }

            if (LocalPlayer.Instance.ChampionEnum != Champion.Varesh && !VareshMenu.Hidden)
            {
                VareshMenu.Hidden = true;
                return;
            }

            if (LocalPlayer.Instance.ChampionEnum == Champion.Varesh && VareshMenu.Hidden) VareshMenu.Hidden = false;
        }
    }
}