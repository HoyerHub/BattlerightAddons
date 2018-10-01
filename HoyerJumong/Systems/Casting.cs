using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Common;
using Hoyer.Common.Extensions;
using Hoyer.Common.Prediction;

namespace Hoyer.Champions.Jumong.Modes
{
    public static class Casting
    {
        public static void CastLogic()
        {
            if (LocalPlayer.Instance.PhysicsCollision.IsImmaterial && EnemiesInRange(6.5f).Count > 0) return;

            if (MenuHandler.SkillBool("close_a3") && EnemiesInRange(2.5f).Count > 0)
            {
                if (AbilitySlot.Ability3.IsReady())
                {
                    Cast(AbilitySlot.Ability3);
                    return;
                }
            }

            var enemyTeam = EntitiesManager.EnemyTeam;
            if (TargetSelection.UseMaxCursorDist)
            {
                enemyTeam = enemyTeam.Where(e => e.Pos().Distance(Main.MouseWorldPos) <= TargetSelection.MaxCursorDist).ToArray();
            }
            var validEnemies = enemyTeam.Where(e => e.IsValidTarget() && e.Pos().Distance(Vector2.Zero) > 0.3f).ToList();
            var validForProjectiles = validEnemies.Any(e => e.IsValidTargetProjectile());
            var validForBigProjectiles = validEnemies.Any(e => e.IsValidTargetProjectile(true));
            if (validEnemies.Any())
            {
                var closestRange = validEnemies.OrderBy(e => e.Distance(LocalPlayer.Instance)).First().Distance(LocalPlayer.Instance);
                if (MenuHandler.UseSkill(AbilitySlot.Ability4) && AbilitySlot.Ability4.IsReady() && AbilitySlot.Ability4.InRange(closestRange))
                {
                    Cast(AbilitySlot.Ability4);
                    return;
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability5) && AbilitySlot.Ability5.IsReady() && AbilitySlot.Ability5.InRange(closestRange))
                {
                    Cast(AbilitySlot.Ability5);
                    return;
                }

                if (MenuHandler.UseSkill(AbilitySlot.EXAbility2) && AbilitySlot.Ability2.IsReady() &&
                    LocalPlayer.Instance.Energized.Energy >= 25 && AbilitySlot.EXAbility2.InRange(closestRange) && validForBigProjectiles)
                {
                    if (!MenuHandler.SkillBool("save_a6") || LocalPlayer.Instance.Energized.Energy >= 50)
                    {
                        Cast(AbilitySlot.EXAbility2);
                        return;
                    }
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability2) && AbilitySlot.Ability2.IsReady() && AbilitySlot.EXAbility2.InRange(closestRange) &&
                    validForBigProjectiles)
                {
                    if (EnemiesInRange(5).Count == 0)
                    {
                        Cast(AbilitySlot.Ability2);
                        return;
                    }
                }

                if (MenuHandler.UseSkill(AbilitySlot.EXAbility1) && AbilitySlot.Ability1.IsReady() &&
                    LocalPlayer.Instance.Energized.Energy >= 25 && AbilitySlot.EXAbility1.InRange(closestRange) && validForProjectiles)
                {
                    if (!MenuHandler.SkillBool("save_a6") || LocalPlayer.Instance.Energized.Energy >= 50)
                    {
                        if (LocalPlayer.Instance.Living.MaxRecoveryHealth - LocalPlayer.Instance.Living.Health >= 22 &&
                            enemyTeam.All(e => e.Buffs.All(b => b.ObjectName != "SeekersMarkBuff")))
                        {
                            Cast(AbilitySlot.EXAbility1);
                            return;
                        }
                    }
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability1) && AbilitySlot.Ability1.InRange(closestRange) && validForProjectiles)
                {
                    Cast(AbilitySlot.Ability1);
                    return;
                }
            }
            Jumong.DebugOutput = "No valid targets";
        }

        private static void Cast(AbilitySlot slot)
        {
            LocalPlayer.PressAbility(slot, true);
        }

        private static List<Character> EnemiesInRange(float distance)
        {
            return EntitiesManager.EnemyTeam.Where(e => e.Distance(LocalPlayer.Instance) < distance).ToList();
        }
    }
}