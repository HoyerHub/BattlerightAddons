using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;

namespace Hoyer.Champions.Jumong
{
    public static class MenuHandler
    {
        public static Menu HoyerMainMenu;
        public static Menu JumongMenu;

        private static MenuIntSlider _modeSlider;
        private static MenuCheckBox _enabledBox;

        public static void Init()
        {
            HoyerMainMenu = MainMenu.GetMenu("HoyerMain");
            JumongMenu = HoyerMainMenu.Add(new Menu("HoyerJumong", "Jumong", true));

            JumongMenu.Add(new MenuLabel("Jumong"));
            _enabledBox = new MenuCheckBox("jumong_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate (ChangedValueArgs<bool> args) { Jumong.Enabled = args.NewValue; };
            JumongMenu.Add(_enabledBox);

            JumongMenu.AddSeparator();
            
            _modeSlider = new MenuIntSlider("jumong_mode", "", 0, 1);
            _modeSlider.OnValueChange += delegate (ChangedValueArgs<int> args)
            {
                SetModeSliderName(args.NewValue);
                Jumong.SetMode(args.NewValue);
            };
            JumongMenu.Add(_modeSlider);

            JumongMenu.Add(new MenuCheckBox("jumong_usecursor", "Use cursor pos for target selection"));

            FirstRun();
            Console.WriteLine("[HoyerJumong/MenuHandler] Jumong menu init");
        }

        private static void SetModeSliderName(int val)
        {
            if (val == 0)
                _modeSlider.DisplayName = "Mode: Handle Aiming";
            else if (val == 1)
                _modeSlider.DisplayName = "Mode: Handle Aiming and SpellCasting";
        }

        private static void FirstRun()
        {
            Jumong.Enabled = _enabledBox.CurrentValue;
            Jumong.SetMode(_modeSlider.CurrentValue);
            SetModeSliderName(_modeSlider.CurrentValue);
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