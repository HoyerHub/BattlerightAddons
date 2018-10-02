using System;
using BattleRight.Core;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Base.Debugging;
using Hoyer.Base.Prediction;
using Hoyer.Base.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hoyer.Base.Local
{
    public static class MenuEvents
    {
        public static event Action Initialize = delegate { };
        public static event Action Update = delegate { };

        public static Menu HoyerMenu;
        public static Menu BaseMenu;
        public static Menu PredMenu;
        public static Menu HumanMenu;
        public static Menu TrackerMenu;

        public static void Setup()
        {
            Main.Init += Init;
        }

        public static void Unload()
        {
            Main.Init -= Init;
            Game.OnMatchStart -= MatchUpdate;
            Game.OnMatchEnd -= MatchUpdate;
        }

        public static void Init()
        {
            HoyerMenu = MainMenu.AddMenu("Hoyer.MainMenu", "Hoyer");
            BaseMenu = HoyerMenu.Add(new Menu("Hoyer.Base", "Base", true));
            BaseMenu.AddLabel("Common Utils");

            var hideNames = new MenuCheckBox("hide_names", "Hide all names (Video Mode)", false);
            hideNames.OnValueChange += delegate(ChangedValueArgs<bool> args)
            {
                HideNames.Active = args.NewValue;
                if (!args.NewValue)
                {
                    foreach (var label in Object.FindObjectsOfType(typeof(TextMeshProUGUI)))
                    {
                        var component = ((MonoBehaviour) label).GetComponent<TextMeshProUGUI>();
                        if (label.name == "Name" || label.name == "NameText") component.enabled = true;
                    }
                }
            };
            BaseMenu.Add(hideNames);

            var showStealth = new MenuCheckBox("show_stealth", "Show predicted stealth positions");
            showStealth.OnValueChange += delegate(ChangedValueArgs<bool> args) { StealthPrediction.DrawStealthed = args.NewValue; };
            BaseMenu.Add(showStealth);

            BaseMenu.AddSeparator();

            InitPredMenu();

            InitHumanizerMenu();

            LoadValues();
            
            Initialize.Invoke();
            Main.DelayAction(delegate
            {
                Update.Invoke();
            }, 1);
            Game.OnMatchStart += MatchUpdate;
            Game.OnMatchEnd += MatchUpdate;
        }

        private static void InitPredMenu()
        {
            PredMenu = BaseMenu.Add(new Menu("HoyerPred", "Prediction", true));
            PredMenu.AddLabel("Common Prediction Settings");

            var useStealthPred = PredMenu.Add(new MenuCheckBox("use_stealth_pred", "Use Stealth Pred to aim", false));
            useStealthPred.OnValueChange += delegate(ChangedValueArgs<bool> args) { StealthPrediction.ShouldUse = args.NewValue; };

            var castRangeSlider = PredMenu.Add(new MenuSlider("pred_castrange", "Start casting range modifier", 0.92f, 1.1f, 0.9f));
            castRangeSlider.OnValueChange += delegate(ChangedValueArgs<float> args) { Prediction.Prediction.CastingRangeModifier = args.NewValue; };

            var cancelRangeSlider = PredMenu.Add(new MenuSlider("pred_cancelrange", "Out of range modifier", 1, 1.1f, 0.9f));
            cancelRangeSlider.OnValueChange += delegate(ChangedValueArgs<float> args) { Prediction.Prediction.CancelRangeModifier = args.NewValue; };

            var predModes = new[] {"Basic Aimlogic (fastest)", "SDK Prediction", "DaPip's TestPred"};
            var predMode = PredMenu.Add(new MenuComboBox("pred_mode", "Prediction Mode", 1, predModes));
            predMode.OnValueChange += delegate(ChangedValueArgs<int> args)
            {
                Prediction.Prediction.Mode = args.NewValue;
                Console.WriteLine("[HoyerCommon/MenuEvents] Prediction changed to " + predModes[args.NewValue]);
            };

            var wallCheck = PredMenu.Add(new MenuComboBox("pred_wallcheck", "Wall Check", 1,
                new[] {"No Wallcheck", "Only HighBlock", "LowBlock and HighBlock"}));
            wallCheck.OnValueChange += delegate(ChangedValueArgs<int> args) { Prediction.Prediction.WallCheckMode = args.NewValue; };
        }

        private static void InitHumanizerMenu()
        {
            HumanMenu = BaseMenu.Add(new Menu("HoyerHuman", "Humanizer", true));
            HumanMenu.AddLabel("Common Humanizer Settings");

            var useMaxCursorDist = HumanMenu.Add(new MenuCheckBox("usemaxcursordistance", "Use Max Cursor distance"));
            useMaxCursorDist.OnValueChange += delegate(ChangedValueArgs<bool> args) { TargetSelection.UseMaxCursorDist = args.NewValue; };

            var maxCursorDist = HumanMenu.Add(new MenuSlider("maxcursordistance", "Max Cursor move distance", 3, 6, 1));
            maxCursorDist.OnValueChange += delegate(ChangedValueArgs<float> args) { TargetSelection.MaxCursorDist = args.NewValue; };

            var drawMaxCursorDist = HumanMenu.Add(new MenuCheckBox("drawmaxcursordistance", "Draw Max Cursor distance"));
            drawMaxCursorDist.OnValueChange += delegate(ChangedValueArgs<bool> args) { DebugHelper.DrawMaxCursorDist = args.NewValue; };

            var useClosestPos = HumanMenu.Add(new MenuCheckBox("useclosestpos", "Use closest position on line"));
            useClosestPos.OnValueChange += delegate(ChangedValueArgs<bool> args) { Prediction.Prediction.UseClosestPointOnLine = args.NewValue; };
        }

        private static void MatchUpdate(EventArgs args)
        {
            Update.Invoke();
        }

        private static void LoadValues()
        {
            HideNames.Active = BaseMenu.Get<MenuCheckBox>("hide_names").CurrentValue;
            StealthPrediction.DrawStealthed = BaseMenu.Get<MenuCheckBox>("show_stealth").CurrentValue;
            StealthPrediction.ShouldUse = PredMenu.Get<MenuCheckBox>("use_stealth_pred").CurrentValue;
            Prediction.Prediction.Mode = PredMenu.Get<MenuComboBox>("pred_mode").CurrentValue;
            Prediction.Prediction.WallCheckMode = PredMenu.Get<MenuComboBox>("pred_wallcheck").CurrentValue;
            Prediction.Prediction.CastingRangeModifier = PredMenu.Get<MenuSlider>("pred_castrange").CurrentValue;
            Prediction.Prediction.CancelRangeModifier = PredMenu.Get<MenuSlider>("pred_cancelrange").CurrentValue;
            TargetSelection.UseMaxCursorDist = HumanMenu.Get<MenuCheckBox>("usemaxcursordistance").CurrentValue;
            TargetSelection.MaxCursorDist = HumanMenu.Get<MenuSlider>("maxcursordistance").CurrentValue;
            DebugHelper.DrawMaxCursorDist = HumanMenu.Get<MenuCheckBox>("drawmaxcursordistance").CurrentValue;
            Prediction.Prediction.UseClosestPointOnLine = HumanMenu.Get<MenuCheckBox>("useclosestpos").CurrentValue;
        }
    }
}