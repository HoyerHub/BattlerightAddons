using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;

namespace Hoyer.Evade
{
    public static class MenuHandler
    {
        public static Menu HoyerMain;
        public static Menu EvadeMain;
        public static Menu EvadeSkillsMenu;

        private static readonly Dictionary<string, List<MenuItem>> SkillMenuByChampion = new Dictionary<string, List<MenuItem>>();

        private static MenuCheckBox _enabledWalkBox;
        private static MenuCheckBox _enabledSkillsBox;

        public static void Init()
        {
            HoyerMain = MainMenu.GetMenu("HoyerMain");
            EvadeMain = HoyerMain.Add(new Menu("HoyerEvade", "Evade", true));

            EvadeMain.Add(new MenuLabel("Evade"));
            _enabledSkillsBox = new MenuCheckBox("evade_skills", "Use skills to dodge dangerous skillshots");
            _enabledSkillsBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Evade.UseSkills = args.NewValue; };
            EvadeMain.Add(_enabledSkillsBox);

            _enabledWalkBox = new MenuCheckBox("evade_walk", "Try to walk out of skillshots (doesnt work well enough yet)", false);
            _enabledWalkBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Evade.UseWalk = args.NewValue; };
            EvadeMain.Add(_enabledWalkBox);

            EvadeSkillsInit();

            FirstRun();
        }

        private static void EvadeSkillsInit()
        {
            EvadeSkillsMenu = EvadeMain.Add(new Menu("EvadeSkills", "Evade Skills", true));

            var sorted = new SortedDictionary<string, List<DodgeAbilityInfo>>();
            foreach (var dodgeAbility in AbilityDatabase.DodgeAbilities.Where(a => a.UseInEvade))
            {
                if (sorted.ContainsKey(dodgeAbility.Champion))
                {
                    sorted[dodgeAbility.Champion].Add(dodgeAbility);
                }
                else
                {
                    sorted.Add(dodgeAbility.Champion, new List<DodgeAbilityInfo> {dodgeAbility});
                }
            }

            foreach (var champion in sorted)
            {
                SkillMenuByChampion.Add(champion.Key, new List<MenuItem> {EvadeSkillsMenu.AddLabel(champion.Key)});
                foreach (var abilityInfo in champion.Value)
                {
                    SkillMenuByChampion[champion.Key].Add(EvadeSkillsMenu.Add(new MenuCheckBox(
                        "use_" + champion.Key + abilityInfo.AbilitySlot.ToKeyString(),
                        "Use " + abilityInfo.AbilitySlot.ToKeyString() + " (" + abilityInfo.AbilityType.ToFriendlyString() + ")")));
                    SkillMenuByChampion[champion.Key].Add(EvadeSkillsMenu.Add(new MenuIntSlider(
                        "danger_" + champion.Key + abilityInfo.AbilitySlot.ToKeyString(), "Minimum danger", abilityInfo.MinDanger, 5, 1)));
                }

                SkillMenuByChampion[champion.Key].Add(EvadeSkillsMenu.AddSeparator());
            }
        }

        public static bool ShouldUse(this DodgeAbilityInfo info)
        {
            return ((MenuCheckBox)SkillMenuByChampion[info.Champion]
                .First(s=>s.Name == "use_" + info.Champion + info.AbilitySlot.ToKeyString()))
                .CurrentValue;
        }

        public static int GetDanger(this DodgeAbilityInfo info)
        {
            return ((MenuIntSlider)SkillMenuByChampion[info.Champion]
                .First(s => s.Name == "danger_" + info.Champion + info.AbilitySlot.ToKeyString()))
                .CurrentValue;
        }

        private static void FirstRun()
        {
            Evade.UseWalk = _enabledWalkBox.CurrentValue;
            Evade.UseSkills = _enabledSkillsBox.CurrentValue;
        }

        public static void Update()
        {
            if (Game.IsInGame)
            {
                var champ = LocalPlayer.Instance.CharName;
                foreach (var pair in SkillMenuByChampion)
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
            }
            else
            {
                foreach (var pair in SkillMenuByChampion)
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