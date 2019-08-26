using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace;
using OpenSpace.Visual;

[CustomEditor(typeof(FamilyComponent))]
public class FamilyEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();


        FamilyComponent fc = (FamilyComponent)target;

        if (GUILayout.Button("Rebuild")) {
            var obj = fc.family.objectLists[0].entries[0].po.visualSet[0].obj;
            ((obj as MeshObject).subblocks[0] as MeshElement).Reset();
            var test = ((obj as MeshObject).subblocks[0] as MeshElement).Gao;
        }

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        foreach(MechanicsIDCard idCard in fc.idCards) {
            GUILayout.BeginVertical();
            GUILayout.Label("Gravity: " + idCard.gravity);
            GUILayout.Label("MaxInertia: " + idCard.maxInertia);

            MechanicsIDCardFlags editableFlags = new MechanicsIDCardFlags();
            editableFlags.SetRawFlags((int)idCard.flags);
            GUILayout.Label("Flags: "+editableFlags.flagPreview);
            uint oldFlags = idCard.flags;
            DrawMechanicsIDCardFlags(editableFlags);
            idCard.flags = (uint)editableFlags.rawFlags;

            if (oldFlags!=idCard.flags) {
                fc.dirty = true;
            }

            GUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        GUILayout.EndVertical();

    }



    public void DrawMechanicsIDCardFlags(MechanicsIDCardFlags flags) {
        GUILayoutOption[] widthOption = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100) };

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        flags.Animation = EditorGUILayout.ToggleLeft("Animation", flags.Animation, widthOption);
        flags.Collision = EditorGUILayout.ToggleLeft("Collision", flags.Collision, widthOption);
        flags.Gravity = EditorGUILayout.ToggleLeft("Gravity", flags.Gravity, widthOption);
        flags.Tilt = EditorGUILayout.ToggleLeft("Tilt", flags.Tilt, widthOption);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        flags.Gymnastics = EditorGUILayout.ToggleLeft("Gymnastics", flags.Gymnastics, widthOption);
        flags.OnGround = EditorGUILayout.ToggleLeft("OnGround", flags.OnGround, widthOption);
        flags.Climbing = EditorGUILayout.ToggleLeft("Climbing", flags.Climbing, widthOption);
        flags.Spider = EditorGUILayout.ToggleLeft("Spider", flags.Spider, widthOption);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        flags.Shoot = EditorGUILayout.ToggleLeft("Shoot", flags.Shoot, widthOption);
        flags.CollisionControl = EditorGUILayout.ToggleLeft("CollisionControl", flags.CollisionControl, widthOption);
        flags.KeepZVelocity = EditorGUILayout.ToggleLeft("KeepZVelocity", flags.KeepZVelocity, widthOption);
        flags.SpeedLimit = EditorGUILayout.ToggleLeft("SpeedLimit", flags.SpeedLimit, widthOption);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        flags.Inertia = EditorGUILayout.ToggleLeft("Inertia", flags.Inertia, widthOption);
        flags.Stream = EditorGUILayout.ToggleLeft("Stream", flags.Stream, widthOption);
        flags.StickOnPlatform = EditorGUILayout.ToggleLeft("StickOnPlatform", flags.StickOnPlatform, widthOption);
        flags.Scale = EditorGUILayout.ToggleLeft("Scale", flags.Scale, widthOption);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        flags.Flag16 = EditorGUILayout.ToggleLeft("Flag16", flags.Flag16, widthOption);
        flags.Swim = EditorGUILayout.ToggleLeft("Swim", flags.Swim, widthOption);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }
}