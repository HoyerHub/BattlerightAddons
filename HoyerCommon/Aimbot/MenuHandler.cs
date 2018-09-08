using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using UnityEngine;

namespace Hoyer.Common.Aimbot
{
    public static class MenuHandler
    {
        public static Menu AimbotMenu;
        public static Menu Skills;

        public static bool Enabled;
        public static bool Interrupt;

        private static readonly Dictionary<string, Dictionary<AbilitySlot, MenuSpell>> SkillsMenuDict = new Dictionary<string, Dictionary<AbilitySlot, MenuSpell>>();
        
        public static bool Get(AbilitySlot slot)
        {
            if (!SkillsMenuDict.ContainsKey(LocalPlayer.Instance.CharName)) return false;
            if (!SkillsMenuDict[LocalPlayer.Instance.CharName].ContainsKey(slot)) return false;
            return SkillsMenuDict[LocalPlayer.Instance.CharName][slot].Enabled;
        }

        public static void Unload()
        {
            Game.OnMatchStart -= Game_OnMatchStart;
            Game.OnMatchEnd -= Game_OnMatchEnd;
            SkillsMenuDict.Clear();
        }

        public static void Setup()
        {
            Main.DelayAction(delegate
            {
                AimbotMenu = MenuEvents.HoyerMenu.Add(new Menu("Hoyer.Aimbot", "Aimbot", true));
                var enabled = AimbotMenu.Add(new MenuKeybind("aimbot_enabled", "Enabled", KeyCode.V));
                enabled.OnValueChange += args => Enabled = args.NewValue;
                var isToggle = AimbotMenu.Add(new MenuCheckBox("aimbot_shouldtoggle", "Enabled is toggle"));
                isToggle.OnValueChange += args => enabled.IsToggle = args.NewValue;
                var interrupt = AimbotMenu.Add(new MenuCheckBox("aimbot_interrupt", "Interrupt Abilities if no valid target"));
                interrupt.OnValueChange += args => Interrupt = args.NewValue;

                Skills = AimbotMenu.Add(new Menu("Aimbot.Skills", "Skills", true));
                try
                {
                    AddSkillEntries();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                enabled.IsToggle = isToggle;
                Interrupt = interrupt;
                Enabled = enabled;
                Game.OnMatchStart += Game_OnMatchStart;
                Game.OnMatchEnd += Game_OnMatchEnd;
            }, 0.08f);
        }

        private static void Game_OnMatchEnd(System.EventArgs args)
        {
            foreach (var dict in SkillsMenuDict)
            {
                foreach (var spell in dict.Value)
                {
                    spell.Value.Show();
                }
            }
        }

        private static void Game_OnMatchStart(System.EventArgs args)
        {
            foreach (var dict in SkillsMenuDict)
            {
                if (dict.Key == LocalPlayer.Instance.CharName)
                {
                    foreach (var spell in dict.Value)
                    {
                        spell.Value.Show();
                    }
                }
                else
                {
                    foreach (var spell in dict.Value)
                    {
                        spell.Value.Hide();
                    }
                }
            }
        }

        private static void AddSkillEntries()
        {
            var sorted = new SortedDictionary<string, List<AbilityInfo>>();
            foreach (var ability in AbilityDatabase.Abilities.Where(a => a.Danger > 0 && a.ObjectName != ""))
            {
                var champion = ability.Champion;
                if (sorted.ContainsKey(champion))
                {
                    if (sorted[champion].All(a => a.AbilitySlot != ability.AbilitySlot)) sorted[champion].Add(ability);
                }
                else
                {
                    sorted.Add(champion, new List<AbilityInfo> { ability });
                }
            }

            foreach (var champion in sorted)
            {
                var champ = champion.Key;
                var dict = new Dictionary<AbilitySlot, MenuSpell>();
                foreach (var abilityInfo in champion.Value)
                {
                    dict.Add(abilityInfo.AbilitySlot, new MenuSpell(abilityInfo));
                }
                SkillsMenuDict.Add(champ, dict);
            }
        }

        private class MenuSpell
        {
            public bool Enabled;
            private readonly List<MenuItem> _menuItems = new List<MenuItem>();

            public MenuSpell(AbilityInfo data)
            {
                Create(data);
                _menuItems.Add(Skills.AddSeparator(10));
            }

            private void Create(AbilityInfo data)
            {
                var abilityKey = data.AbilitySlot.ToFriendlyString();
                var champ = data.Champion;
                var enabledCheck = new MenuCheckBox("enabled_" + data.Champion + abilityKey,
                    "Handle aiming for " + champ + "'s " + abilityKey + " (" + data.AbilityType.ToFriendlyString() + ")");
                enabledCheck.OnValueChange += args => Enabled = args.NewValue;
                _menuItems.Add(Skills.Add(enabledCheck));
                Enabled = enabledCheck.CurrentValue;
            }

            public void Hide()
            {
                foreach (var menuItem in _menuItems)
                {
                    menuItem.Hidden = true;
                }
            }

            public void Show()
            {
                foreach (var menuItem in _menuItems)
                {
                    menuItem.Hidden = false;
                }
            }
        }
    }
}