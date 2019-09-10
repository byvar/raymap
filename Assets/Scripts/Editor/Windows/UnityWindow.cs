using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UnityWindow : EditorWindow {
	protected Rect GetNextRect(ref float yPos, float padding = 4f, float height = 0f, float vPadding = 0f, float vPaddingBottom = 0f) {
		if (height == 0f) height = EditorGUIUtility.singleLineHeight;
		Rect rect = new Rect(padding, yPos + vPadding, Mathf.Max(0f, EditorGUIUtility.currentViewWidth - padding * 2f - (scrollbarShown ? scrollbarWidth : 0f)), height);
		yPos += height + vPadding * 2f + vPaddingBottom;
		return rect;
	}
	protected void DrawHeader(ref float yPos, string title) {
		if (yPos > 0) {
			Rect rect = GetNextRect(ref yPos, padding: 0f, height: EditorStyles.toolbarButton.fixedHeight, vPadding: 4f);
			EditorGUI.LabelField(rect, new GUIContent(title), EditorStyles.toolbarButton);
		} else {
			Rect rect = GetNextRect(ref yPos, padding: 0f, height: EditorStyles.toolbarButton.fixedHeight, vPaddingBottom: 4f);
			EditorGUI.LabelField(rect, new GUIContent(title), EditorStyles.toolbarButton);
		}
	}
	protected Rect PrefixToggle(Rect rect, ref bool value) {
		value = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.height, rect.height), value);
		return new Rect(rect.x + rect.height, rect.y, rect.width - rect.height, rect.height);
	}
	protected Rect BrowseButton(Rect rect, string name, GUIContent content, Action action) {
		GUIStyle butStyle = EditorStyles.miniButtonRight;
		Rect buttonRect = new Rect(rect.x + rect.width - 24, rect.y, 24, rect.height);
		GUI.SetNextControlName("Button " + name);
		if (GUI.Button(buttonRect, content, butStyle)) {
			action();
		}
		return new Rect(rect.x, rect.y, rect.width - 24, rect.height);
	}
	protected string DirectoryField(Rect rect, string title, string value, bool includeLabel = true) {
		rect = BrowseButton(rect, title, EditorGUIUtility.IconContent("Folder Icon"), () => {
			string selectedFolder = EditorUtility.OpenFolderPanel(title, value, "");
			if (!string.IsNullOrEmpty(selectedFolder)) {
				GUI.FocusControl("Button " + title);
				value = selectedFolder;
				Dirty = true;
			}
		});
		if (!includeLabel) {
			value = EditorGUI.TextField(rect, value);
		} else {
			value = EditorGUI.TextField(rect, new GUIContent(title), value);
		}
		return value;
	}
	protected string FileField(Rect rect, string title, string value, bool save, string extension, bool includeLabel = true) {
		rect = BrowseButton(rect, title, EditorGUIUtility.IconContent("Folder Icon"), () => {
			string file;
			string directory = "";
			string defaultName = "";
			if (!string.IsNullOrEmpty(value)) {
				directory = Path.GetFileName(Path.GetFullPath(value));
				defaultName = Path.GetFileName(value);
			}
			if (save) {
				file = EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
			} else {
				file = EditorUtility.OpenFilePanel(title, directory, extension);
			}
			if (!string.IsNullOrEmpty(file)) {
				//Debug.Log(file);
				GUI.FocusControl("Button " + title);
				value = file;
				Dirty = true;
			}
		});
		if (!includeLabel) {
			value = EditorGUI.TextField(rect, value);
		} else {
			value = EditorGUI.TextField(rect, new GUIContent(title), value);
		}
		return value;
	}
	/*Rect BrowseButtonRect(ref Rect rect) {
		Rect buttonRect = new Rect(rect.x + rect.width - 24, rect.y, 24, rect.height);
		rect = new Rect(rect.x, rect.y, rect.width - 24, rect.height);
		return buttonRect;
	}
	void BrowseButton(Rect rect, string name, GUIContent content, Action action) {
		GUIStyle butStyle = EditorStyles.miniButtonRight;
		GUI.SetNextControlName("Button " + name);
		if (GUI.Button(rect, content, butStyle)) {
			action();
		}
	}
	string DirectoryField(Rect rect, string title, string value) {
		GUIStyle butStyle = EditorStyles.miniButtonRight;
		Rect buttonRect = BrowseButtonRect(ref rect);
		GUI.SetNextControlName("TextField " + title);
		EditorGUI.TextField(rect, new GUIContent(title), value);
		BrowseButton(buttonRect, title, EditorGUIUtility.IconContent("Folder Icon"), () => {
			string selectedFolder = EditorUtility.OpenFolderPanel(title, value, "");
			if (!string.IsNullOrEmpty(selectedFolder)) {
				GUI.FocusControl("Button " + title);
				value = selectedFolder;
				UbiCanvasSettings.Save();
			}
		});
		if (title.Contains("PSV")) Debug.Log(value);
		return value;
	}*/


	/// <summary>
	/// Any changes made?
	/// </summary>
	protected bool Dirty { get; set; }
	protected bool scrollbarShown = false;
	protected float scrollbarWidth = 16f;

	protected void OnInspectorUpdate() {
		if (EditorApplication.isPlaying && !EditorApplication.isPaused) {
			Repaint();
		}
	}
}
