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
			Resolution res = GetCurrentResolution();
			System.DateTime dateTime = System.DateTime.Now;
			byte[] screenshotBytes = await pb.Capture(res.width* 4, res.height * 4);
			OpenSpace.Util.ByteArrayToFile(UnitySettings.ScreenshotPath + "/" + dateTime.ToString("yyyy_MM_dd HH_mm_ss") + ".png", screenshotBytes);
		}
    }

	Resolution GetCurrentResolution() {
		return new Resolution {
			width = Camera.main.pixelWidth,
			height = Camera.main.pixelHeight,
			refreshRate = Screen.currentResolution.refreshRate,
		};
	}

}