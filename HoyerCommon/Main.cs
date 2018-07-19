using System;
using BattleRight.Sandbox;
using UnityEngine;

namespace Hoyer.Common
{
    public class Main:IAddon
    {
        public void OnInit()
        {
            Application.logMessageReceived += delegate (string condition, string trace, LogType type)
            {
                if (type != LogType.Log) Console.WriteLine(condition + Environment.NewLine + trace);
            };
        }

        public void OnUnload()
        {
        }
    }
}