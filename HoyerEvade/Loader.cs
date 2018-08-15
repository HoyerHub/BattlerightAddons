using BattleRight.Core;
using BattleRight.Sandbox;
using Hoyer.Common;
using Hoyer.Common.Debug;
using Hoyer.Common.Local;
using Hoyer.Common.Utilities;

namespace Hoyer.Evade
{
    public class Loader : IAddon
    {
        public void OnInit()
        {
            DrawEvade.Setup();
            Evade.Init();
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
        }
        
        public void OnUnload()
        {
        }
    }
}