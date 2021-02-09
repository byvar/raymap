using OpenSpace;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor {
    class ReferencesMenu {
        [MenuItem("GameObject/Raymap/Show References", false, 0)]
        static void Init()
        {
            GameObject a = Selection.activeGameObject;
            if (a!=null) {
                var references = a.GetComponentsInChildren<IReferenceable>().Select(r => r.References).ToList();
                
                references.ForEach(i => Debug.Log(i.referencedByDsgMems.Count));

                var dsgMemReferences = references.SelectMany(r => r.referencedByDsgMems);
                var scriptReferences = references.SelectMany(r => r.referencedByNodes);

                var persoBehaviours = MapLoader.Loader.persos.Select(p => p.Gao.GetComponent<PersoBehaviour>());
                var scriptComponents = MapLoader.Loader.scriptComponents.OfType<ScriptComponent>();

                var dsgMemPersos = persoBehaviours.Where(p => dsgMemReferences.Contains(p.perso?.brain?.mind?.dsgMem)).Select(p => p.perso);
                var scriptList = scriptComponents.Where(p => p?.script?.scriptNodes?.Intersect(scriptReferences).Any() ?? false);

                ReferencesResultWindow.Init();

                ReferencesResultWindow window = (ReferencesResultWindow)EditorWindow.GetWindow(typeof(ReferencesResultWindow));
                window.hasResults = true;
                window.searchTarget = a;
                window.dsgMemPersosResults = dsgMemPersos.ToList();
                window.scriptResults = scriptList.ToList();
            }
        }
    }

    public class ReferencesResultWindow : EditorWindow {

        public List<Perso> dsgMemPersosResults = new List<Perso>();
        public List<ScriptComponent> scriptResults = new List<ScriptComponent>();
        public GameObject searchTarget = null;
        public bool hasResults = false;
        public Vector2 scrollPosition1 = Vector2.zero;
        public Vector2 scrollPosition2 = Vector2.zero;
        private float headerHeight = 50;

        [MenuItem("Raymap/Find References")]
        public static void Init()
        {
            ReferencesResultWindow window = (ReferencesResultWindow)EditorWindow.GetWindow(typeof(ReferencesResultWindow));
            window.titleContent = new GUIContent("Find References");
            window.Show();
        }

        void OnGUI()
        {
            if (!EditorApplication.isPlaying || Settings.s == null) {
                EditorGUILayout.HelpBox("Please start the scene to use this window.", MessageType.Info);
                hasResults = false;
                return;
            }

            if (hasResults) {

                GUILayout.BeginHorizontal(GUILayout.Height(headerHeight));
                if (GUILayout.Button($"Find references to {searchTarget.name}", EditorStyles.linkLabel)) {
                    EditorGUIUtility.PingObject(searchTarget);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();
                ShowReferencesFromDsgVars();
                ShowReferencesFromScripts();
                GUILayout.EndVertical();

            } else {
                EditorGUILayout.HelpBox("Right click an object in the hierarchy and select\nRaymap > Show References to find references.", MessageType.Info);
            }
        }

        private void ShowReferencesFromScripts()
        {
            if (scriptResults.Count == 0) {
                EditorGUILayout.HelpBox("No references from Scripts", MessageType.Info);
                return;
            }
            GUILayout.Label($"References from Scripts:");

            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, GUILayout.MinHeight((this.position.height / 2) - headerHeight), GUILayout.ExpandHeight(true));

            scriptResults.ForEach(s =>
            {
                GUILayout.BeginHorizontal();
                if (s?.script?.behaviorOrMacro != null) {
                    GUILayout.Label(s.script.behaviorOrMacro.ToString());
                } else {
                    GUILayout.Label("Script at offset "+s.script.offset);
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Go")) {
                    EditorGUIUtility.PingObject(s.gameObject);
                }
                GUILayout.EndHorizontal();
            });
            GUILayout.EndScrollView();
        }

        private void ShowReferencesFromDsgVars()
        {
            if (dsgMemPersosResults.Count == 0) {
                EditorGUILayout.HelpBox("No references to from Designer Variables", MessageType.Info);
                return;
            }
            GUILayout.Label($"References from Designer Variables:");

            scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, GUILayout.MinHeight((this.position.height / 2) - headerHeight), GUILayout.ExpandHeight(true));

            if (dsgMemPersosResults.Count == 0) {
                EditorGUILayout.HelpBox("No references found", MessageType.Info);
            }

            dsgMemPersosResults.ForEach(d =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(d.fullName);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Go")) {
                    EditorGUIUtility.PingObject(d.Gao);
                }
                GUILayout.EndHorizontal();
            });
            GUILayout.EndScrollView();
        }
    }
}
