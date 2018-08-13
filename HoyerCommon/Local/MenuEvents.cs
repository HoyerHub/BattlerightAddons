using System;
using BattleRight.Core;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Debug;
using Hoyer.Common.Extensions;
using Hoyer.Common.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hoyer.Common.Local
{
    public static class MenuEvents
    {
        public static event Action Initialize = delegate { };
        public static event Action Update = delegate { };

        public static Menu HoyerMenu;
        public static Menu PredMenu;

        public static void Setup()
        {
            Main.Init += Init;
        }

        public static bool GetBool(string name)
        {
            return HoyerMenu.Get<MenuCheckBox>(name).CurrentValue;
        }

        public static void Init()
        {
            HoyerMenu = MainMenu.AddMenu("HoyerMain", "Hoyer");
            HoyerMenu.AddLabel("Common Utils");

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
            HoyerMenu.Add(hideNames);

            var showStealth = new MenuCheckBox("show_stealth", "Show predicted stealth positions");
            showStealth.OnValueChange += delegate(ChangedValueArgs<bool> args) { StealthPrediction.DrawStealthed = args.NewValue; };
            HoyerMenu.Add(showStealth);

            var drawProjectiles = new MenuCheckBox("draw_projectiles", "Draw Projectile Paths", false);
            drawProjectiles.OnValueChange += delegate(ChangedValueArgs<bool> args) { DrawProjectiles.Active = args.NewValue; };
            HoyerMenu.Add(drawProjectiles);

            HoyerMenu.AddSeparator();

            InitPredMenu();

            LoadValues();

            Console.WriteLine("[HoyerCommon/MenuEvents] Menu Init");
            Initialize.Invoke();
            Main.DelayAction(delegate
            {
                Console.WriteLine("[HoyerCommon/MenuEvents] Checking Menus");
                Update.Invoke();
            }, 1);
            Game.OnMatchStart += MatchUpdate;
            Game.OnMatchEnd += MatchUpdate;
        }

        private static void InitPredMenu()
        {
            PredMenu = HoyerMenu.Add(new Menu("HoyerPred", "Prediction", true));
            PredMenu.AddLabel("Common Prediction Settings");

            var useStealthPred = PredMenu.Add(new MenuCheckBox("use_stealth_pred", "Use Stealth Pred to aim (WIP)", false));
            useStealthPred.OnValueChange += delegate(ChangedValueArgs<bool> args) { StealthPrediction.ShouldUse = args.NewValue; };

            var castRangeSlider = PredMenu.Add(new MenuSlider("pred_castrange", "Start casting range modifier", 0.92f, 1.1f, 0.9f));
            castRangeSlider.OnValueChange += delegate(ChangedValueArgs<float> args) { Prediction.CastingRangeModifier = args.NewValue; };

            var cancelRangeSlider = PredMenu.Add(new MenuSlider("pred_cancelrange", "Out of range modifier", 1, 1.1f, 0.9f));
            cancelRangeSlider.OnValueChange += delegate(ChangedValueArgs<float> args) { Prediction.CancelRangeModifier = args.NewValue; };

            var predModes = new[] {"Basic Aimlogic (fastest)", "SDK Prediction", "DaPip's TestPred"};
            var predMode = PredMenu.Add(new MenuComboBox("pred_mode", "Prediction Mode", 1, predModes));
            predMode.OnValueChange += delegate(ChangedValueArgs<int> args)
            {
                Prediction.Mode = args.NewValue;
                Console.WriteLine("[HoyerCommon/MenuEvents] Prediction changed to " + predModes[args.NewValue]);
            };
        }


        private static void MatchUpdate(EventArgs args)
        {
            Console.WriteLine("[HoyerCommon/MenuEvents] Checking Menus");
            Update.Invoke();
        }

        private static void LoadValues()
        {
            HideNames.Active = HoyerMenu.Get<MenuCheckBox>("hide_names").CurrentValue;
            StealthPrediction.DrawStealthed = HoyerMenu.Get<MenuCheckBox>("show_stealth").CurrentValue;
            StealthPrediction.ShouldUse = PredMenu.Get<MenuCheckBox>("use_stealth_pred").CurrentValue;
            Prediction.Mode = PredMenu.Get<MenuComboBox>("pred_mode").CurrentValue;
            Prediction.CastingRangeModifier = PredMenu.Get<MenuSlider>("pred_castrange").CurrentValue;
            Prediction.CancelRangeModifier = PredMenu.Get<MenuSlider>("pred_cancelrange").CurrentValue;
        }
    }
}