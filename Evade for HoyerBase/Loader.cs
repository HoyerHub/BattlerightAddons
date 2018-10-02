using System;
using BattleRight.Sandbox;
using Hoyer.Base.Local;

namespace Hoyer.Evade
{
    public class Loader : IAddon
    {
        public void OnInit()
        {
            DrawEvade.Setup();
            Evade.Setup();
            MenuEvents.Initialize += MenuHandler.Init;
            MenuEvents.Update += MenuHandler.Update;
        }
        
        public void OnUnload()
        {
            Console.WriteLine("Unload Evade Started");
            MenuHandler.Unload();
            DrawEvade.Unload();
            Evade.Unload();
            MenuEvents.Initialize -= MenuHandler.Init;
            MenuEvents.Update -= MenuHandler.Update;
            Console.WriteLine("Unload Evade Ended");
        }
    }
}