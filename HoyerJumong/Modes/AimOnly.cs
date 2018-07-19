using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;

namespace Hoyer.Champions.Jumong.Modes
{
    public class AimOnly : IMode
    {
        public void Update()
        {
            var castingFill = LocalPlayer.Instance.CastingFill;
            int[] waitAim = { 0,1,3 };
            if ((castingFill > 0.7 && castingFill < 1.1 && waitAim.Includes(LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex)) 
                || LocalPlayer.Instance.AbilitySystem.IsCasting)
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
                var castingSpell = Skills.Active.Get((AbilitySlot)LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex);
                if (castingSpell == null) return;

                if (OrbLogic(castingSpell.Range, true)) return;
                var target = GetTarget(castingSpell);
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

        private bool OrbLogic(float skillRange, bool checkHover = false)
        {
            if (EntitiesManager.CenterOrb == null || EntitiesManager.CenterOrb.Get<LivingObject>().IsDead) return false;

            var orbPos = EntitiesManager.CenterOrb.Get<MapGameObject>().Position;

            if (checkHover && !EntitiesManager.CenterOrb.Get<MapGameObject>().IsHoveringNear() ||
                !(orbPos.Distance(LocalPlayer.Instance) < skillRange)) return false;

            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }

        private Character GetTarget(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var target = TargetSelector.GetTarget(TargetingMode.NearMouse);

            if (target.Distance(LocalPlayer.Instance.Aiming.AimPosition) > 3 || !target.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
            {
                var possibleTargets = EntitiesManager.EnemyTeam.Where(e => e != null && e.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
                    .ToList();
                if (possibleTargets.Count == 0)
                {
                    if (!OrbLogic(castingSpell.Range)) LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                    return null;
                }

                target = possibleTargets.OrderBy(o => o.Distance(LocalPlayer.Instance)).First();
            }

            return target;
        }
    }
}