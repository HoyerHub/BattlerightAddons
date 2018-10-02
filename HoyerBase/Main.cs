using System;
using BattleRight.Core;
using BattleRight.Core.Math;
using BattleRight.Sandbox;
using BattleRight.SDK.ClipperLib;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Data.Addons;
using Hoyer.Base.Debugging;
using Hoyer.Base.Local;
using Hoyer.Base.Trackers;
using Hoyer.Base.Utilities;

namespace Hoyer.Base
{
    public class Main:IAddon
    {
        public static Clipper Clipper = new Clipper();
        public static event Action Init = delegate { };

        public static Vector2 MouseWorldPos
        {
            get
            {
                if (!_updatedMouseThisFrame)
                {
                    _updatedMouseThisFrame = true;
                    _mouseWorldPos = InputManager.MousePosition.ScreenToWorld();
                }
                return _mouseWorldPos;
            }
        }
        private static Vector2 _mouseWorldPos = Vector2.Zero;
        private static bool _updatedMouseThisFrame;

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
            StealthPrediction.Setup();
            HideNames.Setup();
            Skills.Setup();
            ObjectTracker.Setup();
            MenuEvents.Setup();
            DebugHelper.Setup();
            BuffTracker.Setup();
            Game.OnPreUpdate += Game_OnPreUpdate;
            DelayAction(Init.Invoke, 0.5f);
        }

        private void Game_OnPreUpdate(EventArgs args)
        {
            _updatedMouseThisFrame = false;
        }

        public void OnUnload()
        {
            Console.WriteLine("Unload Common Started");
            AbilityDatabase.Unload();
            AddonMenus.Unload();
            StealthPrediction.Unload();
            HideNames.Unload();
            Skills.Unload();
            ObjectTracker.Unload();
            MenuEvents.Unload();
            DebugHelper.Unload();
            BuffTracker.Unload();
            Game.OnPreUpdate -= Game_OnPreUpdate;
            Console.WriteLine("Unload Common Ended");
        }
    }
}