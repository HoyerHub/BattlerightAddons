using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;

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
                Active.Add(new SkillBase(slot, data.AbilityType.ToSkillType(), data.Range == 0 ? data.MaxRange : data.Range, data.ProjectileSpeed, data.Radius, data.FixedDelay));
            }
        }

        public static SkillBase Get(int id)
        {
            return Active.Get(AbilityDatabase.Get(id).AbilitySlot);
        }

        public static void Setup()
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