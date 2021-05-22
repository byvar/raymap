using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RaymanTrail))]
    public class RaymanTrailEditor : UnityEditor.Editor {


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = target as RaymanTrail;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(t.Recording ? "Stop Recording" : "Start Recording")) {
                t.Recording = !t.Recording; }

            if (GUILayout.Button("Reset")) {
                t.Reset();
            }
            GUILayout.EndHorizontal();

            if (!t.Recording) {
                if (GUILayout.Button(t.ShowAll ? "Show Single Attempt" : "Show All Attempts")) {
                    t.ShowAll = !t.ShowAll;
                    t.PlaybackStage = 1.0f;
                }

                if (!t.ShowAll) {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("First Attempt")) t.AttemptToShowNumber = 0;
                    if (GUILayout.Button("Previous Attempt")) t.AttemptToShowNumber--;
                    var centeredStyle = new GUIStyle(GUI.skin.label);
                    centeredStyle.alignment = TextAnchor.MiddleCenter;
                    GUILayout.Label(t.AttemptToShowNumber.ToString(), centeredStyle);
                    if (GUILayout.Button("Next Attempt")) t.AttemptToShowNumber++;
                    if (GUILayout.Button("Last Attempt")) t.AttemptToShowNumber = t.AttemptCount;

                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();
                    GUILayout.Label("Playback Attempt");
                    GUILayout.BeginHorizontal();

                    if (GUILayout.RepeatButton("<<")) {
                        t.PreviousFrame();
                    }
                    if (GUILayout.Button("<")) {
                        t.PreviousFrame();
                    }
                    if (GUILayout.Button(">")) {
                        t.NextFrame();
                    }
                    if (GUILayout.RepeatButton(">>")) {
                        t.NextFrame();
                    }
                    GUILayout.EndHorizontal();
                    t.PlaybackStage = GUILayout.HorizontalSlider(t.PlaybackStage, 0.0f, 1.0f);
                    GUILayout.Space(30);
                    GUILayout.EndVertical();
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save to Clipboard")) {

                    GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(t.Data); ;
                }
                if (GUILayout.Button("Paste from Clipboard")) {

                    t.Data = JsonConvert.DeserializeObject<RaymanTrail.RecordingData>(GUIUtility.systemCopyBuffer);
                }
                GUILayout.EndHorizontal();
            }
        }

    }
}
