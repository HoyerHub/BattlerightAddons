using System;
using System.Collections.Generic;
using System.Linq;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;
using BattleRight.SDK.Enumeration;

namespace Hoyer.Common.Data.Abilites
{
    //Made by Hoyer
    //Work in progress
    public static class AbilityDatabase
    {
        public static List<AbilityInfo> Abilities = new List<AbilityInfo>();
        public static List<DodgeAbilityInfo> DodgeAbilities = new List<DodgeAbilityInfo>();
        public static List<ObstacleAbilityInfo> ObstacleAbilities = new List<ObstacleAbilityInfo>();

        public static void Unload()
        {
            Abilities.Clear();
            DodgeAbilities.Clear();
            ObstacleAbilities.Clear();
        }

        public static void Setup()
        {
            #region Abilities
            Add(new AbilityInfo
            {
                Champion = "Dummy McFuckFace",
                ObjectName = "CannonShot",
                Range = 7.4f,
                Radius = 0.3f,
                AbilitySlot = AbilitySlot.Ability1,
                AbilityType = AbilityType.LineProjectile,
                Speed = 12,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "Splinter",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1529787171,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.4f,
                Radius = 0.25f,
                Speed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "Splinter",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 385747261,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.4f,
                Radius = 0.25f,
                Speed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "IceLance",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1497250588,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.3f,
                Speed = 22,
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
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.3f,
                Speed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ashka",
                ObjectName = "FireballSecond",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 436368160,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.3f,
                Speed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ashka",
                ObjectName = "FireStormProjectile",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1755790702,
                AbilityType = AbilityType.LineProjectile,
                Range = 9,
                Radius = 0.35f,
                Speed = 20.8f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Ashka",
                ObjectName = "VolcanoEruption",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityType = AbilityType.CircleThrowObject,
                Range = 9,
                Radius = 2,
                FixedDelay = 1.1f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Destiny",
                ObjectName = "Blaster",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1983687638,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.2f,
                Radius = 0.3f,
                Speed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Destiny",
                ObjectName = "ChargedBolt",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1247213693,
                AbilityType = AbilityType.LineProjectile,
                RequiresCharging = true,
                MinRange = 5.2f,
                MaxRange = 10,
                Radius = 0.4f,
                Speed = 23.2f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Destiny",
                ObjectName = "ShieldBlaster",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1707385525,
                AbilityType = AbilityType.LineProjectile,
                Range = 7,
                Radius = 0.3f,
                Speed = 15.8f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ezmo",
                ObjectName = "ArcaneFire",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1090264556,
                AbilityType = AbilityType.LineProjectile,
                Range = 5.3f,
                Radius = 0.25f,
                Speed = 17.9f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ezmo",
                ObjectName = "ChaosGrip",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1087070488,
                AbilityType = AbilityType.LineProjectile,
                RequiresCharging = true,
                MinRange = 5.1f,
                MaxRange = 10,
                Speed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Iva",
                ObjectName = "RocketLauncher",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1240421213,
                AbilityType = AbilityType.LineProjectile,
                Range = 9,
                Radius = 0.35f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Iva",
                ObjectName = "TazerProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 100459505,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.6f,
                Radius = 0.3f,
                Speed = 24,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Iva",
                ObjectName = "ConcussionShot",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 1424966216,
                AbilityType = AbilityType.LineProjectile,
                Range = 10,
                Radius = 0.35f,
                Speed = 27.75f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Jade",
                ObjectName = "RevolverShot",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 56844512,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.8f,
                Radius = 0.25f,
                Speed = 17.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jade",
                ObjectName = "RevolverShotSecond",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1414514524,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.8f,
                Radius = 0.25f,
                Speed = 17.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jade",
                ObjectName = "Snipe",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 830935979,
                AbilityType = AbilityType.LineProjectile,
                Range = 11.5f,
                Radius = 0.4f,
                CollisionType = CollisionType.Walls,
                CollideCount = 99,
                Speed = 29,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Jade",
                ObjectName = "DisablingShot",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1632681037,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.35f,
                Speed = 29.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jade",
                ObjectName = "SnapShot",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 159773418,
                AbilityType = AbilityType.LineProjectile,
                Range = 11.5f,
                Radius = 0.35f,
                CollisionType = CollisionType.Walls,
                Speed = 29.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jade",
                ObjectName = "ExplosiveShellsProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1014869658,
                AbilityType = AbilityType.LineProjectile,
                Range = 11.6f,
                Radius = 0.4f,
                Speed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "HuntingArrow",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1702843065,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.9f,
                Radius = 0.25f,
                Speed = 17.9f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "SteadyShot",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1845720173,
                AbilityType = AbilityType.LineProjectile,
                Range = 10.5f,
                Radius = 0.35f,
                Speed = 29.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "BlackArrowProjectile",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 131957505,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.8f,
                Radius = 0.35f,
                Speed = 17f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1338599461,
                AbilityType = AbilityType.CircleThrowObject,
                Range = 10,
                Radius = 2,
                FixedDelay = 0.5f,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 508323608,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.8f,
                Radius = 1,
                Speed = 12
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "GuidedArrow",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 782235039,
                AbilityType = AbilityType.LineProjectile,
                Range = 10.5f,
                Radius = 0.4f,
                Speed = 25.75f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "SeekersArrow",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 1050374556,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.8f,
                Radius = 0.3f,
                Speed = 23.75f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Jumong",
                ObjectName = "DragonSlayer",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 478592007,
                CollideCount = 2,
                AbilityType = AbilityType.LineProjectile,
                RequiresCharging = true,
                MinCastTime = 0.8f,
                MaxCastTime = 2,
                MinRange = 9.8f,
                MaxRange = 16.7f,
                Radius = 0.6f,
                Speed = 27.5f,
                Danger = 5,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "RazorBoomerang",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1604073547,
                AbilityType = AbilityType.CurveProjectile,
                Range = 7.9f,
                Radius = 0.25f,
                Speed = 18,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "RazorBoomerangHaste",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 197673012,
                AbilityType = AbilityType.CurveProjectile,
                Range = 7.9f,
                Radius = 0.25f,
                Speed = 18,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeBoomerangRight",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 373835089,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 10,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeBoomerangLeft",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 373835089,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 10,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeEmpoweredBoomerangRight",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 16252536,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 10,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeEmpoweredBoomerangLeft",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 16252536,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 10,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeThrowingTechniqueBoomerangRight",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 833668998,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 11,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeThrowingTechniqueBoomerangLeft",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 833668998,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 11,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeEmpoweredThrowingTechniqueBoomerangRight",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 37801162,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 11,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "XStrikeEmpoweredThrowingTechniqueBoomerangLeft",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 37801162,
                AbilityType = AbilityType.CurveProjectile,
                MinRange = 5.8f,
                MaxRange = 11,
                Radius = 0.5f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Taya",
                ObjectName = "SpinningBoomerang",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1333545630,
                AbilityType = AbilityType.CurveProjectile,
                Range = 11.5f,
                Radius = 0.25f,
                Speed = 18,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "HandOfCorruption",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 2026099356,
                AbilityType = AbilityType.LineProjectile,
                Range = 7f,
                Radius = 0.25f,
                Speed = 15.25f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "HandOfJudgement",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 538043106,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.3f,
                Speed = 17.8f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "HandOfPunishment",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 462412611,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.3f,
                Radius = 0.3f,
                Speed = 17.8f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "Shatter",
                AbilityId = 1302893033,
                AbilitySlot = AbilitySlot.Ability5,
                AbilityType = AbilityType.CircleThrowObject,
                Range = 9,
                Radius = 1.8f,
                FixedDelay = 0.7f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "Shatter",
                AbilityId = 792310466,
                AbilitySlot = AbilitySlot.Ability5,
                AbilityType = AbilityType.CircleThrowObject,
                Range = 9,
                Radius = 1.8f,
                FixedDelay = 0.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "CombinedPowersFly1",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityType = AbilityType.Dash,
                AbilityId = 1633833490,
                Range = 6.75f,
                Radius = 0.6f,
                CanCounter = false,
                Speed = 17,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Varesh",
                ObjectName = "CombinedPowersFly2",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityType = AbilityType.Dash,
                AbilityId = 1633833490,
                StartDelay = 0.6f,
                Range = 6.75f,
                Radius = 0.6f,
                CanCounter = false,
                Speed = 17,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Rook",
                ObjectName = "BoulderTossThrow",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityType = AbilityType.CircleThrowObject,
                Range = 10,
                Radius = 2,
                FixedDelay = 0.9f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Rook",
                ObjectName = "Rush",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityType = AbilityType.Dash,
                AbilityId = 1077458428,
                Range = 11.5f,
                Radius = 0.8f,
                Speed = 20,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Raigon",
                ObjectName = "SeismicShock",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 2039596325,
                AbilityType = AbilityType.LineProjectile,
                Range = 10,
                Radius = 0.35f,
                Speed = 18.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Raigon",
                ObjectName = "TectonicShock",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 580605940,
                AbilityType = AbilityType.LineProjectile,
                Range = 10,
                Radius = 0.35f,
                Speed = 15.25f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Jamila",
                ObjectName = "HookShotProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 600764760,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.1f,
                Radius = 0.35f,
                Speed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Jamila",
                ObjectName = "ShadowStar",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 127314570,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.6f,
                Radius = 0.35f,
                Speed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Shifu",
                ObjectName = "Javelin",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 467552288,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.35f,
                Speed = 21.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Shifu",
                ObjectName = "Harpoon",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 959922252,
                AbilityType = AbilityType.LineProjectile,
                Range = 9f,
                Radius = 0.35f,
                Speed = 21.5f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Thorn",
                ObjectName = "EntanglingRootsProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 571225273,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.35f,
                Speed = 21.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Thorn",
                ObjectName = "DireThornsProjectile",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 503632860,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.3f,
                Speed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Thorn",
                ObjectName = "ThornBarrageProjectile",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 491882699,
                AbilityType = AbilityType.LineProjectile,
                Range = 9,
                Radius = 0.3f,
                Speed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Croak",
                ObjectName = "SludgeSpit",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 951999577,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.35f,
                Speed = 21.5f,
                Danger = 3, 
            });
            Add(new AbilityInfo
            {
                Champion = "Ruh Kaan",
                ObjectName = "ClawOfTheWicked",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1703342344,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.1f,
                Radius = 0.35f,
                Speed = 23.2f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Ruh Kaan",
                ObjectName = "ShadowBolt",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 921344682,
                AbilityType = AbilityType.LineProjectile,
                Range = 11f,
                Radius = 0.35f,
                Speed = 25f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Ruh Kaan",
                ObjectName = "ShadowClaw",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1826789489,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.6f,
                Radius = 0.35f,
                Speed = 23.75f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "WarAxe",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 713928362,
                AbilityType = AbilityType.Melee,
                Range = 2.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "WarAxeSecond",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1029450594,
                AbilityType = AbilityType.Melee,
                Range = 2.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "BloodAxe",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 375275413,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.3f,
                Radius = 0.35f,
                Speed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "ValiantLeap",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 1353508382,
                AbilityType = AbilityType.CircleJump,
                Range = 8,
                Radius = 2.3f,
                FixedDelay = 1f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "WarStomp",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1383890402,
                AbilityType = AbilityType.CircleJump,
                Range = 8,
                Radius = 2.3f,
                FixedDelay = 0.8f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "ExtendedShieldDash",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1860143224,
                AbilityType = AbilityType.Dash,
                Range = 6,
                Speed = 12f,
                Radius = 0.6f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "ShieldDash",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1546404495,
                AbilityType = AbilityType.Dash,
                Range = 4.8f,
                Radius = 0.6f,
                Speed = 9.6f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Bakko",
                ObjectName = "HeroicCharge",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 16920816,
                AbilityType = AbilityType.Dash,
                CanCounter = false,
                Range = 14,
                Radius = 1f,
                Speed = 11.6f,
                Danger = 5,
            });
            Add(new AbilityInfo
            {
                Champion = "Freya",
                ObjectName = "StormMace",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 578492749,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.35f,
                Speed = 19.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Freya",
                ObjectName = "LightningStrikeLeap",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 773505836,
                AbilityType = AbilityType.CircleJump,
                Range = 7,
                Radius = 2.8f,
                FixedDelay = 1,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "Thwack",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 659588529,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.5f,
                Radius = 0.3f,
                Speed = 15.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "ThwackHeavy",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1939263245,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.5f,
                Radius = 0.3f,
                Speed = 15.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "BoomBloomProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1414049618,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.3f,
                Radius = 0.35f,
                Speed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Blossom",
                ObjectName = "ForceOfNatureProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 6683886,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.6f,
                Radius = 0.8f,
                Speed = 17.8f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "ToxicBolt",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 417414898,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.4f,
                Radius = 0.25f,
                Speed = 17.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "PanicFlask",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 713746381,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.8f,
                Radius = 0.35f,
                Speed = 23.75f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "DeadlyInjectionProjectile",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 551273815,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.8f,
                Radius = 0.35f,
                Speed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Lucie",
                ObjectName = "PetrifyDart",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 22452765,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.8f,
                Radius = 0.35f,
                Speed = 23.75f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Oldur",
                ObjectName = "SandsOfTime",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1866135133,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.5f,
                Radius = 0.35f,
                Speed = 15.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Oldur",
                ObjectName = "ChronoBolt",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 729343584,
                AbilityType = AbilityType.LineProjectile,
                Range = 10,
                Radius = 0.35f,
                Speed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "VolatileWaterHeavy",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 168381495,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.2f,
                Radius = 0.25f,
                Speed = 18f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "VolatileWaterCharged",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 326591978,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.6f,
                Radius = 0.25f,
                Speed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "MothProjectile",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1046839413,
                AbilityType = AbilityType.LineProjectile,
                Range = 7,
                Radius = 0.25f,
                Speed = 15.75f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "Bloodsucker",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1185187021,
                AbilityType = AbilityType.LineProjectile,
                Range = 9,
                Radius = 0.4f,
                Speed = 25.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "BrainBugProjectile",
                AbilitySlot = AbilitySlot.EXAbility2,
                AbilityId = 854998663,
                AbilityType = AbilityType.LineProjectile,
                Range = 7.3f,
                Radius = 0.35f,
                Speed = 24f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "ScarabProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 2048282027,
                AbilityType = AbilityType.LineProjectile,
                Range = 12,
                Radius = 0.45f,
                Speed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "ScarabProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1264199073,
                AbilityType = AbilityType.LineProjectile,
                Range = 12,
                Radius = 0.45f,
                Speed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Pestilus",
                ObjectName = "ScarabProjectile",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityId = 1747281246,
                AbilityType = AbilityType.LineProjectile,
                Range = 12,
                Radius = 0.45f,
                Speed = 24f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "SoulBolt",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 540689967,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.8f,
                Radius = 0.25f,
                Speed = 15.75f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "SpiritGuide",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 920831585,
                AbilityType = AbilityType.LineProjectile,
                CanCounter = false,
                Range = 8.5f,
                Radius = 0.45f,
                Speed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "SoulTransfer",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 740507874,
                AbilityType = AbilityType.LineProjectile,
                CanCounter = false,
                Range = 9,
                Radius = 0.45f,
                Speed = 23.5f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Poloma",
                ObjectName = "MalevolentSpirit",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 7667133,
                AbilityType = AbilityType.LineProjectile,
                Range = 9,
                Radius = 0.4f,
                Speed = 18f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Sirius",
                ObjectName = "CrescentGale",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1783636229,
                AbilityType = AbilityType.LineProjectile,
                Range = 9.5f,
                Radius = 0.5f,
                Speed = 23.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Sirius",
                ObjectName = "LunarStrike",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityType = AbilityType.CircleThrowObject,
                Range = 9,
                Radius = 2,
                FixedDelay = 0.9f,
                Danger = 4,
            });
            Add(new AbilityInfo
            {
                Champion = "Ulric",
                ObjectName = "SmiteProjectile",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 550064645,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.3f,
                Speed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ulric",
                ObjectName = "SmiteProjectileSecond",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 771796668,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.3f,
                Speed = 21.5f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Ulric",
                ObjectName = "SmiteProjectileThird",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 1665205519,
                AbilityType = AbilityType.LineProjectile,
                Range = 8.5f,
                Radius = 0.3f,
                Speed = 21.5f,
                Danger = 2,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "TrickShotLeft",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1987108759,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.5f,
                Radius = 0.35f,
                Speed = 15.25f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "TrickShotRight",
                AbilitySlot = AbilitySlot.Ability1,
                AbilityId = 1987108759,
                AbilityType = AbilityType.LineProjectile,
                Range = 6.5f,
                Radius = 0.35f,
                Speed = 15.25f,
                Danger = 1,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "GrandConjuration",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1501923214,
                AbilityType = AbilityType.LineProjectile,
                Range = 10,
                Radius = 0.4f,
                Speed = 25f,
                Danger = 3,
            });
            Add(new AbilityInfo
            {
                Champion = "Zander",
                ObjectName = "GrandConjurationDuplicate",
                AbilitySlot = AbilitySlot.Ability7,
                AbilityType = AbilityType.LineProjectile,
                Range = 10,
                Radius = 0.4f,
                Speed = 25.5f,
                Danger = 3,
            });

            #endregion

            #region DodgeSpells

            Add(new DodgeAbilityInfo
            {
                Champion = "Taya",
                AbilitySlot = AbilitySlot.Ability4,
                SharedCooldown = AbilitySlot.EXAbility2,
                AbilityId = 1600053270,
                AbilityIndex = 10,
                AbilityType = DodgeAbilityType.KnockAway,
                Range = 1,
                MinDanger = 2,
                Priority = 1,
                CastTime = 0.1f
            });
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
                Priority = 2,
                CastTime = 0.2f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Jumong",
                AbilitySlot = AbilitySlot.Ability6,
                AbilityType = DodgeAbilityType.Ghost,
                MinDanger = 4,
                Priority = 1,
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
                Champion = "Rook",
                AbilitySlot = AbilitySlot.Ability2,
                AbilityId = 1397275147,
                AbilityIndex = 2,
                AbilityType = DodgeAbilityType.Jump,
                Range = 0,
                MinDanger = 3,
                Priority = 2,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Jade",
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
                Champion = "Bakko",
                AbilitySlot = AbilitySlot.Ability6,
                AbilityId = 1283963825,
                AbilityIndex = 13,
                AbilityType = DodgeAbilityType.AoEHealthShield,
                Range = 3f,
                CastTime = 0.15f
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
                Champion = "Raigon",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 690503554,
                AbilityIndex = 7,
                AbilityType = DodgeAbilityType.Shield,
                UsesMousePos = false,
                MinDanger = 2,
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
                MinDanger = 2,
                Priority = 1,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Varesh",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 238454699,
                AbilityIndex = 3,
                AbilityType = DodgeAbilityType.HealthShield,
                NeedsSelfCast = true,
                MinDanger = 2,
                Priority = 3,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Varesh",
                AbilitySlot = AbilitySlot.Ability6,
                AbilityId = 1939428505,
                AbilityIndex = 8,
                AbilityType = DodgeAbilityType.Jump,
                Range = 7,
                MinDanger = 3,
                Priority = 2,
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
            /*  The idea is fine, but it doesn't really catch the spells, they just go straight through at this point...
                It will probably work when I get pre-cast countering
            Add(new DodgeAbilityInfo
            {
                Champion = "Destiny",
                AbilitySlot = AbilitySlot.Ability5,
                AbilityId = 2029650450,
                AbilityIndex = 2,
                AbilityType = DodgeAbilityType.Obstacle,
                MinDanger = 2,
                Priority = 2,
                CastTime = 0.3f
            });*/
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
            Add(new DodgeAbilityInfo
            {
                Champion = "Alysia",
                AbilitySlot = AbilitySlot.Ability4,
                AbilityId = 1086412191,
                AbilityIndex = 7,
                AbilityType = DodgeAbilityType.HealthShield,
                NeedsSelfCast = true,
                MinDanger = 2,
                Priority = 3,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Alysia",
                AbilitySlot = AbilitySlot.Ability3,
                AbilityId = 2022133500,
                AbilityIndex = 6,
                AbilityType = DodgeAbilityType.Jump,
                UsesMousePos = true,
                Range = 9,
                MinDanger = 3,
                Priority = 2,
                CastTime = 0.1f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Alysia",
                AbilitySlot = AbilitySlot.Ability6,
                AbilityId = 612469406,
                AbilityIndex = 12,
                AbilityType = DodgeAbilityType.Shield,
                UsesMousePos = true,
                Range = 7.5f,
                MinDanger = 3,
                Priority = 1,
                CastTime = 0.2f
            });
            Add(new DodgeAbilityInfo
            {
                Champion = "Alysia",
                AbilitySlot = AbilitySlot.EXAbility1,
                AbilityId = 1600213570,
                AbilityIndex = 8,
                AbilityType = DodgeAbilityType.HealthShield,
                NeedsSelfCast = true,
                MinDanger = 4,
                Priority = 4,
                CastTime = 0.1f
            });
            #endregion

            #region Obstacles
            Add(new ObstacleAbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "BubbleBarrierArea",
                Radius = 2
            });
            Add(new ObstacleAbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "OceanSageWaterBarrierArea",
                Radius = 1.5f
            });
            Add(new ObstacleAbilityInfo
            {
                Champion = "Pearl",
                ObjectName = "UnstableBubbleArea",
                Radius = 2
            });
            Add(new ObstacleAbilityInfo
            {
                Champion = "Oldur",
                ObjectName = "ChronofluxArea",
                Radius = 2
            });
            Add(new ObstacleAbilityInfo
            {
                Champion = "Oldur",
                ObjectName = "ChronofluxAreaLesser",
                Radius = 1.1f
            });
            Add(new ObstacleAbilityInfo
            {
                Champion = "Alysia",
                ObjectName = "PolarBarrierProjectileAlt",
                Radius = 1
            });

            #endregion
        }
        public static AbilityInfo[] Get(string champ, AbilitySlot slot)
        {
            return Abilities.Where(a => a.Champion == champ && a.AbilitySlot == slot).ToArray();
        }

        public static AbilityInfo Get(string objectName)
        {
            return Abilities.FirstOrDefault(a => a.ObjectName == objectName);
        }

        public static AbilityInfo Get(int spellId)
        {
            return Abilities.FirstOrDefault(a => a.AbilityId == spellId);
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
        public static ObstacleAbilityInfo GetObstacle(string objectName)
        {
            return ObstacleAbilities.FirstOrDefault(a => a.ObjectName == objectName);
        }
        private static void Add(DodgeAbilityInfo info)
        {
            DodgeAbilities.Add(info);
        }
        private static void Add(ObstacleAbilityInfo info)
        {
            ObstacleAbilities.Add(info);
        }
        private static void Add(AbilityInfo info)
        {
            Abilities.Add(info);
        }
    }
}