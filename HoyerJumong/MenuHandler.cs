using System;
using System.Linq;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;

namespace Hoyer.Champions.Jumong
{
    public static class MenuHandler
    {
        public static Menu HoyerMain;
        public static Menu JumongMain;

        private static MenuIntSlider _modeSlider;
        private static MenuCheckBox _enabledBox;

        public static void Init()
        {
            HoyerMain = MainMenu.GetMenu("HoyerMain");
            JumongMain = HoyerMain.Add(new Menu("HoyerJumong", "Jumong", true));
            //AimMode = JumongMain.Add(new Menu("HoyerJumongAim", "Mode: Aim", true));
            //CastMode = JumongMain.Add(new Menu("HoyerJumongCast", "Mode: Cast and aim", true));

            JumongMain.Add(new MenuLabel("Jumong"));
            _enabledBox = new MenuCheckBox("jumong_enabled", "Enabled");
            _enabledBox.OnValueChange += delegate (ChangedValueArgs<bool> args) { /*characterMenu.Hidden = !args.NewValue;*/ Jumong.Enabled = args.NewValue; };
            JumongMain.Add(_enabledBox);

            JumongMain.AddSeparator();
            
            _modeSlider = new MenuIntSlider("jumong_mode", "", 0, 1);
            _modeSlider.OnValueChange += delegate (ChangedValueArgs<int> args)
            {
                SetModeSliderName(args.NewValue);
                Jumong.SetMode(args.NewValue);
            };
            JumongMain.Add(_modeSlider);

            JumongMain.Add(new MenuCheckBox("jumong_usecursor", "Use cursor pos for target selection"));

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
            Jumong.Enabled = _enabledBox.CurrentValue;
            Jumong.SetMode(_modeSlider.CurrentValue);
            SetModeSliderName(_modeSlider.CurrentValue);
        }

        public static void Update()
        {
            var menu = MainMenu.GetMenu("HoyerJumong");
            if (menu == null || LocalPlayer.Instance == null)
            {
                return;
            }
            menu.Hidden = LocalPlayer.Instance.CharName != "Jumong";
        }
    }
}