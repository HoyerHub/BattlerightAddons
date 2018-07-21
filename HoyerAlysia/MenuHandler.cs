using System;
using System.Linq;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;

namespace Hoyer.Champions.Alysia
{
    public static class MenuHandler
    {
        public static Menu HoyerMain;
        public static Menu AlysiaMain;

        private static MenuIntSlider _modeSlider;
        private static MenuCheckBox _enabledBox;

        public static void Init()
        {
            HoyerMain = MainMenu.GetMenu("HoyerMain");
            AlysiaMain = HoyerMain.Add(new Menu("HoyerAlysia", "Alysia", true));
            //AimMode = AlysiaMain.Add(new Menu("HoyerAlysiaAim", "Mode: Aim", true));
            //CastMode = AlysiaMain.Add(new Menu("HoyerAlysiaCast", "Mode: Cast and aim", true));

            AlysiaMain.Add(new MenuLabel("Alysia"));
            _enabledBox = new MenuCheckBox("alysia_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate (ChangedValueArgs<bool> args) { /*characterMenu.Hidden = !args.NewValue;*/ Alysia.Enabled = args.NewValue; };
            AlysiaMain.Add(_enabledBox);

            AlysiaMain.AddSeparator();
            
            _modeSlider = new MenuIntSlider("alysia_mode", "", 0, 1);
            _modeSlider.OnValueChange += delegate (ChangedValueArgs<int> args)
            {
                SetModeSliderName(args.NewValue);
                Alysia.SetMode(args.NewValue);
            };
            AlysiaMain.Add(_modeSlider);

            AlysiaMain.Add(new MenuCheckBox("alysia_usecursor", "Use cursor pos for target selection"));

            FirstRun();
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
            var menu = MainMenu.GetMenu("HoyerAlysia");
            if (menu == null || LocalPlayer.Instance == null)
            {
                return;
            }
            menu.Hidden = LocalPlayer.Instance.CharName != "Alysia";
        }
    }
}