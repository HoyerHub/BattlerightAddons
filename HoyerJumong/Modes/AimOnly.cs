using System;
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
            get { return !MenuHandler.HitStealthed.CurrentValue; }
        }

        public void Update()
        {
            if (!MenuHandler.AimUserInput.CurrentValue) return;

            var castingFill = LocalPlayer.Instance.CastingFill;
            int[] waitAim = {0, 1, 3};

            if (!waitAim.Includes(LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex) &&
                LocalPlayer.Instance.AbilitySystem.CastingAbilityIsCasting)
            {
                GetTargetAndAim();
            }
            else if (castingFill > 0.7 && castingFill < 1.1)
            {
                GetTargetAndAim();
            }
            else
            {
                LocalPlayer.EditAimPosition = false;
            }
        }

        private void GetTargetAndAim()
        {
            try
            {
                var castingSpell = Skills.Active.Get((AbilitySlot) LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex);
                if (castingSpell == null) return;

                if (OrbLogic(castingSpell, true)) return;
                var target = UseCursor ? GetTargetFromCursor(castingSpell) : GetTargetNoCursor(castingSpell);
                if (target == null) return;

                var prediction = target.GetPrediction(castingSpell);
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(prediction);
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

        private Character GetTargetNoCursor(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e => e != null && e.IsValidTarget(castingSpell, isProjectile, useOnIncaps, AvoidStealthed))
                .ToList();

            if (possibleTargets.Count == 0)
            {
                if (!OrbLogic(castingSpell)) LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                return null;
            }

            if (possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5))
                return TargetSelector.GetTarget(possibleTargets, TargetingMode.Closest, float.MaxValue);
            return TargetSelector.GetTarget(possibleTargets, TargetingMode.LowestHealth, float.MaxValue);
        }

        private Character GetTargetFromCursor(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var target = TargetSelector.GetTarget(TargetingMode.NearMouse);
            if (target.Distance(LocalPlayer.Instance.Aiming.AimPosition) > 3 ||
                               !target.IsValidTarget(castingSpell, isProjectile, useOnIncaps, AvoidStealthed))
            {
                var possibleTargets = EntitiesManager.EnemyTeam.Where(e => e != null && e.IsValidTarget(castingSpell, isProjectile, useOnIncaps, AvoidStealthed))
                    .ToList();
                if (possibleTargets.Count == 0)
                {
                    if (!OrbLogic(castingSpell)) LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                    return null;
                }

                target = possibleTargets.OrderBy(o => o.Distance(LocalPlayer.Instance)).First();
            }

            return target;
        }
    }
}