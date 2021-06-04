using OpenSpace;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor {

    public class RecordingToolWindow : EditorWindow
    {

        private static RecordingTool t;

        private List<string> alwaysNameList;

        [MenuItem("Raymap/Recording Tool")]
        public static void Init()
        {
            RecordingToolWindow window = (RecordingToolWindow)EditorWindow.GetWindow(typeof(RecordingToolWindow));
            window.titleContent = new GUIContent("Recording Tool");
            window.Show();
        }

        void OnGUI()
        {
            if (!EditorApplication.isPlaying || Settings.s == null) {
                EditorGUILayout.HelpBox("Please start the scene to use this window.", MessageType.Info);
                t = null;
                return;
            }

            if (t == null) {
                t = Instantiate(MapLoader.Loader.controller.recordingToolPrefab);
            }

            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.MiddleCenter;

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Status: " + t.State.ToString());
            GUILayout.BeginHorizontal();

            if (t.State != RecordingTool.RecordingState.Recording) {
                if (GUILayout.Button("Start Recording")) {
                    t.StartRecording();
                }
            } else {
                if (GUILayout.Button("Stop Recording")) {
                    t.StopRecording();
                }
            }

            EditorGUI.BeginDisabledGroup(t.State != RecordingTool.RecordingState.Playback);
            DrawPlayControls(t);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(t.State == RecordingTool.RecordingState.Idle);
            if (GUILayout.Button("Reset")) {
                t.Reset();
            }

            GUILayout.EndHorizontal();
            t.CurrentTime = GUILayout.HorizontalSlider(t.CurrentTime, 0.0f, t.Data?.Duration ?? 0);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUILayout.Label("Playback Speed");
            string playbackSpeed = GUILayout.TextField(t.PlaybackSpeed.ToString("#.##"));
            float.TryParse(playbackSpeed, out t.PlaybackSpeed);

            if (GUILayout.Button("-1x")) t.PlaybackSpeed = -1;
            if (GUILayout.Button("0.5x")) t.PlaybackSpeed = 0.5f;
            if (GUILayout.Button("1x")) t.PlaybackSpeed = 1;
            if (GUILayout.Button("2x")) t.PlaybackSpeed = 2;

            GUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            GUILayout.Space(30);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save to Clipboard")) {

                GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(t.Data); ;
            }
            if (GUILayout.Button("Paste from Clipboard")) {

                t.Data = JsonConvert.DeserializeObject<RecordingTool.RecordingData>(GUIUtility.systemCopyBuffer);
            }
            GUILayout.EndHorizontal();

            t.AlignCameraToGame = EditorGUILayout.Toggle("Align Unity's Camera with Game Camera", t.AlignCameraToGame);
            t.InterpolateTransforms = EditorGUILayout.Toggle("Interpolate object transforms during playback", t.InterpolateTransforms);

            if (GUILayout.Button("(DEBUG) Show Spawned Always Objects")) {
                alwaysNameList = new List<string>();
                if (t.SpawnedAlwaysObjects != null) {
                    foreach (var spo in t.SpawnedAlwaysObjects.Values) {
                        alwaysNameList.Add(spo.perso.fullName);
                    }
                }
            }

            GUILayout.BeginVertical();
            if (alwaysNameList != null) {
                foreach (var spo in alwaysNameList) {
                    GUILayout.Label(spo);
                }
            }
            GUILayout.EndVertical();
        }

        private static void DrawPlayControls(RecordingTool t)
        {
            if (GUILayout.RepeatButton("<<")) {
                t.SeekBackwards();
            }

            if (GUILayout.Button("<")) {
                t.PreviousFrame();
            }

            if (GUILayout.Button(t.IsPlaying?"Pause":"Play")) {
                t.TogglePlaying();
            }

            if (GUILayout.Button(">")) {
                t.NextFrame();
            }

            if (GUILayout.RepeatButton(">>")) {
                t.SeekForwards();
            }
        }
    }
}
