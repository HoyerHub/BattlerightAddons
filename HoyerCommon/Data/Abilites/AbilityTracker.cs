using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.Events;
using Hoyer.Common.Extensions;
using Hoyer.Common.Utilities;
using Vector2 = BattleRight.Core.Math.Vector2;

namespace Hoyer.Common.Data.Abilites
{
    public static class AbilityTracker
    {
        public static void Setup()
        {
            Enemy.Projectiles.Setup();
        }

        public static class Enemy
        {
            public static class Projectiles
            {
                public static readonly List<CastingProjectile> Casting = new List<CastingProjectile>();
                public static List<Projectile> Active = new List<Projectile>();

                public static void Setup()
                {
                    Game.OnUpdate += OnUpdate;
                    SpellDetector.OnSpellCast += OnSpellCast;
                }

                private static void OnSpellCast(BattleRight.SDK.EventsArgs.SpellCastArgs args)
                {
                    if (args.Caster.Id == 25) return;
                    if (args.Caster.Team != LocalPlayer.Instance.Team)
                    {
                        //Console.WriteLine(args.Caster.AbilitySystem.CastingAbilityName + ": " + args.Caster.AbilitySystem.CastingAbilityId);
                        var abilityInfo = AbilityDatabase.Get(args.Caster.AbilitySystem.CastingAbilityId);
                        if (abilityInfo != null)
                        {
                            //Console.WriteLine("AbilityDatabase: Data found for spell: " + abilityInfo.ObjectName);
                            Casting.Add(new CastingProjectile(abilityInfo, args.Caster));
                        }
                    }
                }

                private static void OnUpdate(EventArgs args)
                {
                    Active.Clear();
                    Active.AddRange(EntitiesManager.ActiveProjectiles.Where(p=>p.BaseObject.TeamId != LocalPlayer.Instance.BaseObject.TeamId));
                    CheckForCasts();
                    UpdateCasts();
                }

                private static void UpdateCasts()
                {
                    foreach (var cast in Casting)
                    {
                        cast.Update();
                    }
                }
                private static void CheckForCasts()
                {
                    var toRemove = new List<CastingProjectile>();
                    foreach (var cast in Casting)
                    {
                        if (!cast.Caster.AbilitySystem.IsCasting || cast.Caster.AbilitySystem.IsPostCasting) toRemove.Add(cast);
                    }

                    foreach (var projectile in toRemove)
                    {
                        Casting.Remove(projectile);
                    }
                }
            }
        }
        
    }

    public class CastingProjectile
    {
        public Character Caster;
        public AbilityInfo Data;
        public Vector2 EndPos;

        public CastingProjectile(AbilityInfo ability, Character caster)
        {
            Caster = caster;
            Data = ability;
            Update();
        }

        public void Update()
        {
            UpdateEndPos();
        }

        private void UpdateEndPos()
        {
            EndPos = Caster.Pos().Extend(InputManager.MousePosition, Data.Range);
        }

        public bool WillCollideWithPlayer
        {
            get
            {
                return Geometry.CircleVsThickLine(LocalPlayer.Instance.Pos(),
                    LocalPlayer.Instance.MapCollision.MapCollisionRadius, Caster.Pos(),
                    EndPos, Data.Radius + LocalPlayer.Instance.MapCollision.MapCollisionRadius + 0.1f, true);
            }
        }
    }
}