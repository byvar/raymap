using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(PersoBehaviour))]
public class PersoBehaviourEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PersoBehaviour pb = (PersoBehaviour)target;
        pb.poListIndex = EditorGUILayout.Popup("Objects List", pb.poListIndex, pb.poListNames);
        pb.stateIndex = EditorGUILayout.Popup(pb.stateIndex, pb.stateNames);
        /*if (pb.perso != null && pb.perso.p3dData != null) {
            GUILayout.Label("S0: " + pb.perso.p3dData.off_stateInitial);
            GUILayout.Label("S1: " + pb.perso.p3dData.off_stateCurrent);
            GUILayout.Label("S2: " + pb.perso.p3dData.off_state2);
        }*/

        GUILayout.BeginVertical();
        byte updateCheckByte = pb.perso.stdGame.updateCheckByte;
        GUILayout.Label("StdGame.UpdateByte: " + Convert.ToString(updateCheckByte, 2).PadLeft(8, '0'));
        bool consideredOnScreen = (updateCheckByte & (1 << 5)) != 0;
        bool consideredTooFarAway = (updateCheckByte & (1 << 7)) != 0;
        GUILayout.Label("Considered on screen (bit 5): " + consideredOnScreen);
        GUILayout.Label("Considered too far away (bit 7): " + consideredTooFarAway);
        GUILayout.Label("State custom bits: " + Convert.ToString(pb.state.customStateBits, 2).PadLeft(8, '0'));
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUI.enabled = pb.stateIndex > 0;
        if (GUILayout.Button("Previous state")) pb.SetState(pb.stateIndex - 1);
        GUI.enabled = (pb.stateNames != null && pb.stateIndex < pb.stateNames.Length - 1);
        if (GUILayout.Button("Next state")) pb.SetState(pb.stateIndex + 1);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Print scripts")) pb.PrintScripts();
        if (GUILayout.Button("Print translated scripts")) pb.PrintTranslatedScripts();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Print DsgVar")) pb.PrintDsgVar();
        if (GUILayout.Button("Print DsgVar from Mind->DsgMem")) pb.PrintDsgVarFromMindMem();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Print Animation Debug Info")) pb.PrintAnimationDebugInfo();
    }
}