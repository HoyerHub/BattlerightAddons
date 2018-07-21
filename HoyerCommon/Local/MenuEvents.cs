using System;
using BattleRight.Core;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Utilities;
using TMPro;
using UnityEngine;
using Component = Hoyer.Common.Utilities.Component;
using Object = UnityEngine.Object;

namespace Hoyer.Common.Local
{
    public static class MenuEvents
    {
        public static event Action Initialize = delegate { };
        public static event Action Update = delegate { };

        public static Menu HoyerMenu;

        static MenuEvents()
        {
            Component.Init += Init;
        }

        public static void Init()
        {
            HoyerMenu = MainMenu.AddMenu("HoyerMain", "Hoyer");
            HoyerMenu.AddLabel("Common Utils");

            var hideNames = new MenuCheckBox("hide_names", "Hide all names (Video Mode)");
            hideNames.OnValueChange += delegate (ChangedValueArgs<bool> args)
            {
                HideNames.Active = args.NewValue;
                if (!args.NewValue)
                {
                    foreach (var label in Object.FindObjectsOfType(typeof(TextMeshProUGUI)))
                    {
                        var component = ((MonoBehaviour)label).GetComponent<TextMeshProUGUI>();
                        if (label.name == "Name" || label.name == "NameText") component.enabled = true;
                    }
                }
            };
            HoyerMenu.Add(hideNames);
            
            var showStealth = new MenuCheckBox("show_stealth", "Show predicted stealth positions");
            showStealth.OnValueChange += delegate (ChangedValueArgs<bool> args) { StealthPrediction.DrawStealthed = args.NewValue; };
            HoyerMenu.Add(showStealth);

            HoyerMenu.AddSeparator();
            LoadValues();
            
            Initialize.Invoke();
            Update.Invoke();
            Game.OnMatchStart += Game_OnMatchStart;
        }

        private static void LoadValues()
        {
            HideNames.Active = HoyerMenu.Get<MenuCheckBox>("hide_names").CurrentValue;
            StealthPrediction.DrawStealthed = HoyerMenu.Get<MenuCheckBox>("show_stealth").CurrentValue;
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            Update.Invoke();
        }
    }
}