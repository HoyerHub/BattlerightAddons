using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Extensions;

namespace Hoyer.Evade
{
    public static class MenuHandler
    {
        public static Menu HoyerMain;
        public static Menu EvadeMain;

        public static Menu EvadeSkillsMenu;
        public static Menu DodgeableSkillsMenu;

        public static Menu EvadeStatusMenu;
        public static Menu EvadeOverrideMenu;

        public static int JumpMode = 0;

        private static Dictionary<string, List<MenuItem>> EvadeSkillsMenuByChampion;
        private static Dictionary<string, List<MenuItem>> DodgeableSkillsMenuByChampion;

        private static Dictionary<string, List<MenuItem>> EvadeStatusMenuByChampion;
        private static Dictionary<string, List<MenuItem>> EvadeOverrideMenuByChampion;

        private static MenuCheckBox _enabledWalkBox;
        private static MenuCheckBox _enabledSkillsBox;
        private static MenuCheckBox _enabledDrawings;
        private static MenuComboBox _jumpMode;

        private static MenuCheckBox _activeSpell;

        public static void Init()
        {
            try
            {
                EvadeSkillsMenuByChampion = new Dictionary<string, List<MenuItem>>();
                DodgeableSkillsMenuByChampion = new Dictionary<string, List<MenuItem>>();
                EvadeStatusMenuByChampion = new Dictionary<string, List<MenuItem>>();
                EvadeOverrideMenuByChampion = new Dictionary<string, List<MenuItem>>();

                HoyerMain = MainMenu.GetMenu("Hoyer.MainMenu");
                EvadeMain = HoyerMain.Add(new Menu("Evade.MainMenu", "Evade", true));

                EvadeMain.Add(new MenuLabel("Evade"));
                _enabledSkillsBox = new MenuCheckBox("evade_skills", "Use skills to dodge dangerous skillshots");
                _enabledSkillsBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Evade.UseSkills = args.NewValue; };
                EvadeMain.Add(_enabledSkillsBox);

                _enabledWalkBox = new MenuCheckBox("evade_walk", "Try to walk out of skillshots (doesnt work well enough yet)", false);
                _enabledWalkBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Evade.UseWalk = args.NewValue; };
                _enabledWalkBox.Hidden = true;
                EvadeMain.Add(_enabledWalkBox);

                _enabledDrawings = new MenuCheckBox("evade_draw", "Draw Evade Drawings", false);
                _enabledDrawings.OnValueChange += delegate(ChangedValueArgs<bool> args) { DrawEvade.Active = args.NewValue; };
                EvadeMain.Add(_enabledDrawings);

                _jumpMode = EvadeMain.Add(new MenuComboBox("evade_jumpmode", "Jump Logic", 1, new[] {"Mouse Cursor", "DaPip's BestJumpPos"}));
                _jumpMode.OnValueChange += delegate(ChangedValueArgs<int> args) { JumpMode = args.NewValue; };

                EvadeStatusInit();
                EvadeSkillsInit();
                EvadeEnemySkillsInit();
                EvadeOverrideInit();

                AddDodgeableEntries();
                AddEvadeEntries();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            FirstRun();
        }

        public static void Unload()
        {
            DodgeableSkillsMenuByChampion = null;
            EvadeOverrideMenuByChampion = null;
            EvadeSkillsMenuByChampion = null;
            EvadeStatusMenuByChampion = null;
        }

        private static void AddEvadeEntries()
        {
            var sorted = new SortedDictionary<string, List<DodgeAbilityInfo>>();
            foreach (var dodgeAbility in AbilityDatabase.DodgeAbilities.Where(a => a.UseInEvade))
            {
                var champion = dodgeAbility.Champion;
                if (sorted.ContainsKey(champion))
                {
                    sorted[champion].Add(dodgeAbility);
                }
                else
                {
                    sorted.Add(champion, new List<DodgeAbilityInfo> {dodgeAbility});
                }
            }

            foreach (var champion in sorted)
            {
                var champ = champion.Key;
                EvadeSkillsMenuByChampion.Add(champ, new List<MenuItem> {EvadeSkillsMenu.AddLabel(champ)});
                EvadeStatusMenuByChampion.Add(champ, new List<MenuItem> {EvadeStatusMenu.AddLabel(champ)});

                EvadeOverrideMenuByChampion.Add(champ, new List<MenuItem> {EvadeOverrideMenu.AddLabel(champ)});
                EvadeOverrideMenuByChampion[champ][0].Hidden = true;

                foreach (var abilityInfo in champion.Value)
                {
                    AddEvadeSkillsEntry(champ, abilityInfo);
                    AddEvadeStatusEntry(champ, abilityInfo);
                    AddEvadeOverrideEntry(champ, abilityInfo);
                }

                EvadeSkillsMenuByChampion[champ].Add(EvadeSkillsMenu.AddSeparator());
            }
        }

