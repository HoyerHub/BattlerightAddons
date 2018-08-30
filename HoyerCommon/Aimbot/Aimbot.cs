using System;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.Core.GameObjects.Models;
using BattleRight.Core.Math;
using BattleRight.SDK;
using BattleRight.SDK.Enumeration;
using BattleRight.SDK.Events;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;
using BattleRight.SDK.UI.Values;
using Hoyer.Common.Data.Abilites;
using Hoyer.Common.Data.Addons;
using Hoyer.Common.Extensions;
using Hoyer.Common.Local;

namespace Hoyer.Common.AimBot
{
    public static class Aimbot
    {
        private static bool _shouldUse;
        private static bool _isCasting;
        private static int _castingId;

        public static void Unload()
        {
            Game.OnMatchStateUpdate -= Game_OnMatchStart;
            Game.OnMatchEnd -= OnMatchEnd;
            SpellDetector.OnSpellCast -= SpellDetector_OnSpellCast;
            SpellDetector.OnSpellStopCast -= SpellDetector_OnSpellStopCast;
            Game.OnUpdate -= Update;
            MenuEvents.Initialize -= MenuHandler.Setup;
            MenuHandler.Unload();
        }

        public static void Setup()
        {
            Game.OnMatchStart += Game_OnMatchStart;
            Game.OnMatchEnd += OnMatchEnd;
            SpellDetector.OnSpellCast += SpellDetector_OnSpellCast;
            SpellDetector.OnSpellStopCast += SpellDetector_OnSpellStopCast;
            Game.OnUpdate += Update;
            MenuEvents.Initialize += MenuHandler.Setup;
        }

        private static void OnMatchEnd(EventArgs args)
        {
            MenuHandler.AimbotMenu.Hidden = false;
        }

        private static void SpellDetector_OnSpellStopCast(BattleRight.SDK.EventsArgs.SpellStopArgs args)
        {
            if (!MenuHandler.Enabled || args.Caster.Name != LocalPlayer.Instance.Name) return;
            _isCasting = false;
            _castingId = 0;
            if (_shouldUse) LocalPlayer.EditAimPosition = false;
        }

        private static void SpellDetector_OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
        {
            if (!MenuHandler.Enabled || args.Caster.Name != LocalPlayer.Instance.Name) return;
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
                    if (!_shouldUse)
                    {
                        //var addon = addons[0];
                        //Console.WriteLine("[HoyerCommon/Aimbot] Found " + addon.Creator + "'s " + addon.Name + ". Disabling Aimbot for this match");
                    }
                    else
                    {
                        Skills.AddFromDatabase();
                        //Console.WriteLine("[HoyerCommon/Aimbot] Found " + Skills.Active.Count + " abilities for " +
                        //                  LocalPlayer.Instance.CharName.ToCharacterString() + " in the database");
                    }
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
            var skill = Skills.Get(_castingId);
            if (skill == null)
            {
                return;
            }

            if (MenuHandler.Get(skill.Slot))
            {
                if (OrbLogic(skill, true)) return;
                var pred = GetTargetPrediction(skill, AbilityDatabase.Get(_castingId));
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
            if (EntitiesManager.CenterOrb == null) return false;
            var orbLiving = EntitiesManager.CenterOrb.Get<LivingObject>();
            if (orbLiving.IsDead) return false;

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

        private static Prediction.Output GetTargetPrediction(SkillBase castingSpell, AbilityInfo data)
        {
            var isProjectile = data.AbilityType == AbilityType.LineProjectile;
            var useOnIncaps = data.Danger >= 3;

            var possibleTargets = EntitiesManager.EnemyTeam.Where(e =>
                    e != null && !e.Living.IsDead && e.Pos() != Vector2.Zero &&
                    e.Distance(LocalPlayer.Instance) < castingSpell.Range * Prediction.CancelRangeModifier)
                .ToList();

            var output = Prediction.Output.None;

            while (possibleTargets.Count > 0 && !output.CanHit)
            {
                Character tryGetTarget = null;
                tryGetTarget = TargetSelector.GetTarget(possibleTargets, TargetingMode.NearMouse, float.MaxValue);
                if (data.AbilityType == AbilityType.CircleThrowObject || data.AbilityType == AbilityType.CircleJump)
                {
                    if (tryGetTarget.IsValidTarget())
                    {
                        output = Prediction.Basic(tryGetTarget, castingSpell);
                        output.CanHit = true;
                    }
                    else
                    {
                        possibleTargets.Remove(tryGetTarget);
                    }
                }
                else if (tryGetTarget.IsValidTarget(castingSpell, isProjectile, useOnIncaps))
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
    }
}