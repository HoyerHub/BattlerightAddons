using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.SDK;

namespace Hoyer.Common.Local
{
    public static class Skills
    {
        public static readonly List<SkillBase> Active = new List<SkillBase>();
        public static event Action Initialize = delegate {};

        static Skills()
        {
            Game.OnMatchStart += Game_OnMatchStart;
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            Active.Clear();
            Initialize.Invoke();
        }
    }
}