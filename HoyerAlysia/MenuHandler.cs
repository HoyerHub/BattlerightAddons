using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;

namespace Hoyer.Champions.Alysia
{
    public static class MenuHandler
    {
        public static Menu HoyerMenu;
        public static Menu AlysiaMenu;

        private static MenuIntSlider _modeSlider;
        private static MenuCheckBox _enabledBox;

        public static void Init()
        {
            HoyerMenu = MainMenu.GetMenu("HoyerMain");
            AlysiaMenu = HoyerMenu.Add(new Menu("HoyerAlysia", "Alysia", true));

            AlysiaMenu.Add(new MenuLabel("Alysia"));
            _enabledBox = new MenuCheckBox("alysia_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate (ChangedValueArgs<bool> args) { Alysia.Enabled = args.NewValue; };
            AlysiaMenu.Add(_enabledBox);

            AlysiaMenu.AddSeparator();
            
            _modeSlider = new MenuIntSlider("alysia_mode", "", 0, 1);
            _modeSlider.OnValueChange += delegate (ChangedValueArgs<int> args)
            {
                SetModeSliderName(args.NewValue);
                Alysia.SetMode(args.NewValue);
            };
            AlysiaMenu.Add(_modeSlider);

            AlysiaMenu.Add(new MenuCheckBox("alysia_usecursor", "Use cursor pos for target selection"));

            FirstRun();
            Console.WriteLine("[HoyerAlysia/MenuHandler] Alysia Menu Init");
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
            Alysia.Enabled = _enabledBox.CurrentValue;
            Alysia.SetMode(_modeSlider.CurrentValue);
            SetModeSliderName(_modeSlider.CurrentValue);
        }

        public static void Update()
        {
            if (AlysiaMenu == null)
            {
                Console.WriteLine("[HoyerAlysia/MenuHandler] Can't find menu, please report this to Hoyer :(");
                return;
            }
            if (!Game.IsInGame)
            {
                if (AlysiaMenu.Hidden)
                {
                    Console.WriteLine("[HoyerAlysia/MenuHandler] Showing Menu");
                    AlysiaMenu.Hidden = false;
                }
                return;
            }
            if (LocalPlayer.Instance.ChampionEnum != Champion.Alysia && !AlysiaMenu.Hidden)
            {
                Console.WriteLine("[HoyerAlysia/MenuHandler] Hiding Menu");
                AlysiaMenu.Hidden = true;
                return;
            }
            if (LocalPlayer.Instance.ChampionEnum == Champion.Alysia && AlysiaMenu.Hidden)
            {
                Console.WriteLine("[HoyerAlysia/MenuHandler] Showing Menu");
                AlysiaMenu.Hidden = false;
            }
        }
    }
}