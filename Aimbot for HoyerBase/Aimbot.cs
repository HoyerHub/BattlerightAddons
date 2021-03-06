﻿using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Sandbox;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.Events;
using Hoyer.Base.Data.Abilites;
using Hoyer.Base.Data.Addons;
using Hoyer.Base.Extensions;
using Hoyer.Base.MathUtils;
using Hoyer.Base.Menus;
using Hoyer.Base.Prediction;

namespace Hoyer.Base.Aimbot
{
    public class Aimbot:IAddon
    {
        private static bool _shouldUse;
        private static bool _isCasting;
        private static int _castingId;

        public void OnUnload()
        {
            Game.OnMatchStart -= Game_OnMatchStart;
            Game.OnMatchEnd -= OnMatchEnd;
            SpellDetector.OnSpellCast -= SpellDetector_OnSpellCast;
            SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
            Game.OnUpdate -= Update;
            MenuEvents.Initialize -= MenuHandler.Setup;
            MenuHandler.Unload();
        }

        public void OnInit()
        {
            Game.OnMatchStart += Game_OnMatchStart;
            Game.OnMatchEnd += OnMatchEnd;
            SpellDetector.OnSpellCast += SpellDetector_OnSpellCast;
            SpellDetector.OnSpellStopCast += SpellDetector_OnSpellStopCast;
            Game.OnUpdate += Update;
            Game.OnPreUpdate += Game_OnPreUpdate;
            MenuEvents.Initialize += MenuHandler.Setup;
        }

        private void Game_OnPreUpdate(EventArgs args)
        {
            LocalPlayer.EditAimPosition = false;
        }

        private static void OnMatchEnd(EventArgs args)
        {
            MenuHandler.AimbotMenu.Hidden = false;
        }

        private static void SpellDetector_OnSpellStopCast(BattleRight.SDK.EventsArgs.SpellStopArgs args)
        {
            if (args.Caster.Name != LocalPlayer.Instance.Name) return;
            _isCasting = false;
            _castingId = 0;
            if (_shouldUse) LocalPlayer.EditAimPosition = false;
        }

        private static void SpellDetector_OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
        {
            if (args.Caster.Name != LocalPlayer.Instance.Name) return;
            _isCasting = true;
            _castingId = args.Caster.AbilitySystem.CastingAbilityId;
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            _shouldUse = false;
            Main.DelayAction(delegate
            {
                try
                {
                    var addons = AddonMenus.Active.Where(a => a.SupportedCharacters.Contains(LocalPlayer.Instance.CharName)).ToArray();
                    _shouldUse = !addons.Any();
                    if (_shouldUse) ActiveSkills.AddFromDatabase();
                    MenuHandler.AimbotMenu.Hidden = !_shouldUse;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }, 0.2f);
        }

        private static void Update(EventArgs args)
        {
            if (!MenuHandler.Enabled || !_shouldUse || !_isCasting || Game.CurrentMatchState != MatchState.InRound) return;
            var skill = ActiveSkills.Get(_castingId);
            if (skill == null)
            {
                return;
            }

            if (MenuHandler.Get(skill.Slot))
            {
                if (OrbLogic(skill, true)) return;
                var pred = TargetSelection.GetTargetPrediction(skill, AbilityDatabase.Get(_castingId));
                if (!pred.CanHit && !OrbLogic(skill))
                {
                    if (MenuHandler.Interrupt) LocalPlayer.PressAbility(AbilitySlot.Interrupt, true);
                    LocalPlayer.EditAimPosition = false;
                    return;
                }

                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(pred.CastPosition);
            }
        }

        private static bool OrbLogic(SkillBase skill, bool shouldCheckHover = false)
        {
            var orb = EntitiesManager.CenterOrb;
            if (orb == null || !orb.IsValid || !orb.IsActiveObject) return false;
            var orbLiving = orb.Get<LivingObject>();
            if (orbLiving.IsDead) return false;

            var orbMapObj = orb.Get<MapGameObject>();
            var orbPos = orbMapObj.Position;
            if (!TargetSelection.CursorDistCheck(orbPos)) return false;
            if (orbLiving.Health <= 16 && skill.Slot != AbilitySlot.Ability7)
            {
                LocalPlayer.EditAimPosition = true;
                LocalPlayer.Aim(orbPos);
                return true;
            }

            if (orbPos.Distance(LocalPlayer.Instance) > skill.Range || 
                shouldCheckHover && !orbMapObj.IsHoveringNear()) return false;

            if (skill.SkillType == SkillType.Line && Prediction.Prediction.UseClosestPointOnLine)
                orbPos = GeometryLib.NearestPointOnFiniteLine(LocalPlayer.Instance.Pos().Extend(orbPos, 0.6f),
                    orbPos, Main.MouseWorldPos);
            LocalPlayer.EditAimPosition = true;
            LocalPlayer.Aim(orbPos);
            return true;
        }
    }
}