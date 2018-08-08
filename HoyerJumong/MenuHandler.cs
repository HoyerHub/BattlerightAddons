using System;
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

        public static MenuCheckBox AvoidStealthed;
        public static MenuCheckBox UseCursor;
        public static MenuCheckBox AimUserInput;

        private static MenuKeybind _comboKey;
        private static MenuCheckBox _enabledBox;
        private static MenuCheckBox _comboToggle;

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

            AvoidStealthed = new MenuCheckBox("jumong_ignorestealthed", "Ignore stealthed enemies");
            JumongMenu.Add(AvoidStealthed);

            FirstRun();
            Console.WriteLine("[HoyerJumong/MenuHandler] Jumong Menu Init");
        }

        private static void FirstRun()
        {
            Jumong.Enabled = _enabledBox.CurrentValue;
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