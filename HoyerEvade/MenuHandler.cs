using System;
using System.Linq;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;

namespace Hoyer.Evade
{ 
    public static class MenuHandler
    {
        public static Menu HoyerMain;
        public static Menu EvadeMain;
        
        private static MenuCheckBox _enabledWalkBox;
        private static MenuCheckBox _enabledSkillsBox;

        public static void Init()
        {
            HoyerMain = MainMenu.GetMenu("HoyerMain");
            EvadeMain = HoyerMain.Add(new Menu("HoyerEvade", "Evade", true));

            EvadeMain.Add(new MenuLabel("Evade"));
            _enabledWalkBox = new MenuCheckBox("evade_walk", "Try to walk out of skillshots (doesnt work well enough yet)");
            _enabledWalkBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Evade.UseWalk = args.NewValue; };
            EvadeMain.Add(_enabledWalkBox);
            
            _enabledSkillsBox = new MenuCheckBox("evade_skills", "Use skills to dodge dangerous skillshots");
            _enabledSkillsBox.OnValueChange += delegate(ChangedValueArgs<bool> args) { Evade.UseSkills = args.NewValue; };
            EvadeMain.Add(_enabledSkillsBox);

            FirstRun();
        }

        private static void FirstRun()
        {
            Evade.UseWalk = _enabledWalkBox.CurrentValue;
            Evade.UseSkills = _enabledSkillsBox.CurrentValue;
        }
    }
}