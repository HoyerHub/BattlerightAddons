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
using Hoyer.Common;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.TargetSelection;
using Prediction = Hoyer.Common.Prediction.Prediction;

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

        private static void GetTargetAndAim(SkillBase skill)
        {
            if (OrbLogic(skill, true))
            {
                return;
            }
            var prediction = TargetSelection.GetTargetPrediction(skill, Skills.GetData(skill.Slot));

            if (!prediction.CanHit)
            {
                if (OrbLogic(skill))
                {
                    Jumong.DebugOutput = "Attacking orb (no valid targets)";
                }
                else
                {
                    LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                }
                return;
            }

            Jumong.DebugOutput = "Aiming at " + prediction.Target.CharName;
            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(prediction.CastPosition);
        }

        private static bool OrbLogic(SkillBase skill, bool shouldCheckHover = false)
        {
            if (skill.Slot == AbilitySlot.Ability4 || skill.Slot == AbilitySlot.Ability5 || skill.Slot == AbilitySlot.EXAbility1) return false;
            var orb = EntitiesManager.CenterOrb;
            if (orb == null || !orb.IsValid || !orb.IsActiveObject) return false;
            var livingObj = orb.Get<LivingObject>();
            if (livingObj.IsDead) return false;

            var orbMapObj = orb.Get<MapGameObject>();
            var orbPos = orbMapObj.Position;

            if (livingObj.Health <= 16 && skill.Slot != AbilitySlot.Ability7)
            {
                Jumong.DebugOutput = "Attacking orb (Orb Steal)";
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(orbPos);
                return true;
            }

            if (orbPos.Distance(LocalPlayer.Instance) > skill.Range ||
                shouldCheckHover && !orbMapObj.IsHoveringNear()) return false;

            if (shouldCheckHover) Jumong.DebugOutput = "Attacking orb (mouse hovering)";
            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }

        private static TargetingMode GetTargetingMode(IEnumerable<Character> possibleTargets)
        {
            if (MenuHandler.UseCursor) return TargetingMode.NearMouse;
            return possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5) ? TargetingMode.Closest : TargetingMode.LowestHealth;
        }
    }
}