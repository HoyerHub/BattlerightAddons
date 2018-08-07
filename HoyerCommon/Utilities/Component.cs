using System;
using System.Collections;
using BattleRight.Core;
using UnityEngine;
using Object = UnityEngine.Object;

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

        public static void Setup()
        {
            AddObject();
            DelayAction(Component_Init, 0.5f);
        }

        private static void Component_Init()
        {
            Console.WriteLine("[HoyerCommon/Component] Init");
            Init.Invoke();
        }

        private static void AddObject()
        {
            var gameObject = new GameObject { name = "HoyerWasHere" };
            _activeComponent = gameObject.AddComponent<HoyerComponent>();
            gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("ViewCanvas").transform);
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