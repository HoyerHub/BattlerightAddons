using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;

namespace Hoyer.Champions.Varesh.Systems
{
    public static class Casting
    {
        public static void CastLogic()
        {
            if (MenuHandler.SkillBool("close_a3") && EnemiesInRange(2.5f).Count > 0)
            {
                if (AbilitySlot.Ability3.IsReady())
                {
                    Cast(AbilitySlot.Ability3);
                    return;
                }
            }

            var enemyTeam = EntitiesManager.EnemyTeam;
            var validEnemies = enemyTeam.Where(e => e.IsValidTarget() && e.Pos().Distance(Vector2.Zero) > 0.3f).ToList();

            if (validEnemies.Any())
            {
                var anyValidForProjectiles = validEnemies.Any(e => e.IsValidTargetProjectile());
                var anyValidForBigProjectiles = validEnemies.Any(e => e.IsValidTargetProjectile(true));
                bool anyWithCorruption, anyWithJudgement;
                Varesh.BuffCheck(validEnemies, out anyWithCorruption, out anyWithJudgement);

                var closestRange = validEnemies.OrderBy(e => e.Distance(LocalPlayer.Instance)).First().Distance(LocalPlayer.Instance);

                if (MenuHandler.UseSkill(AbilitySlot.Ability5) && AbilitySlot.Ability5.IsReadyHasCharges() && AbilitySlot.Ability5.InRange(closestRange) && 
                    anyWithJudgement)
                {
                    Cast(AbilitySlot.Ability5);
                    return;
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability7) && AbilitySlot.Ability7.IsReady() && AbilitySlot.Ability7.InRange(closestRange))
                {
                    Cast(AbilitySlot.Ability7);
                    return;
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability2) && AbilitySlot.Ability2.IsReady() && AbilitySlot.Ability2.InRange(closestRange) &&
                    anyValidForBigProjectiles)
                {
                    if (EnemiesInRange(4).Count == 0)
                    {
                        Cast(AbilitySlot.Ability2);
                        return;
                    }
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability5) && AbilitySlot.Ability5.IsReadyHasCharges(2) && AbilitySlot.Ability5.InRange(closestRange) &&
                    anyWithCorruption)
                {
                    Cast(AbilitySlot.Ability5);
                    return;
                }

                if (MenuHandler.UseSkill(AbilitySlot.EXAbility1) && AbilitySlot.Ability2.IsReady() &&
                    LocalPlayer.Instance.Energized.Energy >= 25 && AbilitySlot.EXAbility1.InRange(closestRange) && anyValidForBigProjectiles && !anyWithJudgement)
                {
                    if (!MenuHandler.SkillBool("save_a6") || LocalPlayer.Instance.Energized.Energy >= 50)
                    {
                        Cast(AbilitySlot.EXAbility1);
                        return;
                    }
                }
                if (MenuHandler.UseSkill(AbilitySlot.Ability1) && AbilitySlot.Ability1.InRange(closestRange) && anyValidForProjectiles)
                {
                    Cast(AbilitySlot.Ability1);
                    return;
                }
                Varesh.DebugOutput = "No valid targets";
            }
        }

        private static List<Character> EnemiesInRange(float distance)
        {
            return EntitiesManager.EnemyTeam.Where(e => e.Distance(LocalPlayer.Instance) < distance).ToList();
        }

        private static void Cast(AbilitySlot slot)
        {
            LocalPlayer.PressAbility(slot, true);
        }
    }
}