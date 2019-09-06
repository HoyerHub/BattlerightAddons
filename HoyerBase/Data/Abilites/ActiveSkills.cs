using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK;
using Hoyer.Base.Extensions;

namespace Hoyer.Base.Data.Abilites
{
    public static class ActiveSkills
    {
        public static List<SkillBase> Active;
        public static List<AbilityInfo> ActiveInfos;
        public static event Action Initialize = delegate {};

        public static void Setup()
        {
            Active = new List<SkillBase>();
            ActiveInfos = new List<AbilityInfo>();
            Game.OnMatchStart += Game_OnMatchStart;
        }

        public static void Unload()
        {
            Game.OnMatchStart -= Game_OnMatchStart;
            Active = null;
            ActiveInfos = null;
        }

        public static void AddFromDatabase(AbilitySlot slot)
        {
            var data = AbilityDatabase.Get(LocalPlayer.Instance.CharName, slot)[0];
            if (data != null)
            {
                Active.Add(new SkillBase(slot, data.AbilityType.ToSkillType(), data.Range == 0 ? data.MaxRange : data.Range, data.Speed, data.Radius, data.FixedDelay));
                ActiveInfos.Add(data);
            }
        }

        public static void AddFromDatabase()
        {
            ActiveInfos.Clear();
            Active.Clear();
            if (string.IsNullOrEmpty(LocalPlayer.Instance.CharName))
            {
                foreach (var ability in AbilityDatabase.Abilities.Where(a => a.Champion == "Shen Rao"))
                {
                    ActiveInfos.Add(ability);
                    Active.Add(new SkillBase(ability.AbilitySlot, ability.AbilityType.ToSkillType(), ability.Range == 0 ? ability.MaxRange : ability.Range, ability.Speed, ability.Radius, ability.FixedDelay));
                }
            }
            else foreach (var ability in AbilityDatabase.Abilities.Where(a=>a.Champion == LocalPlayer.Instance.CharName))
            {
                ActiveInfos.Add(ability);
                Active.Add(new SkillBase(ability.AbilitySlot, ability.AbilityType.ToSkillType(), ability.Range == 0 ? ability.MaxRange : ability.Range, ability.Speed, ability.Radius, ability.FixedDelay));
            }
        }

        public static SkillBase Get(int id)
        {
            var data = ActiveInfos.FirstOrDefault(a => a.AbilityId == id);
            if (data == null)
            {
                return null;
            }
            return Active.Get(data.AbilitySlot);
        }

        public static AbilityInfo GetData(AbilitySlot slot)
        {
            return ActiveInfos.FirstOrDefault(a => a.AbilitySlot == slot);
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            ActiveInfos.Clear();
            Active.Clear();
            Initialize.Invoke();
        }
    }
}