using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.Math;
using BattleRight.SDK;
using Hoyer.Common.Extensions;

namespace Hoyer.Champions.Varesh.Systems
{
    public static class Casting
    {
        private static void FindValidSpell()
        {
            var enemyTeam = EntitiesManager.EnemyTeam;
            var validEnemies = enemyTeam.Where(e => e.IsValidTarget() && e.Pos().Distance(Vector2.Zero) > 0.3f).ToList();
            var validForProjectiles = validEnemies.Any(e => e.IsValidTargetProjectile());
            var validForBigProjectiles = validEnemies.Any(e => e.IsValidTargetProjectile(true));
            if (validEnemies.Any())
            {
                var closestRange = validEnemies.OrderBy(e => e.Distance(LocalPlayer.Instance)).First().Distance(LocalPlayer.Instance);

                if (MenuHandler.UseSkill(AbilitySlot.Ability2) && AbilitySlot.Ability2.IsReady() && AbilitySlot.EXAbility2.InRange(closestRange) &&
                    validForBigProjectiles)
                {
                    if (EnemiesInRange(4).Count == 0)
                    {
                        Cast(AbilitySlot.Ability2);
                        return;
                    }
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
                if (MenuHandler.UseSkill(AbilitySlot.Ability1) && AbilitySlot.Ability1.InRange(closestRange) && validForProjectiles)
                {
                    Cast(AbilitySlot.Ability1);
                }
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