        private static void AddEvadeOverrideEntry(string champ, DodgeAbilityInfo abilityInfo)
        {
            var abilityKey = abilityInfo.AbilitySlot.ToFriendlyString();
            var comboBox = EvadeOverrideMenu.Add(new MenuComboBox("override_" + champ + abilityKey,
                "Choose logic for " + champ + "'s " + abilityKey + " (" + abilityInfo.AbilityType.ToFriendlyString() + ")", 0, new[] {"Default"}));
            comboBox.Hidden = true;
            EvadeOverrideMenuByChampion[champ].Add(comboBox);
        }

        private static void AddEvadeSkillsEntry(string champ, DodgeAbilityInfo abilityInfo)
        {
            var abilityKey = abilityInfo.AbilitySlot.ToFriendlyString();
            EvadeSkillsMenuByChampion[champ].Add(EvadeSkillsMenu.Add(new MenuCheckBox(
                "use_" + champ + abilityKey,
                "Use " + abilityKey + " (" + abilityInfo.AbilityType.ToFriendlyString() + ")")));
            EvadeSkillsMenuByChampion[champ].Add(EvadeSkillsMenu.Add(new MenuIntSlider(
                "danger_" + champ + abilityKey, "Minimum danger", abilityInfo.MinDanger, 5, 1)));
            EvadeSkillsMenuByChampion[champ].Add(EvadeSkillsMenu.AddSeparator(5));
        }

        private static void AddDodgeableEntries()
        {
            var sorted = new SortedDictionary<string, List<AbilityInfo>>();
            foreach (var ability in AbilityDatabase.Abilities.Where(a => a.Danger > 0 && a.ObjectName != ""))
            {
                var champion = ability.Champion;
                if (sorted.ContainsKey(champion))
                {
                    sorted[champion].Add(ability);
                }
                else
                {
                    sorted.Add(champion, new List<AbilityInfo> {ability});
                }
            }

            foreach (var champion in sorted)
            {
                var champ = champion.Key;
                DodgeableSkillsMenuByChampion.Add(champ, new List<MenuItem> {DodgeableSkillsMenu.AddLabel(champ)});
                foreach (var abilityInfo in champion.Value)
                {
                    AddDodgeableEntry(champ, abilityInfo);
                }

                DodgeableSkillsMenuByChampion[champ].Add(DodgeableSkillsMenu.AddSeparator());
            }
        }

        private static void EvadeStatusInit()
        {
            EvadeStatusMenu = EvadeMain.Add(new Menu("Evade.StatusMenu", "Evade Status", true));
            EvadeStatusMenu.Hidden = true;
        }

        private static void AddEvadeStatusEntry(string champ, DodgeAbilityInfo abilityInfo)
        {
            EvadeStatusMenuByChampion[champ].Add(EvadeStatusMenu.Add(new MenuCheckBox(
                "isActive_" + champ + abilityInfo.AbilitySlot.ToFriendlyString(),
                "Active: " + champ + abilityInfo.AbilityId)));
        }

        private static void EvadeOverrideInit()
        {
            EvadeOverrideMenu = EvadeMain.Add(new Menu("Evade.OverrideMenu", "External Logic", true));
            EvadeOverrideMenu.AddLabel("   ↓ Evade logic from other addons will be available here ↓");
            EvadeOverrideMenu.AddSeparator(40);
        }

        private static void EvadeEnemySkillsInit()
        {
            DodgeableSkillsMenu = EvadeMain.Add(new Menu("EnemySkills", "Dodgeable Skills", true));
        }

        private static void AddDodgeableEntry(string champ, AbilityInfo abilityInfo)
        {
            DodgeableSkillsMenuByChampion[champ].Add(DodgeableSkillsMenu.Add(new MenuCheckBox(
                "dodge_" + champ + abilityInfo.ObjectName + abilityInfo.AbilityId,
                "Dodge " + abilityInfo.ObjectName + " (" + abilityInfo.AbilitySlot.ToFriendlyString() + ")")));
            DodgeableSkillsMenuByChampion[champ].Add(DodgeableSkillsMenu.Add(new MenuIntSlider(
                "danger_" + champ + abilityInfo.ObjectName + abilityInfo.AbilityId, "Danger", abilityInfo.Danger, 5, 1)));
            DodgeableSkillsMenuByChampion[champ].Add(DodgeableSkillsMenu.AddSeparator(5));
        }

