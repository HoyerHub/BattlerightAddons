using System;
using BattleRight.Core;

namespace Hoyer.Common.Local
{
    public static class CommonEvents
    {
        public static event Action PreUpdate = delegate { };
        public static event Action Update = delegate { };
        public static event Action PostUpdate = delegate { };

        public static void Setup()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            PreUpdate.Invoke();
            Update.Invoke();
            PostUpdate.Invoke();
        }
    }
}