using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using Hoyer.Common.Utilities;
using UnityEngine;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Common.Extensions
{
    public static class CharacterExtensions
    {
        public static Vector2 Pos(this Character character)
        {
            return StealthPrediction.ShouldUse ? StealthPrediction.GetPosition(character) : character.MapObject.Position;
        }

        public static bool IsValidTarget(this Character enemy)
        {
            if (enemy == null || enemy.Buffs == null || enemy.Living.IsDead || (enemy.PhysicsCollision.IsImmaterial && !enemy.CharacterModel.IsModelInvisible))
            {
                return false;
            }

            return true;
        }

        public static bool IsValidTargetProjectile(this Character enemy, bool useOnHardCC = false)
        {
            if (enemy == null || enemy.Buffs == null || enemy.Living.IsDead || enemy.PhysicsCollision.IsImmaterial && !enemy.CharacterModel.IsModelInvisible)
            {
                return false;
            }
            foreach (var buff in enemy.Buffs.Where(b => b != null && b.ObjectName != null))
            {
                if (buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" ||
                                     buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam" || buff.ObjectName == "TimeBenderBuff" || buff.ObjectName == "DivineShieldBuff")
                {
                    return false;
                }
                if (!useOnHardCC && (buff.ObjectName == "Incapacitate" || buff.ObjectName == "PetrifyStone"))
                {
                    return false;
                }
            }
            return !LocalPlayer.Instance.CheckCollisionToTarget(enemy, 0.35f);
        }

        // ReSharper disable once InconsistentNaming
        public static bool IsValidTarget(this Character enemy, SkillBase spell, bool isProjectile = true, bool useOnHardCC = false, bool avoidStealted = false)
        {
            if (enemy == null || enemy.Buffs == null || enemy.Living.IsDead || enemy.PhysicsCollision.IsImmaterial && !enemy.CharacterModel.IsModelInvisible || avoidStealted && enemy.CharacterModel.IsModelInvisible)
            {
                return false;
            }

            float timeLeft;
            if (spell.FixedDelay > 0)
            {
                timeLeft = spell.FixedDelay;
            }
            else timeLeft = enemy.Distance(LocalPlayer.Instance) / spell.Speed;
            timeLeft += 0.2f;
            
            foreach (var buff in enemy.Buffs.Where(b => b != null && b.ObjectName != null))
            {
                if (isProjectile && (buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" || buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam" || buff.ObjectName == "TimeBenderBuff" || buff.ObjectName == "DivineShieldBuff"))
                {
                    if (timeLeft < buff.TimeToExpire)
                    {
                        return false;
                    }
                }
                if (!useOnHardCC && (buff.ObjectName == "Incapacitate" || buff.ObjectName == "PetrifyStone"))
                {
                    if (timeLeft < buff.TimeToExpire)
                    {
                        return false;
                    }
                }
                if (buff.ObjectName == "Jetpack")
                {
                    if (timeLeft < buff.TimeToExpire)
                    {
                        return false;
                    }
                }
            }
            if (isProjectile) return !LocalPlayer.Instance.CheckCollisionToTarget(enemy, spell.SpellCollisionRadius);
            return true;
        }

        public static Prediction.Output GetPrediction(this Character target, SkillBase castingSpell)
        {
            return Prediction.Get(target, castingSpell);
        }

        public static bool IsHoveringNear(this MapGameObject obj)
        {
            return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y + 1).ScreenToWorld().Distance(obj.Position) < 2;
        }
    }
}