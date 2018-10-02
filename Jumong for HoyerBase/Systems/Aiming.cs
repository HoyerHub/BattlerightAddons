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
using Hoyer.Base.Extensions;
using Hoyer.Base.Local;
using Hoyer.Base.Prediction;
using Hoyer.Base.Utilities.Geometry;
using Prediction = Hoyer.Base.Prediction.Prediction;

namespace Hoyer.Champions.Jumong.Modes
{
    public static class Aiming
    {
        public static void GetTargetAndAim()
        {
            var castingId = LocalPlayer.Instance.AbilitySystem.CastingAbilityId;
            var skill = Skills.Get(castingId);
            if (skill == null) return;
            if (OrbLogic(skill, true)) return;

            var prediction = TargetSelection.GetTargetPrediction(skill, Skills.GetData(skill.Slot));

            if (!prediction.CanHit)
            {
                if (OrbLogic(skill))
                {
                    Main.DebugOutput = "Attacking orb (no valid targets)";
                }
                else if (MenuHandler.InterruptSpells)
                {
                    LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                }
                return;
            }

            Main.DebugOutput = "Aiming at " + prediction.Target.CharName;
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
            
            if (!TargetSelection.CursorDistCheck(orbPos)) return false;
            if (livingObj.Health <= 16 && skill.Slot != AbilitySlot.Ability7)
            {
                Main.DebugOutput = "Attacking orb (Orb Steal)";
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(orbPos);
                return true;
            }

            if (orbPos.Distance(LocalPlayer.Instance) > skill.Range ||
                shouldCheckHover && !orbMapObj.IsHoveringNear()) return false;

            if (shouldCheckHover) Main.DebugOutput = "Attacking orb (mouse hovering)";
            if (skill.SkillType == SkillType.Line && Prediction.UseClosestPointOnLine)
                orbPos = GeometryLib.NearestPointOnFiniteLine(LocalPlayer.Instance.Pos().Extend(orbPos, 0.6f),
                orbPos, Base.Main.MouseWorldPos);
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