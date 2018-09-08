using System;
using BattleRight.Sandbox;
using BattleRight.SDK.ClipperLib;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Data.Addons;
using Hoyer.Common.Debugging;
using Hoyer.Common.Local;
using Hoyer.Common.Trackers;
using Hoyer.Common.Utilities;

namespace Hoyer.Common
{
    public class Main:IAddon
    {
        public static Clipper Clipper = new Clipper();
        public static event Action Init = delegate { };

        public static void DelayAction(Action action, float seconds)
        {
            System.Threading.Timer timer = null;
            timer = new System.Threading.Timer(obj =>
                {
                    action();
                    timer.Dispose();
                },
                null, (long)(seconds * 1000), System.Threading.Timeout.Infinite);
        }

        public void OnInit()
        {
            AbilityDatabase.Setup();
            AddonMenus.Setup();
            Aimbot.Aimbot.Setup();
            StealthPrediction.Setup();
            HideNames.Setup();
            Skills.Setup();
            ObjectTracker.Setup();
            MenuEvents.Setup();
            DebugHelper.Setup();
            BuffTracker.Setup();
            DelayAction(Init.Invoke, 0.5f);
        }

        public void OnUnload()
        {
            Console.WriteLine("Unload Common Started");
            AbilityDatabase.Unload();
            AddonMenus.Unload();
            Aimbot.Aimbot.Unload();
            StealthPrediction.Unload();
            HideNames.Unload();
            Skills.Unload();
            ObjectTracker.Unload();
            MenuEvents.Unload();
            DebugHelper.Unload();
            BuffTracker.Unload();
            Console.WriteLine("Unload Common Ended");
        }
    }
}