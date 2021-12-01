using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using System;
using OpenSpace.Animation.Component;
using OpenSpace.Object.Properties;
using UnityEditor.IMGUI.Controls;
using ModelExport.R3.PersoStatesArmatureAnimationsExporting;
using ModelExport.R3.SkinnedAnimatedMeshesExporting;

[CustomEditor(typeof(PersoBehaviour))]
public class PersoBehaviourEditor : Editor {
	StateTransitionsTreeView treeViewStateTransitions;
	TreeViewState treeviewStateTransitionsState;
	MultiColumnHeaderState m_MultiColumnHeaderState;

    //private int initialRule = -1;
    //private int initialReflex = -1;

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
        byte updateCheckByte = pb.perso.stdGame.miscFlags;
        GUILayout.Label("StdGame: " + pb.perso.stdGame.offset.ToString());
        GUILayout.Label("StdGame.UpdateByte: " + Convert.ToString(updateCheckByte, 2).PadLeft(8, '0'));
        bool consideredOnScreen = (updateCheckByte & (1 << 5)) != 0;
        bool consideredTooFarAway = (updateCheckByte & (1 << 7)) != 0;
        GUILayout.Label("Considered on screen (bit 5): " + consideredOnScreen);
        GUILayout.Label("Considered too far away (bit 7): " + consideredTooFarAway);
        GUILayout.Label("State custom bits: " + Convert.ToString(pb.state != null ? pb.state.customStateBits : 0, 2).PadLeft(8, '0'));

		/* // Only enable when working on morph data, it prevents from using the buttons properly otherwise
        if (pb.a3d != null && pb.morphDataArray != null) {
            for (int i = 0; i < pb.a3d.num_channels; i++) {
                AnimMorphData currentMorphData = pb.morphDataArray[i, pb.currentFrame];

                if (currentMorphData != null) {
                    GUILayout.Label("MorphData[" + i + "," + pb.currentFrame + "]: Morph to " + currentMorphData.objectIndexTo + ", progress " + currentMorphData.morphProgress);
                }
            }
        }*/
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUI.enabled = pb.currentState > 0;
        if (GUILayout.Button("Previous state")) pb.SetState(pb.currentState - 1);
        GUI.enabled = (pb.stateNames != null && pb.currentState < pb.stateNames.Length - 1);
        if (GUILayout.Button("Next state")) pb.SetState(pb.currentState + 1);
        GUI.enabled = true;
        GUILayout.EndHorizontal();

		if (pb.IsLoaded) {
			Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 100f);
			InitTransitionsTreeIfNeeded(rect, pb);
			if (treeViewStateTransitions.stateIndex != pb.currentState
				|| treeViewStateTransitions.perso != pb) {
				treeViewStateTransitions.perso = pb;
				treeViewStateTransitions.stateIndex = pb.currentState;
				treeViewStateTransitions.treeModel.SetData(GetData());
				treeViewStateTransitions.Reload();
			}
			treeViewStateTransitions.OnGUI(rect);
		}

		GUILayout.Label($"Initial Rule: "+pb.perso?.brain?.mind?.intelligenceNormal?.defaultComport?.NameSubstring);
		GUILayout.Label($"Initial Reflex: "+pb.perso?.brain?.mind?.intelligenceReflex?.defaultComport?.NameSubstring);

        /*if (pb.state != null && pb.state.stateTransitions != null && pb.state.stateTransitions.Count > 0) {
            GUILayout.Label("State transition");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target State");
            GUILayout.Label("State To Go");
            GUILayout.EndHorizontal();
            foreach (State.Transition t in pb.state.stateTransitions) {
                if (t != null) {
                    State stateToGo = State.FromOffset(t.off_stateToGo);
                    State targetState = State.FromOffset(t.off_targetState);
                    if (stateToGo != null && targetState != null) {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(targetState.ToString())) pb.SetState(targetState);
                        if (GUILayout.Button(stateToGo.ToString())) pb.SetState(stateToGo);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }*/

        if (GUILayout.Button("Print scripts")) pb.PrintScripts();
        if (GUILayout.Button("Print translated scripts")) pb.PrintTranslatedScripts();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Print DsgVar")) pb.PrintDsgVar();
        if (GUILayout.Button("Print DsgVar from Mind->DsgMem")) pb.PrintDsgVarFromMindMem();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Print Animation Debug Info")) pb.PrintAnimationDebugInfo();
		if (GUILayout.Button("Export Meshes and Animations Data (including cleanup)")) ExportMeshesAndAnimationsData();
		if (GUILayout.Button("Export Meshes Data")) ExportMeshesData();
		if (GUILayout.Button("Export Animations Data")) ExportAnimationsData();
    }

	IList<StateTransitionsTreeElement> GetData() {
		PersoBehaviour pb = (PersoBehaviour)target;
		List<StateTransitionsTreeElement> tr = new List<StateTransitionsTreeElement>();
		tr.Add(new StateTransitionsTreeElement("Hidden root", -1, -1));
		BasePersoBehaviour.StateTransition[] transitions = pb.GetStateTransitions(pb.currentState);
		if (transitions != null) {
			int id = 0;
			foreach (var t in transitions) {
				tr.Add(new StateTransitionsTreeElement("State transition " + t.TargetStateName, 0, id) {
					stateToGoName = t.StateToGoName,
					stateToGoIndex = t.StateToGoIndex,
					targetStateName = t.TargetStateName,
					targetStateIndex = t.TargetStateIndex,
					linkingType = t.LinkingType
				});
				id++;
			}
		}
		return tr;
	}

	void InitTransitionsTreeIfNeeded(Rect transitionsRect, PersoBehaviour target) {
		if (treeViewStateTransitions == null || treeviewStateTransitionsState == null || treeViewStateTransitions.perso != target) {
			treeviewStateTransitionsState = new TreeViewState();

			bool firstInit = m_MultiColumnHeaderState == null;
			var headerState = StateTransitionsTreeView.CreateDefaultMultiColumnHeaderState(transitionsRect.width);
			if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
				MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
			m_MultiColumnHeaderState = headerState;

			var multiColumnHeader = new MultiColumnHeader(headerState);
			if (firstInit)
				multiColumnHeader.ResizeToFit();

			var treeModel = new TreeModel<StateTransitionsTreeElement>(GetData());

			treeViewStateTransitions = new StateTransitionsTreeView(treeviewStateTransitionsState, multiColumnHeader, treeModel) {
				perso = target,
				stateIndex = target.stateIndex,
			};
		}
	}

	void ExportMeshesAndAnimationsData()
	{
		ObjectsExportLibraryInterface.ClearExportObjectsLibrary();
		ExportMeshesData();
		ExportAnimationsData();
	}

	void ExportMeshesData()
    {
		PersoBehaviour pb = (PersoBehaviour)target;
		var meshes = pb.gameObject.GetComponentsInChildren<ExportableModel>();
		foreach(var m in meshes) {
			if (m.gameObject.activeInHierarchy) {
				if (m != m.gameObject.transform.parent.GetComponentsInChildren<ExportableModel>()[0]) { // Ignore any ExportableModel that's not the first of its siblinngs
					continue;
				}
				m.AddToExportObjectsLibrary();
			}
        }
	}

	void ExportAnimationsData() {
		PersoBehaviour pb = (PersoBehaviour)target;
		PersoAnimationsDataExporter pade = new PersoAnimationsDataExporter(pb);
		pade.ExportPersoStatesAnimations();
	}
}