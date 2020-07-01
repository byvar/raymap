using OpenSpace;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{

    public class PointerTraceWindow : EditorWindow
    {

        Vector2 scrollPos = new Vector2();
        string searchString = "";
        private List<KeyValuePair<Pointer, string>> results;
        private bool searched = false;
        private int resultCount = 0;
        private static int resultLimit = 1000;

        [MenuItem("Raymap/Trace Pointers")]
        public static void Init()
        {
            PointerTraceWindow window = (PointerTraceWindow)EditorWindow.GetWindow(typeof(PointerTraceWindow));
            window.titleContent = new GUIContent("Trace Pointers");
            window.Show();
        }

        void OnGUI()
        {
            if (!UnitySettings.TracePointers)
            {
                EditorGUILayout.HelpBox("To use this window, enable \"Trace Pointers\" in the settings window.", MessageType.Info);
                return;
            }
            if (!EditorApplication.isPlaying || Settings.s == null)
            {
                EditorGUILayout.HelpBox("Please start the scene to use this window.", MessageType.Info);
                return;
            }

            searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("Search"))
            {
                results = MapLoader.Loader.pointerTraces.Where(k => k.Key.ToString().Contains(searchString)).ToList();
                resultCount = results.Count;
                if (resultCount> resultLimit)
                {
                    results = results.GetRange(0, resultLimit);
                }
                searched = true;
            }

            if (!searched)
            {
                EditorGUILayout.HelpBox("Please search for a pointer first.", MessageType.Info);
                return;
            }

            if (results == null)
            {
                EditorGUILayout.HelpBox("No results found.", MessageType.Info);
                return;
            }

            if (resultCount>resultLimit)
            {
                EditorGUILayout.HelpBox($"Limiting results to {resultLimit}/{resultCount}", MessageType.Info);
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (var pair in results)
            {
                GUILayout.Label($"Pointer {pair.Key}", EditorStyles.boldLabel);
                GUILayout.Label($"Read at {pair.Value}");
            }

            GUILayout.EndScrollView();
        }
    }
}