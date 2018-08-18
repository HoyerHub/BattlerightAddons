using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Common.Utilities;

namespace Hoyer.Common.Data.Abilites
{
    public enum DodgeAbilityType
    {
        Counter,
        Jump,
        Shield,
        HealthShield,
        AoEHealthShield,
        Ghost
    }
    public class DodgeAbilityInfo
    {
        public string Champion;
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