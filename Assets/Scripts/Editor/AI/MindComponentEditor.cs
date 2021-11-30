using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Linq;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(MindComponent))]
public class MindComponentEditor : Editor {

    public override void OnInspectorGUI() {
        MindComponent c = (MindComponent)target;

        Intelligence intelligenceNormal = c.mind.intelligenceNormal;
        Intelligence intelligenceReflex = c.mind.intelligenceReflex;

        AIModel aiModel = c.mind.AI_model;

        GUILayout.BeginHorizontal();
        if (aiModel.behaviors_normal != null && aiModel.behaviors_normal.Length>0) {
            int activeNormalComportIndex = intelligenceNormal.comport != null ? intelligenceNormal.comport.index + 1 : 0;
            List<string> normalComportStrings = aiModel.behaviors_normal.Select(b => b.ToString()).ToList();
            normalComportStrings.Insert(0, "<null>");

            GUILayout.Label("Current normal behavior");
            int o = activeNormalComportIndex-1;
            activeNormalComportIndex = EditorGUILayout.Popup(activeNormalComportIndex, normalComportStrings.ToArray()) - 1;
            if (activeNormalComportIndex!=o) {
                c.writeNormalComport = true;
                intelligenceNormal.comport = activeNormalComportIndex >= 0 ? aiModel.behaviors_normal[activeNormalComportIndex] : null;
            }
        } else {
            GUILayout.Label("No normal behaviors");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (aiModel.behaviors_reflex != null && aiModel.behaviors_reflex.Length > 0) {
            int activeReflexComportIndex = intelligenceReflex.comport != null ? intelligenceReflex.comport.index + 1 : 0;
            List<string> reflexComportStrings = aiModel.behaviors_reflex.Select(b => b.ToString()).ToList();
            reflexComportStrings.Insert(0, "<null>");

            GUILayout.Label("Current reflex behavior");
            int o = activeReflexComportIndex-1;
            activeReflexComportIndex = EditorGUILayout.Popup(activeReflexComportIndex, reflexComportStrings.ToArray()) - 1;
            if (activeReflexComportIndex != o) {
                c.writeReflexComport = true;
                intelligenceReflex.comport = activeReflexComportIndex>=0 ? aiModel.behaviors_reflex[activeReflexComportIndex] : null;
            }
        } else {
            GUILayout.Label("No reflex behaviors");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (aiModel.behaviors_normal != null && aiModel.behaviors_normal.Length > 0) {
            int activeInitialNormalComportIndex = intelligenceNormal.defaultComport != null ? intelligenceNormal.defaultComport.index + 1 : 0;
            List<string> normalComportStrings = aiModel.behaviors_normal.Select(b => b.ToString()).ToList();
            normalComportStrings.Insert(0, "<null>");

            GUILayout.Label("Initial normal behavior");
            int o = activeInitialNormalComportIndex - 1;
            activeInitialNormalComportIndex = EditorGUILayout.Popup(activeInitialNormalComportIndex, normalComportStrings.ToArray()) - 1;
            if (activeInitialNormalComportIndex != o) {
                c.writeInitialNormalComport = true;
                intelligenceNormal.defaultComport = activeInitialNormalComportIndex >= 0 ? aiModel.behaviors_normal[activeInitialNormalComportIndex] : null;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (aiModel.behaviors_reflex != null && aiModel.behaviors_reflex.Length > 0) {
            int activeInitialReflexComportIndex = intelligenceReflex.defaultComport != null ? intelligenceReflex.defaultComport.index + 1 : 0;
            List<string> reflexComportStrings = aiModel.behaviors_reflex.Select(b => b.ToString()).ToList();
            reflexComportStrings.Insert(0, "<null>");

            GUILayout.Label("Initial reflex behavior");
            int o = activeInitialReflexComportIndex - 1;
            activeInitialReflexComportIndex = EditorGUILayout.Popup(activeInitialReflexComportIndex, reflexComportStrings.ToArray()) - 1;
            if (activeInitialReflexComportIndex != o) {
                c.writeInitialReflexComport = true;
                intelligenceReflex.defaultComport = activeInitialReflexComportIndex >= 0 ? aiModel.behaviors_reflex[activeInitialReflexComportIndex] : null;
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Export transitions")) {
			MindComponent.print(c.TransitionExport);
		}
    }
}