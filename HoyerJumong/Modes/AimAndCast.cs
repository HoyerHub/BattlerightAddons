using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Prediction = Hoyer.Common.Prediction;

namespace Hoyer.Champions.Jumong.Modes
{
    public class AimAndCast : IMode
    {
        private bool UseCursor
        {
            get { return MenuHandler.UseCursor.CurrentValue; }
        }

        private bool AvoidStealthed
        {
            get { return MenuHandler.AvoidStealthed.CurrentValue; }
        }

        public void Update()
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

        private void SpellCastLogic()
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
            var validForProjectiles = enemyTeam.Any(e => e.IsValidTargetProjectile());
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

                if (MenuHandler.UseSkill(AbilitySlot.EXAbility2) && AbilitySlot.EXAbility2.IsReady() &&
                    AbilitySlot.EXAbility2.InRange(closestRange) && validForProjectiles)
                {
                    if (!MenuHandler.SkillBool("save_a6") || LocalPlayer.Instance.Energized.Energy >= 50)
                    {
                        Cast(AbilitySlot.EXAbility2);
                        return;
                    }
                }

                if (MenuHandler.UseSkill(AbilitySlot.Ability2) && AbilitySlot.Ability2.IsReady() && AbilitySlot.Ability2.InRange(closestRange) &&
                    validForProjectiles)
                {
                    if (EnemiesInRange(5).Count == 0)
                    {
                        Cast(AbilitySlot.Ability2);
                        return;
                    }
                }

                if (MenuHandler.UseSkill(AbilitySlot.EXAbility1) && AbilitySlot.EXAbility1.IsReady() &&
                    AbilitySlot.EXAbility1.InRange(closestRange) && validForProjectiles)
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

        private void Cast(AbilitySlot slot)
        {
            LocalPlayer.PressAbility(slot, true);
        }

        private List<Character> EnemiesInRange(float distance)
        {
            return EntitiesManager.EnemyTeam.Where(e => e.Distance(LocalPlayer.Instance) < distance).ToList();
        }

        private void GetTargetAndAim(SkillBase skill)
        {
            try
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
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private bool OrbLogic(SkillBase skill, bool shouldCheckHover = false)
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

        private Prediction.Output GetTargetPrediction(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var possibleTargets = EntitiesManager.EnemyTeam
                .Where(e => e != null && !e.Living.IsDead && e.Distance(LocalPlayer.Instance) < castingSpell.Range)
                .ToList();

            var output = Prediction.Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                Character tryGetTarget = null;
                tryGetTarget = TargetSelector.GetTarget(possibleTargets, GetTargetingMode(possibleTargets), float.MaxValue);
                if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps, AvoidStealthed))
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

        private TargetingMode GetTargetingMode(IEnumerable<Character> possibleTargets)
        {
            if (UseCursor) return TargetingMode.NearMouse;
            return possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5) ? TargetingMode.Closest : TargetingMode.LowestHealth;
        }
    }
}