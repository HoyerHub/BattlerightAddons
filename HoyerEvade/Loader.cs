using BattleRight.Core;
using BattleRight.Sandbox;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;

namespace Hoyer.Evade
{
    public class Loader : IAddon
    {
        public void OnInit()
        {
            Evade.Init();
            MenuEvents.Initialize += MenuInit;
        }

        private void MenuInit()
        {
            
        }

        public void OnUnload()
        {
        }
    }
}