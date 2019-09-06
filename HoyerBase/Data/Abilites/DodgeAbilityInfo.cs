using BattleRight.Core.Enumeration;

namespace Hoyer.Base.Data.Abilites
{
    public enum DodgeAbilityType
    {
        Counter,
        Jump,
        Shield,
        Obstacle,
        HealthShield,
        AoEHealthShield,
        Ghost,
        KnockAway
    }

    public class DodgeAbilityInfo
    {
        public string Champion;
        public string ObjectName = "";
        public int Cooldown;
        public int AbilityId;
        public int AbilityIndex = -1;
        public AbilitySlot AbilitySlot;
        public DodgeAbilityType AbilityType;
        public bool UsesMousePos = false;
        public bool NeedsSelfCast = false;
        public bool IsIFrame = true;
        public float Range = 0;
        public int Priority = 3; //Evade will choose lowest available
        public int MinDanger = 0;
        public float CastTime = 0;
        public bool UseInEvade = true;
        public AbilitySlot SharedCooldown = AbilitySlot.Taunt;
    }
}