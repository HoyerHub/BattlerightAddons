using System;
using System.Collections.Generic;
using BattleRight.Core;
using TMPro;
using UnityEngine;

namespace Hoyer.Common.Utilities
{
    public static class HideNames
    {
        public static bool Active;
        private static readonly List<TextMeshProUGUI> HUDLabels = new List<TextMeshProUGUI>();
        private static readonly List<TextMeshProUGUI> UILabels = new List<TextMeshProUGUI>();

        public static void Setup()
        {
            OnInit();
        }

        private static void OnInit()
        {
            Game.OnUpdate += MB_Update;
            Game.OnMatchStart += Game_OnMatchStart;
            if (Game.IsInGame) SetHudLabels();
        }

        private static void Game_OnMatchStart(EventArgs args)
        {
            SetHudLabels();
        }

        private static void SetUILabels()
        {
            UILabels.Clear();
            var uiLabels = GameObject.FindGameObjectWithTag("ViewCanvas").transform.FindChild("UICanvas").GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var label in uiLabels)
                if (label.gameObject.name == "NameText" || label.gameObject.name == "PlayerNameText")
                    UILabels.Add(label);
        }

        private static void SetHudLabels()
        {
            HUDLabels.Clear();
            var hudLabels = GameObject.FindGameObjectWithTag("ViewCanvas").transform.FindChild("HUDCanvas").GetChild(0)
                .GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var label in hudLabels)
                if (label.gameObject.name == "Name" || label.gameObject.name == "Player")
                    HUDLabels.Add(label);
        }

        private static void MB_Update(EventArgs args)
        {
            if (!Active) return;
            if (!Game.IsInGame) SetUILabels();

            foreach (var component in HUDLabels)
                if (component != null && component.enabled)
                    component.enabled = false;
            foreach (var component in UILabels)
                if (component != null && component.enabled)
                    component.enabled = false;
        }
    }
}