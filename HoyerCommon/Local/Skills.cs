using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;

namespace Hoyer.Common.Local
{
    public static class Skills
    {
        public static readonly List<SkillBase> Active = new List<SkillBase>();
        public static event Action Initialize = delegate {};

        public static void AddFromDatabase(AbilitySlot slot)
        {
            var data = AbilityDatabase.Get(LocalPlayer.Instance.CharName, slot)[0];
            if (data != null)
            {
                Active.Add(new SkillBase(slot, data.SkillType, data.Range == 0 ? data.MaxRange : data.Range, data.ProjectileSpeed, data.Radius, data.FixedDelay));
            }
        }

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