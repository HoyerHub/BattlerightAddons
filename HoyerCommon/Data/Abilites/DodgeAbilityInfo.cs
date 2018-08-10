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
        Ghost
    }
    public class DodgeAbilityInfo
    {
        public string Champion;
        public int AbilityId;
        public AbilitySlot AbilitySlot;
        public DodgeAbilityType AbilityType;
        public bool UsesMousePos = false;
        public bool NeedsSelfCast = false;
        public float Range = 0;
        public int Priority = 3; //Evade will choose lowest available
        public int MinDanger = 0;
        public float CastTime = 0;
        public bool UseInEvade = true;
        public AbilitySlot SharedCooldown = AbilitySlot.Taunt;

        public Vector2 GetSafeJumpPos()
        {
            var nearbyAlly = EntitiesManager.LocalTeam.FirstOrDefault(ally => ally.Distance(LocalPlayer.Instance) < Range);
            if (nearbyAlly != null)
            {
                return nearbyAlly.MapObject.Position;
            }

            var towardsBase = LocalPlayer.Instance.MapObject.Position.Extend(new Vector2(-100, 0), Range);
            if (!EntitiesManager.EnemyTeam.Any(e => e.Distance(towardsBase) < 5))
            {
                return towardsBase;
            }

            var mousePos = InputManager.MousePosition.ScreenToWorld();
            if (!EntitiesManager.EnemyTeam.Any(e => e.Distance(mousePos) < 5))
            {
                return mousePos;
            }

            return Vector2.Zero;
        }
    }
}