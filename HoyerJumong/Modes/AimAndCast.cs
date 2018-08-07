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

namespace Hoyer.Champions.Jumong.Modes
{
    public class AimAndCast:IMode
    {
        private bool _useCursor
        {
            get { return MenuHandler.JumongMenu.Get<MenuCheckBox>("jumong_usecursor").CurrentValue; }
        }

        public void Update()
        {
            if (!LocalPlayer.Instance.AbilitySystem.IsCasting || LocalPlayer.Instance.AbilitySystem.IsPostCasting)
            {
                SpellCastLogic();
            }
            else if (LocalPlayer.Instance.AbilitySystem.IsCasting)
                GetTargetAndAim();
        }

        private void SpellCastLogic()
        {
            if (EnemiesInRange(2).Count > 0)
            {
                if (AbilitySlot.Ability3.IsReady())
                {
                    Cast(AbilitySlot.Ability3);
                    return;
                }
            }

            var enemyTeam = EntitiesManager.EnemyTeam;
            var validEnemies = enemyTeam.Where(e => e.IsValidTarget()).ToList();
            if (validEnemies.Any())
            {
                var closestRange = validEnemies.OrderBy(e => e.Distance(LocalPlayer.Instance)).First().Distance(LocalPlayer.Instance);
                if (AbilitySlot.Ability4.IsReady() && AbilitySlot.Ability4.InRange(closestRange))
                {
                    Cast(AbilitySlot.Ability4);
                    return;
                }

                if (AbilitySlot.Ability5.IsReady() && AbilitySlot.Ability5.InRange(closestRange))
                {
                    Cast(AbilitySlot.Ability5);
                    return;
                }

                if (AbilitySlot.EXAbility2.IsReady() && AbilitySlot.EXAbility2.InRange(closestRange))
                {
                    Cast(AbilitySlot.EXAbility2);
                    return;
                }

                if (AbilitySlot.Ability2.IsReady() && AbilitySlot.Ability2.InRange(closestRange))
                {
                    if (EnemiesInRange(5).Count == 0)
                    {
                        Cast(AbilitySlot.Ability2);
                        return;
                    }
                }

                if (AbilitySlot.EXAbility1.IsReady() && AbilitySlot.EXAbility1.InRange(closestRange))
                {
                    if (LocalPlayer.Instance.Living.MaxRecoveryHealth - LocalPlayer.Instance.Living.Health >= 22 &&
                        enemyTeam.All(e => e.Buffs.All(b => b.ObjectName != "SeekersMarkBuff")))
                    {
                        Cast(AbilitySlot.EXAbility1);
                        return;
                    }
                }

                if (AbilitySlot.Ability1.InRange(closestRange))
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

        public void GetTargetAndAim()
        {
            try
            {
                var castingSpell = Skills.Active.Get((AbilitySlot)LocalPlayer.Instance.AbilitySystem.CastingAbilityIndex);
                if (castingSpell == null) return;

                if (OrbLogic(castingSpell.Range, true)) return;

                var target = _useCursor ? GetTargetFromCursor(castingSpell) : GetTargetNoCursor(castingSpell);
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

        private bool OrbLogic(float skillRange, bool shouldCheckHover = false)
        {
            if (EntitiesManager.CenterOrb == null || EntitiesManager.CenterOrb.Get<LivingObject>().IsDead) return false;

            var orbPos = EntitiesManager.CenterOrb.Get<MapGameObject>().Position;

            if (shouldCheckHover && !EntitiesManager.CenterOrb.Get<MapGameObject>().IsHoveringNear() ||
                !(orbPos.Distance(LocalPlayer.Instance) < skillRange)) return false;

            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }

        private Character GetTargetFromCursor(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var target = _useCursor ? TargetSelector.GetTarget(TargetingMode.NearMouse) : TargetSelector.GetTarget(TargetingMode.Closest);
            if (_useCursor && (target.Distance(LocalPlayer.Instance.Aiming.AimPosition) > 3 ||
                               !target.IsValidTarget(castingSpell, isProjectile, useOnIncaps)))
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

        private Character GetTargetNoCursor(SkillBase castingSpell)
        {
            var isProjectile = castingSpell.Slot != AbilitySlot.Ability4 && castingSpell.Slot != AbilitySlot.Ability5;
            var useOnIncaps = castingSpell.Slot == AbilitySlot.Ability2 || castingSpell.Slot == AbilitySlot.EXAbility2;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e => e != null && e.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
                .ToList();

            if (possibleTargets.Count == 0)
            {
                if (!OrbLogic(castingSpell.Range)) LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                return null;
            }

            if (possibleTargets.Any(o => o.Distance(LocalPlayer.Instance) < 5))
                return TargetSelector.GetTarget(possibleTargets, TargetingMode.Closest, float.MaxValue);
            return TargetSelector.GetTarget(possibleTargets, TargetingMode.LowestHealth, float.MaxValue);
        }
    }
}