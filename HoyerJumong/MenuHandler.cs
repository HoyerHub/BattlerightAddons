using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using UnityEngine;

namespace Hoyer.Champions.Jumong
{
    public static class MenuHandler
    {
        public static Menu HoyerMainMenu;
        public static Menu JumongMenu;
        public static Menu SkillMenu;

        public static MenuCheckBox AvoidStealthed;
        public static MenuCheckBox UseCursor;
        public static MenuCheckBox AimUserInput;

        private static MenuKeybind _comboKey;
        private static MenuCheckBox _enabledBox;
        private static MenuCheckBox _comboToggle;

        private static Dictionary<string, bool> SkillCheckBoxes = new Dictionary<string, bool>();

        public static void Init()
        {
            HoyerMainMenu = MainMenu.GetMenu("HoyerMain");
            JumongMenu = HoyerMainMenu.Add(new Menu("HoyerJumong", "Jumong", true));

            JumongMenu.Add(new MenuLabel("Jumong"));
            _enabledBox = new MenuCheckBox("jumong_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate (ChangedValueArgs<bool> args) { Jumong.Enabled = args.NewValue; };
            JumongMenu.Add(_enabledBox);

            JumongMenu.AddSeparator();

            _comboKey = new MenuKeybind("jumong_combokey", "Combo key", KeyCode.V);
            _comboKey.OnValueChange += delegate (ChangedValueArgs<bool> args) { Jumong.SetMode(args.NewValue); };
            JumongMenu.Add(_comboKey);

            _comboToggle = new MenuCheckBox("jumong_combotoggle", "Should Combo key be a toggle", false);
            _comboToggle.OnValueChange += delegate(ChangedValueArgs<bool> args) { _comboKey.IsToggle = args.NewValue; };
            JumongMenu.Add(_comboToggle);

            AimUserInput = new MenuCheckBox("jumong_aimuserinput", "Apply aim logic when combo isn't active");
            JumongMenu.Add(AimUserInput);

            UseCursor = new MenuCheckBox("jumong_usecursor", "Use cursor pos for target selection");
            JumongMenu.Add(UseCursor);

            AvoidStealthed = new MenuCheckBox("jumong_ignorestealthed", "Ignore stealthed enemies", false);
            JumongMenu.Add(AvoidStealthed);

            InitSkillMenu();

            FirstRun();
            Console.WriteLine("[HoyerJumong/MenuHandler] Jumong Menu Init");
        }

        private static void InitSkillMenu()
        {
            SkillMenu = JumongMenu.Add(new Menu("HoyerJumong.Skills", "Skills", true));
            AddSkillCheckbox("combo_a1", "Use M1 in combo");
            AddSkillCheckbox("combo_a2", "Use M2 in combo");
            AddSkillCheckbox("close_a3", "Use Space to avoid melees");
            AddSkillCheckbox("combo_a4", "Use Q in combo");
            AddSkillCheckbox("combo_a5", "Use E in combo");
            AddSkillCheckbox("save_a6", "Save energy for R in combo");
            AddSkillCheckbox("combo_ex1", "Use EX1 in combo");
            AddSkillCheckbox("combo_ex2", "Use EX2 in combo");
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
                    return SkillCheckBoxes["combo_a4"];
                case AbilitySlot.Ability5:
                    return SkillCheckBoxes["combo_a5"];
                case AbilitySlot.EXAbility1:
                    return SkillCheckBoxes["combo_ex1"];
                case AbilitySlot.EXAbility2:
                    return SkillCheckBoxes["combo_ex2"];
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
            Jumong.Enabled = _enabledBox.CurrentValue;
            Jumong.SetMode(false);
            _comboKey.IsToggle = _comboToggle.CurrentValue;
        }

        public static void Update()
        {
            if (JumongMenu == null)
            {
                Console.WriteLine("[HoyerJumong/MenuHandler] Can't find menu, please report this to Hoyer :(");
                return;
            }
            if (!Game.IsInGame)
            {
                if (JumongMenu.Hidden)
                {
                    Console.WriteLine("[HoyerJumong/MenuHandler] Showing Menu");
                    JumongMenu.Hidden = false;
                }
                return;
            }
            if (LocalPlayer.Instance.ChampionEnum != Champion.Jumong && !JumongMenu.Hidden)
            {
                Console.WriteLine("[HoyerJumong/MenuHandler] Hiding Menu");
                JumongMenu.Hidden = true;
                return;
            }
            if (LocalPlayer.Instance.ChampionEnum == Champion.Jumong && JumongMenu.Hidden)
            {
                Console.WriteLine("[HoyerJumong/MenuHandler] Showing Menu");
                JumongMenu.Hidden = false;
            }
        }
    }
}