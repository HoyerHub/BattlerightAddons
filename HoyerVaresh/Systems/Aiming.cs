using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using Hoyer.Common;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;
using Hoyer.Common.Prediction;
using Hoyer.Common.Utilities;
using static Hoyer.Common.Prediction.Prediction;
using Prediction = Hoyer.Common.Prediction.Prediction;

namespace Hoyer.Champions.Varesh.Systems
{
    public static class Aiming
    {
        public static void GetTargetAndAim()
        {
            var castingId = LocalPlayer.Instance.AbilitySystem.CastingAbilityId;
            if (castingId == 238454699 && MenuHandler.SkillBool("close_a3") && EnemiesInRange(2.5f).Count > 0)
            {
                Varesh.DebugOutput = "Shielding self";
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(LocalPlayer.Instance.Pos());
                return;
            }
            var skill = Skills.Get(castingId);
            if (skill == null) return;
            if (OrbLogic(skill, true)) return;
            var prediction = skill.Slot == AbilitySlot.Ability5 ?
                GetEPrediction(skill, Skills.GetData(skill.Slot)) :
                TargetSelection.GetTargetPrediction(skill, Skills.GetData(skill.Slot), skill.Slot == AbilitySlot.Ability2);
            if (!prediction.CanHit)
            {
                if (OrbLogic(skill))
                {
                    Varesh.DebugOutput = "Attacking orb (no valid targets)";
                }
                else
                {
                    if (MenuHandler.InterruptSpells && (!MenuHandler.NeverInterruptE || skill.Slot != AbilitySlot.Ability5))
                    LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                }
                return;
            }

            Varesh.DebugOutput = "Aiming at " + prediction.Target.CharName;
            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(prediction.CastPosition);
        }

        private static bool OrbLogic(SkillBase skill, bool shouldCheckHover = false)
        {
            var orb = EntitiesManager.CenterOrb;
            if (orb == null || !orb.IsValid || !orb.IsActiveObject) return false;
            var livingObj = orb.Get<LivingObject>();
            if (livingObj.IsDead) return false;

            var orbMapObj = orb.Get<MapGameObject>();
            var orbPos = orbMapObj.Position;

            if (!TargetSelection.CursorDistCheck(orbPos)) return false;

            if (livingObj.Health <= 14 && skill.Slot == AbilitySlot.Ability1)
            {
                Varesh.DebugOutput = "Attacking orb (Orb Steal)";
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(orbPos);
                return true;
            }
            if (livingObj.Health <= 22 && skill.Slot == AbilitySlot.Ability2)
            {
                Varesh.DebugOutput = "Attacking orb (Orb Steal)";
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(orbPos);
                return true;
            }
            if (orbPos.Distance(LocalPlayer.Instance) > skill.Range ||
                shouldCheckHover && !orbMapObj.IsHoveringNear()) return false;

            if (shouldCheckHover) Varesh.DebugOutput = "Attacking orb (mouse hovering)";
            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }

        private static Output GetEPrediction(SkillBase castingSpell, AbilityInfo data)
        {
            var possibleTargets = EntitiesManager.EnemyTeam.Where(e =>
                    e != null && !e.Living.IsDead && e.Pos().Distance(Vector2.Zero) > 0.1f &&
                    e.Distance(LocalPlayer.Instance) < castingSpell.Range * CancelRangeModifier)
                .ToList();
            var output = Output.None;

            var actualTargets = new List<Tuple<Character, int>>();
            foreach (var character in possibleTargets)
            {
                var a = 0;
                foreach (var buff in character.Buffs)
                {
                    if (buff.ObjectName == "HandOfJudgementBuff") a += 2;
                    else if (buff.ObjectName == "HandOfCorruptionBuff") a++;
                }
                if (a > 0) actualTargets.Add(Tuple.New(character, a));
            }

            if (!actualTargets.Any()) return output;

            possibleTargets = actualTargets.OrderByDescending(a => a.Second).Select(a=>a.First).ToList();

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                var tryGetTarget = possibleTargets[0];
                if (data.AbilityType == AbilityType.CircleThrowObject || data.AbilityType == AbilityType.CircleJump)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        var pred = tryGetTarget.GetPrediction(castingSpell);
                        if (pred.CanHit && (TargetSelection.CursorDistCheck(pred.CastPosition) || TargetSelection.CursorDistCheck(tryGetTarget.Pos())))
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
                else
                {
                    possibleTargets.Remove(tryGetTarget);
                }
            }

            return output;
        }

        private static List<Character> EnemiesInRange(float distance)
        {
            return EntitiesManager.EnemyTeam.Where(e => e.Distance(LocalPlayer.Instance) < distance).ToList();
        }

        public static void AimUlt()
        {
            var skill = Skills.Active.Get(AbilitySlot.Ability7);
            if (OrbLogic(skill, true)) return;
            var prediction = TargetSelection.GetTargetPrediction(skill, Skills.GetData(skill.Slot), true);
            if (!prediction.CanHit)
            {
                Varesh.DebugOutput = OrbLogic(skill) ? "Attacking orb (no valid targets)" : "Cant find any targets";
                return;
            }
            Varesh.DebugOutput = "Aiming at " + prediction.Target.CharName;
            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(prediction.CastPosition);
        }
    }
}