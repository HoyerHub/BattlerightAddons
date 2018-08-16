using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Prediction = Hoyer.Common.Prediction;

namespace Hoyer.Champions.Jumong.Modes
{
    public static class AimAndCast 
    {
        public static void Update()
        {
            if (!LocalPlayer.Instance.AbilitySystem.IsCasting || LocalPlayer.Instance.AbilitySystem.IsPostCasting)
            {
                SpellCastLogic();
            }
            else if (LocalPlayer.Instance.AbilitySystem.IsCasting)
            {
                var skill = Skills.Get(LocalPlayer.Instance.AbilitySystem.CastingAbilityId);
                if (skill == null) return;
                GetTargetAndAim(skill);
            }
        }

        private static void SpellCastLogic()
        {
            if (MenuHandler.SkillBool("close_a3") && EnemiesInRange(2).Count > 0)
            {
                if (AbilitySlot.Ability3.IsReady())
                {
                    Cast(AbilitySlot.Ability3);
                    return;
                }
            }

            var enemyTeam = EntitiesManager.EnemyTeam;
            var validEnemies = enemyTeam.Where(e => e.IsValidTarget()).ToList();
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
                }
            }
        }

        private static void Cast(AbilitySlot slot)
        {
            LocalPlayer.PressAbility(slot, true);
        }

        private static List<Character> EnemiesInRange(float distance)
        {
            return EntitiesManager.EnemyTeam.Where(e => e.Distance(LocalPlayer.Instance) < distance).ToList();
        }

        private static void GetTargetAndAim(SkillBase skill)
        {
            if (OrbLogic(skill, true)) return;
                var prediction = GetTargetPrediction(skill);

            if (!prediction.CanHit && !OrbLogic(skill))
            {
                LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                    return;
                }

                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(prediction.CastPosition);
        }

        private static bool OrbLogic(SkillBase skill, bool shouldCheckHover = false)
        {
            if (EntitiesManager.CenterOrb == null) return false;
            var orbLiving = EntitiesManager.CenterOrb.Get<LivingObject>();
            if (orbLiving.IsDead) return false;

            if (skill.Slot == AbilitySlot.Ability4 || skill.Slot == AbilitySlot.Ability5 || skill.Slot == AbilitySlot.EXAbility1) return false;
            
            var orbMapObj = EntitiesManager.CenterOrb.Get<MapGameObject>();
            var orbPos = orbMapObj.Position;
            if (orbLiving.Health <= 16 && skill.Slot != AbilitySlot.Ability7)
            {
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(orbPos);
                return true;
            }

            if (shouldCheckHover && !orbMapObj.IsHoveringNear() ||
                !(orbPos.Distance(LocalPlayer.Instance) < skill.Range)) return false;

            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }

        private static Prediction.Output GetTargetPrediction(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var possibleTargets = EntitiesManager.EnemyTeam
                .Where(e => e != null && !e.Living.IsDead && e.Pos() != Vector2.Zero && e.Distance(LocalPlayer.Instance) < castingSpell.Range * Prediction.CancelRangeModifier)
                .ToList();

            var output = Prediction.Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                Character tryGetTarget = null;
                tryGetTarget = TargetSelector.GetTarget(possibleTargets, GetTargetingMode(possibleTargets), float.MaxValue);
                if (castingSpell.Slot == AbilitySlot.Ability4)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        output = Prediction.Basic(tryGetTarget, castingSpell);
                        output.CanHit = true;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps, MenuHandler.AvoidStealthed))
                {
                    var pred = tryGetTarget.GetPrediction(castingSpell);
                    if (pred.CanHit)
                    {
                        output = pred;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else
                {
                    possibleTargets.Remove(tryGetTarget);
                }
            }
            return output;
        }

        private static TargetingMode GetTargetingMode(IEnumerable<Character> possibleTargets)
        {
            if (MenuHandler.UseCursor) return TargetingMode.NearMouse;
            return possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5) ? TargetingMode.Closest : TargetingMode.LowestHealth;
        }
    }
}