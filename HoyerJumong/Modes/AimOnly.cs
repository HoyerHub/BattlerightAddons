using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Prediction = Hoyer.Common.Prediction;

namespace Hoyer.Champions.Jumong.Modes
{
    public class AimOnly : IMode
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
            if (!MenuHandler.AimUserInput.CurrentValue || !LocalPlayer.Instance.AbilitySystem.CastingAbilityIsCasting)
            {
                LocalPlayer.EditAimPosition = false;
                return;
            }

            var castingFill = LocalPlayer.Instance.CastingFill;
            var skill = Skills.Get(LocalPlayer.Instance.AbilitySystem.CastingAbilityId);
            if (skill == null) return;

            AbilitySlot[] waitAim = {AbilitySlot.Ability1, AbilitySlot.Ability2, AbilitySlot.Ability4};

            if (!waitAim.Includes(skill.Slot))
            {
                GetTargetAndAim(skill);
            }
            else if (castingFill > 0.7 && castingFill < 1.1)
            {
                GetTargetAndAim(skill);
            }
        }

        private void GetTargetAndAim(SkillBase skill)
        {
            try
            {
                if (OrbLogic(skill, true)) return;
                var prediction = GetTargetPrediction(skill);

                if (prediction == Prediction.Output.None && !OrbLogic(skill))
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

        private bool OrbLogic(SkillBase skill, bool checkHover = false)
        {
            if (EntitiesManager.CenterOrb == null || EntitiesManager.CenterOrb.Get<LivingObject>().IsDead) return false;
            if (skill.Slot == AbilitySlot.Ability4 || skill.Slot == AbilitySlot.Ability5 || skill.Slot == AbilitySlot.EXAbility1) return false;

            var orbPos = EntitiesManager.CenterOrb.Get<MapGameObject>().Position;

            if (checkHover && !EntitiesManager.CenterOrb.Get<MapGameObject>().IsHoveringNear() ||
                !(orbPos.Distance(LocalPlayer.Instance) < skill.Range)) return false;

            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }

        private Prediction.Output GetTargetPrediction(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e => e != null && !e.Living.IsDead && e.Distance(LocalPlayer.Instance) < castingSpell.Range)
                .ToList();

            var output = Prediction.Output.None;

            while (possibleTargets.Count > 0 && output.CanHit == false)
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
            return possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5) ? 
                TargetingMode.Closest : TargetingMode.LowestHealth;
        }
    }
}