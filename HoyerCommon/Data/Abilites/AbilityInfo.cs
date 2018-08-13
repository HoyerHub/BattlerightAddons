using System;
using BattleRight.Core.Enumeration;
using BattleRight.SDK.Enumeration;

namespace Hoyer.Common.Data.Abilites
{
    [Flags]
    public enum CollisionType
    {
        Units,
        Walls
    }

    public struct Field
    {
        public string Name;
        public Type Type;

        public Field(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }

    public class Battlerite
    {
        public string Name;
        public Field ChangedField;
        public object NewValue;
    }

    public class AbilityInfo
    {
        public string Champion;
        public AbilitySlot AbilitySlot;
        public int AbilityId;
        public string ObjectName = "";
        public SkillType SkillType;
        public int Danger;
        public Battlerite[] ImportantBattlerites;
        public CollisionType CollisionType = CollisionType.Units | CollisionType.Walls;
        public float Range = 0;
        public float MaxRange = 0;
        public float MinRange = 0;
        public float MinCastTime = 0;
        public float MaxCastTime = 0;
        public float Radius = 0;
        public float CollideCount = 1;
        public float ProjectileSpeed = 0;
        public float FixedDelay = 0;                
        public bool RequiresCharging;
        public bool CanCounter = true;
    }
}