        private static void EvadeSkillsInit()
        {
            EvadeSkillsMenu = EvadeMain.Add(new Menu("EvadeSkills", "Evade Skills", true));
        }

        public static int OverrideValue(this DodgeAbilityInfo info)
        {
            var combobox = (MenuComboBox) EvadeOverrideMenuByChampion[info.Champion]
                .FirstOrDefault(s => s.Name == "override_" + info.Champion + info.AbilitySlot.ToFriendlyString());
            return combobox == null ? 0 : combobox.CurrentValue;
        }

        public static void SetStatus(this DodgeAbilityInfo info, bool newValue)
        {
            if (newValue && _activeSpell == null)
            {
                _activeSpell = (MenuCheckBox) EvadeStatusMenuByChampion[info.Champion]
                    .FirstOrDefault(s => s.Name == "isActive_" + info.Champion + info.AbilitySlot.ToFriendlyString());
                if (_activeSpell != null)
                {
                    _activeSpell.CurrentValue = true;
                }
            }
            else if (!newValue && _activeSpell != null)
            {
                _activeSpell.CurrentValue = false;
                _activeSpell = null;
            }
        }

        public static bool ShouldUse(this DodgeAbilityInfo info)
        {
            var champList = EvadeSkillsMenuByChampion[info.Champion];
            var checkbox = (MenuCheckBox) champList.FirstOrDefault(s =>
                s.Name == "use_" + info.Champion + info.AbilitySlot.ToFriendlyString());
            if (checkbox != default(MenuCheckBox)) return checkbox.CurrentValue;

            Console.WriteLine("[Evade/MenuHandler] Strange! Couldn't find menu item for " + info.Champion +
                              info.AbilitySlot.ToFriendlyString());
            return false;
        }

        public static int GetDanger(this DodgeAbilityInfo info)
        {
            return ((MenuIntSlider) EvadeSkillsMenuByChampion[info.Champion]
                    .First(s => s.Name == "danger_" + info.Champion + info.AbilitySlot.ToFriendlyString()))
                .CurrentValue;
        }

        public static bool ShouldUse(this AbilityInfo info)
        {
            return ((MenuCheckBox) DodgeableSkillsMenuByChampion[info.Champion]
                    .First(s => s.Name == "dodge_" + info.Champion + info.ObjectName + info.AbilityId))
                .CurrentValue;
        }

        public static int GetDanger(this AbilityInfo info)
        {
            return ((MenuIntSlider) DodgeableSkillsMenuByChampion[info.Champion]
                    .First(s => s.Name == "danger_" + info.Champion + info.ObjectName + info.AbilityId))
                .CurrentValue;
        }

        private static void FirstRun()
        {
            Evade.UseWalk = false; //_enabledWalkBox.CurrentValue;
            Evade.UseSkills = _enabledSkillsBox.CurrentValue;
            DrawEvade.Active = _enabledDrawings.CurrentValue;
            JumpMode = _jumpMode.CurrentValue;
        }

        public static void Update()
        {
            if (Game.IsInGame)
            {
                var champ = LocalPlayer.Instance.CharName;
                var enemychamps = EntitiesManager.EnemyTeam.Select(e => e.CharName).ToArray();

                foreach (var pair in EvadeSkillsMenuByChampion)
                {
                    if (pair.Key == champ)
                    {
                        foreach (var menuItem in pair.Value)
                        {
                            menuItem.Hidden = false;
                        }
                    }
                    else
                    {
                        foreach (var menuItem in pair.Value)
                        {
                            menuItem.Hidden = true;
                        }
                    }
                }

                foreach (var pair in DodgeableSkillsMenuByChampion)
                {
                    if (enemychamps.Contains(pair.Key))
                    {
                        foreach (var menuItem in pair.Value)
                        {
                            menuItem.Hidden = false;
                        }
                    }
                    else
                    {
                        foreach (var menuItem in pair.Value)
                        {
                            menuItem.Hidden = true;
                        }
                    }
                }
            }
            else
            {
                foreach (var pair in EvadeSkillsMenuByChampion)
                {
                    foreach (var menuItem in pair.Value)
                    {
                        menuItem.Hidden = false;
                    }
                }

                foreach (var pair in DodgeableSkillsMenuByChampion)
                {
                    foreach (var menuItem in pair.Value)
                    {
                        menuItem.Hidden = false;
                    }
                }
            }
        }
    }
}