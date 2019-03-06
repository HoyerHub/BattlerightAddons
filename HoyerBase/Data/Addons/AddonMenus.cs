using System;
using System.Collections.Generic;
using BattleRight.Core;
using BattleRight.SDK.UI;
using BattleRight.SDK.UI.Models;

namespace Hoyer.Base.Data.Addons
{
    public class AddonMenu
    {
        public string Creator;
        public string Name;
        public string[] MenuPath;
        public string[] SupportedCharacters;

        public AddonMenu(string creator, string name, string[] menuPath, string[] supportedCharacters)
        {
            Creator = creator;
            Name = name;
            MenuPath = menuPath;
            SupportedCharacters = supportedCharacters;
        }
    }

    public static class AddonMenus
    {
        private static List<AddonMenu> Menus;
        public static List<AddonMenu> Active;

        public static void Unload()
        {
            Menus = null;
            Active = null;
            Game.OnMatchStart -= Game_OnMatchStart;
        }

        public static void Setup()
        {
            Menus = new List<AddonMenu>();
            Active = new List<AddonMenu>();
            Menus.AddRange(new[]
            {
                new AddonMenu("Hoyer", "Jumong", new []{"Hoyer.MainMenu", "HoyerJumong"}, new []{"Jumong"}),
                new AddonMenu("Hoyer", "Varesh", new []{"Hoyer.MainMenu", "HoyerVaresh"}, new []{"Varesh"}),
                new AddonMenu("DaPip", "PipJade", new []{"pipjademenu"}, new []{"Jade"}),
                new AddonMenu("DaPip", "PipAshka", new []{"pipashkamenu"}, new []{"Ashka"}),
                new AddonMenu("DaPip", "PipKaan", new []{"pipkaanmenu"}, new []{"Ruh Kaan"}),
                new AddonMenu("NotKappa", "Kappa Poloma", new []{"kappa.Poloma"}, new []{"Poloma"}),
                new AddonMenu("Perplexity", "Perplexed Series", new []{"perplexed_series"}, new []{"Rook", "Alysia"})

            });
            Game.OnMatchStart += Game_OnMatchStart;
        }

        private static void Game_OnMatchStart()
        {
            SetActiveMenus();
        }

        private static void SetActiveMenus()
        {
            Active.Clear();
            foreach (var menu in Menus)
            {
                Menu current = null;
                foreach (var item in menu.MenuPath)
                {
                    current = current == null ? MainMenu.GetMenu(item) : current.Get<Menu>(item);
                    if (current == null) break;
                }
                if (current == null) continue;
                Active.Add(menu);
            }
        }
    }
}