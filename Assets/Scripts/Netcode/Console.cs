using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        public int numLinesToShow = 10;
        public bool isVisible = true;


        //#if !UNITY_EDITOR
        private List<string> logLines = new List<string>();
        private Vector2 scrollPos = new Vector2(0, 0);
        private int prevLogLen = 0;
        private int prevBottomScrollPos = 0;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            logLines.Add(logString);
        }

        //void OnGUI()
        //{
            
        //    GUI.Box(new Rect(0, 0, Screen.width, numLinesToShow * 20), "");

        //    Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * logLines.Count);
        //    if (prevLogLen != logLines.Count)
        //    {
        //        if (scrollPos.y == prevBottomScrollPos || prevBottomScrollPos - scrollPos.y < 20)
        //        {
        //            scrollPos = new Vector2(0, 20 * (logLines.Count - numLinesToShow) + 10);
        //        }
        //        prevLogLen = logLines.Count;
        //        prevBottomScrollPos = 20 * (logLines.Count - numLinesToShow) + 10;
        //    }

        //    scrollPos = GUI.BeginScrollView(new Rect(0, 5f, Screen.width, numLinesToShow * 20 - 10), scrollPos, viewport);

        //    for (int i = 0; i < logLines.Count; i++)
        //    {
        //        string line = logLines[i];
        //        Rect labelRect = new Rect(0, 20 * i, viewport.width, 20);
        //        GUI.Label(labelRect, line);
        //    }
        //    GUI.EndScrollView();
        //}
        //#endif
    }
}