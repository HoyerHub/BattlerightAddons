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

    public enum AbilityType
    {
        Melee,
        LineProjectile,
        CurveProjectile,
        CircleThrowObject,
        CircleJump,
        Dash
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
        public string Champion = "";
        public AbilitySlot AbilitySlot;
        public int AbilityId;
        public string ObjectName = "";
        public AbilityType AbilityType;
        public int Danger = 1;
        public Battlerite[] ImportantBattlerites;
        public CollisionType CollisionType = CollisionType.Units | CollisionType.Walls;
        public float Range = 0;
        public float MaxRange = 0;
        public float MinRange = 0;
        public float MinCastTime = 0;
        public float MaxCastTime = 0;
        public float Radius = 0;
        public float CollideCount = 1;
        public float Speed = 0;
        public float FixedDelay = 0;                
        public bool RequiresCharging = false;
        public bool ShouldEvade = true;
        public bool CanCounter = true;
        public float StartDelay = 0;
    }
}