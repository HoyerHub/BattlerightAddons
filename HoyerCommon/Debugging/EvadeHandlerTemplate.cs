using System;
using System.Collections.Generic;
using System.Threading;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;

namespace Hoyer.Common.Debugging
{
    public static class EvadeHandler
    {
        public static EvadeSpell JadeSpace;
        public static EvadeSpell RuhKaanR;
        public static EvadeSpell BakkoEx1;

        public static void Setup()
        {
            JadeSpace = new EvadeSpell("JadeSpace");
            RuhKaanR = new EvadeSpell("Ruh KaanR");
            BakkoEx1 = new EvadeSpell("BakkoEX1");
        }

        public class EvadeSpell
        {
            public bool IsEvading;

            public EvadeSpell(string charAbility)
            {
                _abilityString = charAbility;
                DelayedSetup();
            }

            private readonly string _abilityString;

            private MenuCheckBox _activeCheckBox;
            private MenuComboBox _overrideComboBox;
            private int _overrideIndex;


            //Runs setup after two seconds to ensure all addons are loaded, and then stops the Timer after one use
            private void DelayedSetup()
            {
                Timer timer = null;
                timer = new Timer(obj =>
                    {
                        Setup();
                        timer.Dispose();
                    },
                    null, 2000, Timeout.Infinite);
            }

            private void Setup()
            {
                var hoyerMainMenu = MainMenu.GetMenu("Hoyer.MainMenu");
                if (hoyerMainMenu != null)
                {
                    var evadeMainMenu = hoyerMainMenu.Get<Menu>("Evade.MainMenu");
                    if (evadeMainMenu != null)
                    {
                        var evadeOverrideMenu = evadeMainMenu.Get<Menu>("Evade.OverrideMenu");
                        var evadeStatusMenu = evadeMainMenu.Get<Menu>("Evade.StatusMenu");

                        SetupOverrideMenu(evadeOverrideMenu);
                        SetupStatus(evadeStatusMenu);
                    }
                    else
                    {
                        //HoyerEvade wasn't found
                    }
                }
                else
                {
                    //HoyerCommon wasn't found
                }
            }

            //Creates an entry in the "Hoyer">"Evade">"External Logic" Menu
            private void SetupOverrideMenu(Menu evadeOverrideMenu)
            {
                _overrideComboBox = evadeOverrideMenu.Get<MenuComboBox>("override_" + _abilityString);
                _overrideComboBox.Hidden = false;
                _overrideComboBox.Elements = new List<string>(_overrideComboBox.Elements) { "Addon Name" }.ToArray();
                _overrideComboBox.CurrentValue = _overrideComboBox.Elements.Length - 1;
                _overrideIndex = _overrideComboBox.CurrentValue;
            }

            //Sets up the event for when evade is active on this spell
            private void SetupStatus(Menu evadeStatusMenu)
            {
                _activeCheckBox = evadeStatusMenu.Get<MenuCheckBox>("isActive_" + _abilityString);
                _activeCheckBox.OnValueChange += ActiveCheckBox_OnValueChange;
            }

            //Sets IsEvading to true if this handler is selected in the "External Logic" menu
            private void ActiveCheckBox_OnValueChange(ChangedValueArgs<bool> changedValueArgs)
            {
                if (_overrideComboBox.CurrentValue != _overrideIndex) return;

                IsEvading = changedValueArgs.NewValue;
                Console.WriteLine("[EvadeHandler] " + (IsEvading ? "Started evading" : "Finished evading"));
            }
        }
    }
}