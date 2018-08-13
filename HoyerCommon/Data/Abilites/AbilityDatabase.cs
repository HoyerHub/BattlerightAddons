using System.Collections.Generic;
using System.Linq;
using BattleRight.Core.Enumeration;
using BattleRight.SDK.Enumeration;

namespace Hoyer.Common.Data.Abilites
{
    //Made by Hoyer
    //Work in progress
    public static class AbilityDatabase
    {
        public static List<AbilityInfo> Abilites = new List<AbilityInfo>();
        public static List<DodgeAbilityInfo> DodgeAbilities = new List<DodgeAbilityInfo>();

        static AbilityDatabase()
        {
            #region Abilities

            Add(new AbilityInfo
            {
                Champion = "Dummy McFuckFace",
                ObjectName = "CannonShot",
                AbilitySlot = AbilitySlot.Ability1,
                SkillType = SkillType.Line,
                ProjectileSpeed = 12,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "Splinter",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1529787171,
                SkillType = SkillType.Line,
                Range = 7.4f,
                Radius = 0.25f,
                ProjectileSpeed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "Splinter",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 385747261,
                SkillType = SkillType.Line,
                Range = 7.4f,
                Radius = 0.25f,
                ProjectileSpeed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "IceLance",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1497250588,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.3f,
                ProjectileSpeed = 22,
                Danger = 2,
                CollideCount = 1,
                ImportantBattlerites = new[]
                {
                    new Battlerite
                    {
                        Name = "Piercing Cold",
                        ChangedField = new Field("CollideCount", typeof(float)),
                        NewValue = 2
                    }
                }
            });
            Add(new AbilityInfo
            {
                Champion = "Ashka",
                ObjectName = "Fireball",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 2031258179,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.3f,
                ProjectileSpeed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ashka",
                ObjectName = "FireballSecond",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 436368160,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.3f,
                ProjectileSpeed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ashka",
                ObjectName = "FirestormProjectile",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1755790702,
                SkillType = SkillType.Line,
                Range = 9,
                Radius = 0.35f,
                ProjectileSpeed = 20.8f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Destiny",
                ObjectName = "Blaster",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1983687638,
                SkillType = SkillType.Line,
                Range = 7.2f,
                Radius = 0.3f,
                ProjectileSpeed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Destiny",
                ObjectName = "ChargedBolt",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1247213693,
                SkillType = SkillType.Line,
                RequiresCharging = true,
                MinRange = 5.2f,
                MaxRange = 10,
                Radius = 0.4f,
                ProjectileSpeed = 23.2f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Destiny",
                ObjectName = "ShieldBlaster",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1707385525,
                SkillType = SkillType.Line,
                Range = 7,
                Radius = 0.3f,
                ProjectileSpeed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ezmo",
                ObjectName = "ArcaneFire",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1090264556,
                SkillType = SkillType.Line,
                Range = 5.3f,
                Radius = 0.25f,
                ProjectileSpeed = 17.9f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ezmo",
                ObjectName = "ChaosGrip",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1087070488,
                SkillType = SkillType.Line,
                RequiresCharging = true,
                MinRange = 5.1f,
                MaxRange = 10,
                ProjectileSpeed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Iva",
                ObjectName = "RocketLauncher",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1240421213,
                SkillType = SkillType.Line,
                Range = 9,
                Radius = 0.35f,
                ProjectileSpeed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Iva",
                ObjectName = "TazerProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 100459505,
                SkillType = SkillType.Line,
                Range = 9.6f,
                Radius = 0.3f,
                ProjectileSpeed = 24,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Iva",
                ObjectName = "ConcussionShot",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 1424966216,
                SkillType = SkillType.Line,
                Range = 10,
                Radius = 0.35f,
                ProjectileSpeed = 27.75f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Gunner",
                ObjectName = "RevolverShot",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 56844512,
                SkillType = SkillType.Line,
                Range = 6.8f,
                Radius = 0.25f,
                ProjectileSpeed = 17.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Gunner",
                ObjectName = "RevolverShotSecond",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1414514524,
                SkillType = SkillType.Line,
                Range = 6.8f,
                Radius = 0.25f,
                ProjectileSpeed = 17.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Gunner",
                ObjectName = "Snipe",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 830935979,
                SkillType = SkillType.Line,
                Range = 11.5f,
                Radius = 0.4f,
                CollisionType = CollisionType.Walls,
                CollideCount = 99,
                ProjectileSpeed = 29,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Gunner",
                ObjectName = "DisablingShot",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1632681037,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.35f,
                ProjectileSpeed = 29.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Gunner",
                ObjectName = "SnapShot",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 159773418,
                SkillType = SkillType.Line,
                Range = 11.5f,
                Radius = 0.35f,
                CollisionType = CollisionType.Walls,
                ProjectileSpeed = 29.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Gunner",
                ObjectName = "ExplosiveShellsProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1014869658,
                SkillType = SkillType.Line,
                Range = 11.6f,
                Radius = 0.4f,
                ProjectileSpeed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "HuntingArrow",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1702843065,
                SkillType = SkillType.Line,
                Range = 7.9f,
                Radius = 0.25f,
                ProjectileSpeed = 17.9f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "SteadyShot",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1845720173,
                SkillType = SkillType.Line,
                Range = 10.5f,
                Radius = 0.35f,
                ProjectileSpeed = 29.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "BlackArrowProjectile",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 131957505,
                SkillType = SkillType.Line,
                Range = 7.8f,
                Radius = 0.35f,
                ProjectileSpeed = 17f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1338599461,
                SkillType = SkillType.Circle,
                Range = 10,
                Radius = 2,
                FixedDelay = 0.5f,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 508323608,
                SkillType = SkillType.Line,
                Range = 6.8f,
                Radius = 1,
                ProjectileSpeed = 12
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "GuidedArrow",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 782235039,
                SkillType = SkillType.Line,
                Range = 10.5f,
                Radius = 0.4f,
                ProjectileSpeed = 25.75f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "SeekersArrow",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 1050374556,
                SkillType = SkillType.Line,
                Range = 8.8f,
                Radius = 0.3f,
                ProjectileSpeed = 23.75f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "DragonSlayer",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 478592007,
                CollideCount = 2,
                SkillType = SkillType.Line,
                RequiresCharging = true,
                MinCastTime = 0.8f,
                MaxCastTime = 2,
                MinRange = 9.8f,
                MaxRange = 16.7f,
                Radius = 0.6f,
                ProjectileSpeed = 27.5f,
                Danger = 5,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "HandOfCorruption",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 2026099356,
                SkillType = SkillType.Line,
                Range = 7f,
                Radius = 0.25f,
                ProjectileSpeed = 15.25f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "HandOfJudgement",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 538043106,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.3f,
                ProjectileSpeed = 17.8f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "HandOfPunishment",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 462412611,
                SkillType = SkillType.Line,
                Range = 8.3f,
                Radius = 0.3f,
                ProjectileSpeed = 17.8f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Raigon",
                ObjectName = "SeismicShock",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 2039596325,
                SkillType = SkillType.Line,
                Range = 10,
                Radius = 0.35f,
                ProjectileSpeed = 18.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Raigon",
                ObjectName = "TectonicShock",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 580605940,
                SkillType = SkillType.Line,
                Range = 10,
                Radius = 0.35f,
                ProjectileSpeed = 15.25f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Jamila",
                ObjectName = "HookShotProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 600764760,
                SkillType = SkillType.Line,
                Range = 8.1f,
                Radius = 0.35f,
                ProjectileSpeed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jamila",
                ObjectName = "ShadowStar",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 127314570,
                SkillType = SkillType.Line,
                Range = 7.6f,
                Radius = 0.35f,
                ProjectileSpeed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Shifu",
                ObjectName = "Javelin",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 467552288,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.35f,
                ProjectileSpeed = 21.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Shifu",
                ObjectName = "Harpoon",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 959922252,
                SkillType = SkillType.Line,
                Range = 9f,
                Radius = 0.35f,
                ProjectileSpeed = 21.5f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Thorn",
                ObjectName = "EntanglingRootsProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 571225273,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.35f,
                ProjectileSpeed = 21.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Thorn",
                ObjectName = "DireThornsProjectile",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 503632860,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.3f,
                ProjectileSpeed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Thorn",
                ObjectName = "ThornBarrageProjectile",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 491882699,
                SkillType = SkillType.Line,
                Range = 9,
                Radius = 0.3f,
                ProjectileSpeed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Croak",
                ObjectName = "SludgeSpit",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 951999577,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.35f,
                ProjectileSpeed = 21.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Ruh Kaan",
                ObjectName = "ClawOfTheWicked",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1703342344,
                SkillType = SkillType.Line,
                Range = 7.1f,
                Radius = 0.35f,
                ProjectileSpeed = 23.2f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "BloodAxe",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 375275413,
                SkillType = SkillType.Line,
                Range = 9.3f,
                Radius = 0.35f,
                ProjectileSpeed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Freya",
                ObjectName = "StormMace",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 578492749,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.35f,
                ProjectileSpeed = 19.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "Thwack",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 659588529,
                SkillType = SkillType.Line,
                Range = 6.5f,
                Radius = 0.3f,
                ProjectileSpeed = 15.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "ThwackHeavy",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1939263245,
                SkillType = SkillType.Line,
                Range = 6.5f,
                Radius = 0.3f,
                ProjectileSpeed = 15.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "BoomBloomProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1414049618,
                SkillType = SkillType.Line,
                Range = 8.3f,
                Radius = 0.35f,
                ProjectileSpeed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "ForceOfNatureProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 6683886,
                SkillType = SkillType.Line,
                Range = 9.6f,
                Radius = 0.8f,
                ProjectileSpeed = 17.8f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "ToxicBolt",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 417414898,
                SkillType = SkillType.Line,
                Range = 7.4f,
                Radius = 0.25f,
                ProjectileSpeed = 17.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "PanicFlask",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 713746381,
                SkillType = SkillType.Line,
                Range = 6.8f,
                Radius = 0.35f,
                ProjectileSpeed = 23.75f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "DeadlyInjectionProjectile",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 551273815,
                SkillType = SkillType.Line,
                Range = 8.8f,
                Radius = 0.35f,
                ProjectileSpeed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "PetrifyDart",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 22452765,
                SkillType = SkillType.Line,
                Range = 9.8f,
                Radius = 0.35f,
                ProjectileSpeed = 23.75f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Oldur",
                ObjectName = "SandsOfTime",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1866135133,
                SkillType = SkillType.Line,
                Range = 6.5f,
                Radius = 0.35f,
                ProjectileSpeed = 15.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Oldur",
                ObjectName = "ChronoBolt",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 729343584,
                SkillType = SkillType.Line,
                Range = 10,
                Radius = 0.35f,
                ProjectileSpeed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "VolatileWaterHeavy",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 168381495,
                SkillType = SkillType.Line,
                Range = 7.2f,
                Radius = 0.25f,
                ProjectileSpeed = 18f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "VolatileWaterCharged",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 326591978,
                SkillType = SkillType.Line,
                Range = 7.6f,
                Radius = 0.25f,
                ProjectileSpeed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "MothProjectile",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1046839413,
                SkillType = SkillType.Line,
                Range = 7,
                Radius = 0.25f,
                ProjectileSpeed = 15.75f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "Bloodsucker",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1185187021,
                SkillType = SkillType.Line,
                Range = 9,
                Radius = 0.4f,
                ProjectileSpeed = 25.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "BrainBugProjectile",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 854998663,
                SkillType = SkillType.Line,
                Range = 7.3f,
                Radius = 0.35f,
                ProjectileSpeed = 24f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "ScarabProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 2048282027,
                SkillType = SkillType.Line,
                Range = 12,
                Radius = 0.45f,
                ProjectileSpeed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "ScarabProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1264199073,
                SkillType = SkillType.Line,
                Range = 12,
                Radius = 0.45f,
                ProjectileSpeed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "ScarabProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1747281246,
                SkillType = SkillType.Line,
                Range = 12,
                Radius = 0.45f,
                ProjectileSpeed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "SoulBolt",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 540689967,
                SkillType = SkillType.Line,
                Range = 6.8f,
                Radius = 0.25f,
                ProjectileSpeed = 15.75f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "SpiritGuide",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 920831585,
                SkillType = SkillType.Line,
                CanCounter = false,
                Range = 8.5f,
                Radius = 0.45f,
                ProjectileSpeed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "SoulTransfer",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 740507874,
                SkillType = SkillType.Line,
                CanCounter = false,
                Range = 9,
                Radius = 0.45f,
                ProjectileSpeed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "MalevolentSpirit",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 7667133,
                SkillType = SkillType.Line,
                Range = 9,
                Radius = 0.4f,
                ProjectileSpeed = 18f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Sirius",
                ObjectName = "CrescentGale",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1783636229,
                SkillType = SkillType.Line,
                Range = 9.5f,
                Radius = 0.5f,
                ProjectileSpeed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Ulric",
                ObjectName = "SmiteProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 550064645,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.3f,
                ProjectileSpeed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ulric",
                ObjectName = "SmiteProjectileSecond",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 771796668,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.3f,
                ProjectileSpeed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ulric",
                ObjectName = "SmiteProjectileThird",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1665205519,
                SkillType = SkillType.Line,
                Range = 8.5f,
                Radius = 0.3f,
                ProjectileSpeed = 21.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "TrickShotLeft",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1987108759,
                SkillType = SkillType.Line,
                Range = 6.5f,
                Radius = 0.35f,
                ProjectileSpeed = 15.25f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "TrickShotRight",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1987108759,
                SkillType = SkillType.Line,
                Range = 6.5f,
                Radius = 0.35f,
                ProjectileSpeed = 15.25f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "GrandConjuration",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1501923214,
                SkillType = SkillType.Line,
                Range = 10,
                Radius = 0.4f,
                ProjectileSpeed = 25f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "GrandConjurationDuplicate",
                AbilitySlot = AbilitySlot.Ability7,
                SkillType = SkillType.Line,
                Range = 10,
                Radius = 0.4f,
                ProjectileSpeed = 25.5f,
                Danger = 3,
            });

            #endregion

            #region DodgeSpells

            Add(new DodgeAbilityInfo
            {
                Champion = "Shifu",
                AbilitySlot = AbilitySlot.Ability3,
                SharedCooldown = AbilitySlot.EXAbility1,
                AbilityId = 1493003740,
                AbilityIndex = 1,
                AbilityType = DodgeAbilityType.Ghost,
                MinDanger = 3,
                Priority = 2,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Shifu",
                AbilitySlot = AbilitySlot.EXAbility1,
                SharedCooldown = AbilitySlot.Ability3,
                AbilityIndex = 2,
                AbilityId = 235612200,
                AbilityType = DodgeAbilityType.Ghost,
                UseInEvade = false,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Shifu",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1750205777,
                AbilityIndex = 4,
                AbilityType = DodgeAbilityType.Counter,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityType = DodgeAbilityType.Jump,
                MinDanger = 2,
                Priority = 1,
                CastTime = 0.2f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability6,
                AbilityType = DodgeAbilityType.Ghost,
                MinDanger = 3,
                Priority = 2,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Poloma",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 887586317,
                AbilityIndex = 2,
                AbilityType = DodgeAbilityType.Ghost,
                NeedsSelfCast = true,
                MinDanger = 3,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Rook",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1413535840,
                AbilityIndex = 9,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Gunner",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 512745910,
                AbilityIndex = 6,
                AbilityType = DodgeAbilityType.Jump,
                UsesMousePos = true,
                Range = 7,
                MinDanger = 4,
                Priority = 1,
                CastTime = 0.2f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Ruh Kaan",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 1599533417,
                AbilityIndex = 0,
                AbilityType = DodgeAbilityType.Jump,
                UsesMousePos = true,
                Range = 3.8f,
                MinDanger = 2,
                Priority = 2,
                CastTime = 0.1f
            }); 
            Add(new DodgeAbilityInfo
            {
                Champion = "Ruh Kaan",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1697456501,
                AbilityIndex = 2,
                AbilityType = DodgeAbilityType.Shield,
                UsesMousePos = true,
                MinDanger = 2,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Ruh Kaan",
                AbilitySlot = AbilitySlot.Ability6,
                AbilityId = 676298304,
                AbilityIndex = 6,
                AbilityType = DodgeAbilityType.Jump,
                UsesMousePos = true,
                Range = 7,
                MinDanger = 4,
                Priority = 3,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Bakko",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1445018825,
                AbilityIndex = 2,
                AbilityType = DodgeAbilityType.Shield,
                UsesMousePos = true,
                MinDanger = 2,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Bakko",
                AbilitySlot = AbilitySlot.Ability3,
                SharedCooldown = AbilitySlot.EXAbility1,
                AbilityId = 1353508382,
                AbilityIndex = 4,
                AbilityType = DodgeAbilityType.Jump,
                UsesMousePos = true,
                Range = 8,
                MinDanger = 4,
                Priority = 2,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Bakko",
                AbilitySlot = AbilitySlot.EXAbility1,
                SharedCooldown = AbilitySlot.Ability3,
                AbilityId = 1383890402,
                AbilityIndex = 6,
                AbilityType = DodgeAbilityType.Jump,
                UsesMousePos = true,
                UseInEvade = false,
                Range = 8,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Freya",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 98085269,
                AbilityIndex = 10,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Jamila",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 98085269,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Varesh",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1729824531,
                AbilityIndex = 4,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Destiny",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1574749481,
                AbilityIndex = 10,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Sirius",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 812246817,
                AbilityIndex = 6,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Pearl",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1032123680,
                AbilityIndex = 7,
                AbilityType = DodgeAbilityType.Counter,
                MinDanger = 1,
                Priority = 1,
                CastTime = 0.1f
            });
            #endregion
        }

        public static AbilityInfo[] Get(string champ, AbilitySlot slot)
        {
            return Abilites.Where(a => a.Champion == champ && a.AbilitySlot == slot).ToArray();
        }

        public static AbilityInfo Get(string objectName)
        {
            return Abilites.FirstOrDefault(a => a.ObjectName == objectName);
        }

        public static AbilityInfo Get(int spellId)
        {
            return Abilites.FirstOrDefault(a => a.AbilityId == spellId);
        }

        public static DodgeAbilityInfo GetDodge(string champ, AbilitySlot slot)
        {
            return DodgeAbilities.FirstOrDefault(a => a.Champion == champ && a.AbilitySlot == slot);
        }

        public static DodgeAbilityInfo[] GetDodge(string champ)
        {
            return DodgeAbilities.Where(a => a.Champion == champ).ToArray();
        }

        public static DodgeAbilityInfo GetDodge(int spellid)
        {
            return DodgeAbilities.FirstOrDefault(a => a.AbilityId == spellid);
        }

        private static void Add(DodgeAbilityInfo info)
        {
            DodgeAbilities.Add(info);
        }

        private static void Add(AbilityInfo info)
        {
            Abilites.Add(info);
        }
    }
}