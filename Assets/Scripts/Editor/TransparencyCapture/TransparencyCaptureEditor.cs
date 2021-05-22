using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;

[CustomEditor(typeof(TransparencyCaptureBehaviour))]
public class TransparencyCaptureBehaviourEditor : Editor {

    public override async void OnInspectorGUI() {
        DrawDefaultInspector();

        TransparencyCaptureBehaviour pb = (TransparencyCaptureBehaviour)target;

        if (GUILayout.Button("Take screenshot")) {
			Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
			System.DateTime dateTime = System.DateTime.Now;
			byte[] screenshotBytes = await pb.Capture((int)(res.width * UnitySettings.ScreenshotScale), (int)(res.height * UnitySettings.ScreenshotScale), true);
			OpenSpace.Util.ByteArrayToFile(UnitySettings.ScreenshotPath + "/" + dateTime.ToString("yyyy_MM_dd HH_mm_ss") + ".png", screenshotBytes);
		}
    }

	

}