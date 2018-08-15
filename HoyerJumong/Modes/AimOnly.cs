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
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Prediction = Hoyer.Common.Prediction;

namespace Hoyer.Champions.Jumong.Modes
{
    public static class AimOnly
    {
        private static bool UseCursor
        {
            get { return MenuHandler.UseCursor.CurrentValue; }
        }

        private static bool AvoidStealthed
        {
            get { return MenuHandler.AvoidStealthed.CurrentValue; }
        }

        public static void Update()
        {
            if (!MenuHandler.AimUserInput.CurrentValue || !LocalPlayer.Instance.AbilitySystem.CastingAbilityIsCasting)
            {
                LocalPlayer.EditAimPosition = false;

                if (MenuHandler.SkillBool("close_a3") && EntitiesManager.EnemyTeam.Where(e => e.Distance(LocalPlayer.Instance) < 2).ToList().Count > 0)
                {
                    if (AbilitySlot.Ability3.IsReady())
                    {
                        LocalPlayer.PressAbility(AbilitySlot.Ability3, true);
                        return;
                    }
                }
                return;
            }

            var castingFill = LocalPlayer.Instance.CastingFill;
            var skill = Skills.Get(LocalPlayer.Instance.AbilitySystem.CastingAbilityId);
            if (skill == null) return;

            AbilitySlot[] waitAim = {AbilitySlot.Ability1, AbilitySlot.Ability2, AbilitySlot.EXAbility2};

            if (!waitAim.Includes(skill.Slot))
            {
                GetTargetAndAim(skill);
            }
            else if (castingFill > 0.7 && castingFill < 1.1)
            {
                GetTargetAndAim(skill);
            }
        }

        private static void GetTargetAndAim(SkillBase skill)
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

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e => e != null && !e.Living.IsDead && e.Pos() != Vector2.Zero && e.Distance(LocalPlayer.Instance) < castingSpell.Range * Prediction.CancelRangeModifier)
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
                }
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps, AvoidStealthed))
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
            if (UseCursor) return TargetingMode.NearMouse;
            return possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5) ? 
                TargetingMode.Closest : TargetingMode.LowestHealth;
        }
    }
}