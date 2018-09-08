using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.Core.Enumeration;
using BattleRight.Core.GameObjects;

namespace Hoyer.Common.Trackers
{
    public static class BuffTracker
    {
        public static Dictionary<string, CharacterBuffState> CharacterBuffStates = new Dictionary<string, CharacterBuffState>();

        public static void Setup()
        {
            Game.OnMatchStart += Game_OnMatchStart;
            Game.OnMatchStateUpdate += OnMatchStateUpdate;
            Character.OnBuffGain += Character_OnBuffGain;
            Character.OnBuffRemove += Character_OnBuffRemove;
        }

        private static void OnMatchStateUpdate(MatchStateUpdate args)
        {
            foreach (var buffState in CharacterBuffStates.Values)
            {
                buffState.Clear();
            }
        }

        public static void Unload()
        {
            Game.OnMatchStart -= Game_OnMatchStart;
            Game.OnMatchStateUpdate -= OnMatchStateUpdate;
            Character.OnBuffGain -= Character_OnBuffGain;
            Character.OnBuffRemove -= Character_OnBuffRemove;
        }

        private static void Character_OnBuffGain(BuffEventArgs args)
        {
            var target = args.Buff.Target;
            if (target == null || !(target is Character)) return;

            var targetChar = (Character) target;
            if (!CharacterBuffStates.ContainsKey(targetChar.Name)) return;

            CharacterBuffStates[targetChar.Name].AddBuff(args.Buff);
        }

        private static void Character_OnBuffRemove(BuffEventArgs args)
        {
            var target = args.Buff.Target;
            if (target == null || !(target is Character)) return;

            var targetChar = (Character)target;
            if (!CharacterBuffStates.ContainsKey(targetChar.Name)) return;

            CharacterBuffStates[targetChar.Name].RemoveBuff(args.Buff);
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            CharacterBuffStates.Clear();
            foreach (var character in EntitiesManager.EnemyTeam)
            {
                CharacterBuffStates.Add(character.Name, new CharacterBuffState(character));
            }
        }
    }
    
    public class CharacterBuffState
    {
        public Character Character;
        public bool CrowdControlled;
        public bool SafeFromProjectiles;
        private readonly List<Buff> _activeBuffs = new List<Buff>();

        public CharacterBuffState(Character character)
        {
            Character = character;
            CrowdControlled = false;
            SafeFromProjectiles = false;
        }

        public void Clear()
        {
            _activeBuffs.Clear();
            CrowdControlled = false;
            SafeFromProjectiles = false;
        }

        public void AddBuff(Buff buff)
        {
            _activeBuffs.Add(buff);
            if (!SafeFromProjectiles && (buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" ||
                buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam" || buff.ObjectName == "TimeBenderBuff" || buff.ObjectName == "DivineShieldBuff"))
            {
                SafeFromProjectiles = true;
            }
            if (!CrowdControlled && BuffIsCrowdControl(buff))
            {
                CrowdControlled = true;
            }
        }

        public void RemoveBuff(Buff buff)
        {
            _activeBuffs.Remove(buff);
            if (SafeFromProjectiles && BuffIsAntiProjectile(buff))
            {
                var keepSafe = false;
                foreach (var activeBuff in _activeBuffs)
                {
                    if (BuffIsAntiProjectile(activeBuff)) keepSafe = true;
                }
                SafeFromProjectiles = keepSafe;
            }
            else if (CrowdControlled && BuffIsCrowdControl(buff))
            {
                var keepCc = false;
                foreach (var activeBuff in _activeBuffs)
                {
                    if (BuffIsCrowdControl(activeBuff)) keepCc = true;
                }
                CrowdControlled = keepCc;
            }
        }

        private static bool BuffIsAntiProjectile(Buff buff)
        {
            return buff.BuffType == BuffType.Counter || buff.BuffType == BuffType.Consume || buff.ObjectName == "GustBuff" ||
                   buff.ObjectName == "BulwarkBuff" || buff.ObjectName == "TractorBeam" || buff.ObjectName == "TimeBenderBuff" ||
                   buff.ObjectName == "DivineShieldBuff";
        }
        
        private static bool BuffIsCrowdControl(Buff buff)
        {
            return buff.ObjectName == "Incapacitate" || buff.ObjectName == "PetrifyStone";
        }
    }
}