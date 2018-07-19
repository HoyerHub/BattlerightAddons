using System;
using System.Collections;
using BattleRight.Core;
using UnityEngine;

namespace Hoyer.Common.Utilities
{
    public static class Component
    {
        private static HoyerComponent _activeComponent;

        public static event Action Update = delegate { };
        public static event Action Init = delegate { };

        public static void DelayAction(Action action, float seconds)
        {
            _activeComponent.DelayActionBySeconds(action,seconds);
        }

        static Component()
        {
            AddObject(null);
            Game.OnMatchStart += AddObject;
            Game.OnMatchEnd += AddObject;
            DelayAction(Component_Init, 0.5f);
        }

        private static void Component_Init()
        {
            Init.Invoke();
        }

        private static void AddObject(EventArgs args)
        {
            var gameObject = new GameObject { name = "letsgo" };
            _activeComponent = gameObject.AddComponent<HoyerComponent>();
        }

        public class HoyerComponent : MonoBehaviour
        {   
            public void DelayActionBySeconds(Action action, float seconds)
            {
                StartCoroutine(DelayingEnumerator(action, seconds));
            }

            public IEnumerator DelayingEnumerator(Action action, float seconds)
            {
                yield return new WaitForSeconds(seconds);
                action();
            }

            private void Update()
            {
                Component.Update.Invoke();
            }
        }
    }
}