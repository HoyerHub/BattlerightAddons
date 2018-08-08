using System;
using BattleRight.Sandbox;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Debug;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;
using Component = Hoyer.Common.Utilities.Component;

namespace Hoyer.Common
{
    public class Main:IAddon
    {
        public void OnInit()
        {
            InitializeStaticClasses();
        }

        private void InitializeStaticClasses()
        {
            StealthPrediction.Setup();
            HideNames.Setup();
            Skills.Setup();
            DrawProjectiles.Setup();
            AbilityTracker.Setup();
            MenuEvents.Setup();
            CommonEvents.Setup();
            Component.Setup();
        }

        public void OnUnload()
        {
        }
    }
}