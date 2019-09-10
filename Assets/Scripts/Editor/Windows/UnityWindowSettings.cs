using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenSpace;
using UnityEditor;
using UnityEngine;
using Path = System.IO.Path;

public class UnityWindowSettings : UnityWindow {
	#region Unity Methods
	[MenuItem("Raymap/Settings")]
	public static void ShowWindow() {
		GetWindow<UnityWindowSettings>(false, "Settings", true);
	}
	private void OnEnable() {
		titleContent = EditorGUIUtility.IconContent("Settings");
		titleContent.text = "Settings";
	}
	public void OnGUI() {
		// Increase label width due to it being cut off otherwise
		EditorGUIUtility.labelWidth = 195;

		float yPos = 0f;

		if (totalyPos == 0f) totalyPos = position.height;
		scrollbarShown = totalyPos > position.height;
		scrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, position.height), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), totalyPos));

		EditorGUI.BeginChangeCheck();
		// Game Mode
		DrawHeader(ref yPos, "Mode");
		UnitySettings.GameMode = (Settings.Mode)EditorGUI.EnumPopup(GetNextRect(ref yPos), new GUIContent("Game"), UnitySettings.GameMode);

		// Scene file
		DrawHeader(ref yPos, "Map");

		string buttonString = "No map selected";
		if (!string.IsNullOrEmpty(UnitySettings.MapName)) {
			buttonString = UnitySettings.MapName;
		}
		Rect rect = GetNextRect(ref yPos, vPaddingBottom: 4f);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Map name"));
		if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
			// Initialize settings
			Settings.Init(UnitySettings.GameMode);
			string directory = UnitySettings.CurrentGameDataDir;
			/*string directory = (UnitySettings.CurrentGameDataDir + "/" + Settings.s.ITFDirectory).Replace(Path.DirectorySeparatorChar, '/');
			if (!directory.EndsWith("/")) directory += "/";
			while (directory.Contains("//")) directory = directory.Replace("//", "/");
			string extension = "*.isc";
			if (Settings.s.cooked) {
				extension += ".ckd";
			}*/

			if (Dropdown == null || Dropdown.directory != directory || Dropdown.mode != UnitySettings.GameMode) {
				Dropdown = new MapSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory) {
					name = "Maps",
					mode = UnitySettings.GameMode
				};
			}
			Dropdown.Show(rect);
		}
		if (Dropdown != null && Dropdown.selection != null) {
			UnitySettings.MapName = Dropdown.selection;
			Dropdown.selection = null;
			Dirty = true;
		}

		// Directories
		DrawHeader(ref yPos, "Directories");
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		foreach (Settings.Mode mode in modes) {
			UnitySettings.GameDirs[mode] = DirectoryField(GetNextRect(ref yPos), mode.GetDescription(), UnitySettings.GameDirs.ContainsKey(mode) ? UnitySettings.GameDirs[mode] : "");
		}
		/*if (GUILayout.Button("Update available scenes")) {
			string path = EditorUtility.OpenFilePanel("Scene files", "", "isc.ckd");
			if (path.Length != 0) {
				//UbiCanvasSettings.SelectedLevelFile = AvailableFiles.ElementAtOrDefault(SelectedLevelFileIndex);
			}
		}*/

		// Export
		DrawHeader(ref yPos, "Export Settings");
		rect = GetNextRect(ref yPos);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Export After Load"));
		bool export = UnitySettings.ExportAfterLoad;
		rect = PrefixToggle(rect, ref export);
		UnitySettings.ExportAfterLoad = export;
		if (UnitySettings.ExportAfterLoad) {
			UnitySettings.ExportPath = DirectoryField(rect, "Export Path", UnitySettings.ExportPath, includeLabel: false);
		}

		// Misc
		DrawHeader(ref yPos, "Miscellaneous Settings");
		UnitySettings.AllowDeadPointers = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Allow Dead Pointers"), UnitySettings.AllowDeadPointers);
		UnitySettings.ForceDisplayBackfaces = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Force Display Backfaces"), UnitySettings.ForceDisplayBackfaces);
		UnitySettings.BlockyMode = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Blocky Mode"), UnitySettings.BlockyMode);
		UnitySettings.SaveTextures = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Save Textures"), UnitySettings.SaveTextures);


		totalyPos = yPos;
		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck() || Dirty) {
			UnitySettings.Save();
			Dirty = false;
		}
	}
	#endregion

	#region Properties

	/// <summary>
	/// The selected level file index, based on <see cref="AvailableFiles"/>
	/// </summary>
	private int SelectedLevelFileIndex { get; set; }

	/// <summary>
	/// The file selection dropdown
	/// </summary>
	private MapSelectionDropdown Dropdown { get; set; }

	private float totalyPos = 0f;
	private Vector2 scrollPosition = Vector2.zero;
	#endregion

}