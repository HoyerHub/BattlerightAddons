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
            if (character.CharacterModel.IsModelInvisible && StealthPrediction.Positions.ContainsKey(character.CharName))
            {
                return StealthPrediction.Positions[character.CharName];
            }
            return character.MapObject.Position;
        }

        public static bool IsValidTarget(this Character enemy)
        {
            if (enemy == null || enemy.Buffs == null || enemy.Living.IsDead || enemy.PhysicsCollision.IsImmaterial && !enemy.CharacterModel.IsModelInvisible)
            {
                return false;
            }

            return true;
        }

        // ReSharper disable once InconsistentNaming
        public static bool IsValidTarget(this Character enemy, SkillBase spell, bool isProjectile = true, bool useOnHardCC = false)
        {
            if (enemy == null || enemy.Buffs == null || enemy.Living.IsDead || enemy.PhysicsCollision.IsImmaterial && !enemy.CharacterModel.IsModelInvisible)
            {
                return false;
            }

            float timeLeft;
            if (spell.FixedDelay > 0)
            {
                timeLeft = spell.FixedDelay;
            }
            else timeLeft = enemy.Distance(LocalPlayer.Instance) / spell.Speed;

            var ret = true;
            foreach (var buff in enemy.Buffs.Where(b => b != null && b.ObjectName != null))
            {
                if (isProjectile && (buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" || buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam"))
                {
                    if (timeLeft < buff.TimeToExpire)
                    {
                        ret = false;
                    }
                }
                if (!useOnHardCC && (buff.ObjectName == "Incapacitate" || buff.ObjectName == "PetrifyStone"))
                {
                    if (timeLeft < buff.TimeToExpire)
                    {
                        ret = false;
                    }
                }
                if (buff.ObjectName == "Jetpack")
                {
                    if (timeLeft < buff.TimeToExpire)
                    {
                        ret = false;
                    }
                }
            }

            if (isProjectile && LocalPlayer.Instance.CheckCollisionToTarget(enemy, spell.SpellCollisionRadius))
            {
                ret = false;
            }

            return ret;
        }

        public static Vector2 GetPrediction(this Character target, SkillBase castingSpell)
        {
            var distance = target.Distance(LocalPlayer.Instance);
            return castingSpell.FixedDelay > 0
                ? new Vector2(target.Pos().X + target.NetworkMovement.Velocity.X * castingSpell.FixedDelay,
                    target.Pos().Y + target.NetworkMovement.Velocity.Y * castingSpell.FixedDelay)
                : new Vector2(target.Pos().X + target.NetworkMovement.Velocity.X * (distance / castingSpell.Speed),
                    target.Pos().Y + target.NetworkMovement.Velocity.Y * (distance / castingSpell.Speed));
        }

        public static bool IsHoveringNear(this MapGameObject obj)
        {
            return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y + 1).ScreenToWorld().Distance(obj.Position) < 2;
        }
    }
}