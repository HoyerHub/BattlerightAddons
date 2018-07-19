using BattleRight.Core.Enumeration;

namespace Hoyer.Common.Data.Abilites
{
    public enum DodgeAbilityType
    {
        Counter,
        Jump,
        Ghost
    }
    public class DodgeAbilityInfo
    {
        public string Champion;
        public AbilitySlot AbilitySlot;
        public DodgeAbilityType AbilityType;
        public int Priority = 3; //Evade will choose lowest available
        public int MinDanger = 0;
        public float WaitAfter = 0;
        public float CastTime = 0;
    }
}