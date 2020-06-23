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
		FileSystem.Mode fileMode = FileSystem.Mode.Normal;
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
			fileMode = FileSystem.Mode.Web;
		}

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
			if (UnitySettings.UseLevelTranslation && Settings.settingsDict[UnitySettings.GameMode].levelTranslation != null) {
				buttonString = Settings.settingsDict[UnitySettings.GameMode].levelTranslation.Translate(UnitySettings.MapName);
			}
		}
		Rect rect = GetNextRect(ref yPos, vPaddingBottom: 4f);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Map name"));
		if (fileMode == FileSystem.Mode.Web) {
			UnitySettings.MapName = EditorGUI.TextField(rect, UnitySettings.MapName);
			EditorGUI.HelpBox(GetNextRect(ref yPos, height: 40f), "Your build target is configured as WebGL. Raymap will attempt to load from the server. Make sure the caps in the map name are correct.", MessageType.Warning);
		} else {

			EditorGUI.BeginDisabledGroup(UnitySettings.LoadFromMemory);
			if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
				// Initialize settings
				Settings.Init(UnitySettings.GameMode);
				string directory = CurrentGameDataDir;
				/*string directory = (UnitySettings.CurrentGameDataDir + "/" + Settings.s.ITFDirectory).Replace(Path.DirectorySeparatorChar, '/');
				if (!directory.EndsWith("/")) directory += "/";
				while (directory.Contains("//")) directory = directory.Replace("//", "/");
				string extension = "*.isc";
				if (Settings.s.cooked) {
					extension += ".ckd";
				}*/

				if (MapDropdown == null || MapDropdown.directory != directory || MapDropdown.mode != UnitySettings.GameMode) {
					MapDropdown = new MapSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), directory) {
						name = "Maps",
						mode = UnitySettings.GameMode
					};
				}
				MapDropdown.Show(rect);
			}
			EditorGUI.EndDisabledGroup();
		}
		if (MapDropdown != null && MapDropdown.selection != null) {
			UnitySettings.MapName = MapDropdown.selection;
			MapDropdown.selection = null;
			Dirty = true;
		}
		if (Settings.settingsDict[UnitySettings.GameMode].platform == Settings.Platform.PS1) {
			OpenSpace.PS1.PS1GameInfo.Games.TryGetValue(UnitySettings.GameMode, out OpenSpace.PS1.PS1GameInfo game);
			if (game != null && game.actors?.Where(a => a.isSelectable).Count() > 0) {
				int mapIndex = Array.IndexOf(game.maps.Select(s => s.ToLower()).ToArray(), UnitySettings.MapName.ToLower());
				if (mapIndex != -1 && game.files.FirstOrDefault(f => f.type == OpenSpace.PS1.PS1GameInfo.File.Type.Map).memoryBlocks[mapIndex].isActorSelectable) {
					rect = GetNextRect(ref yPos, vPaddingBottom: 4f);
					rect = EditorGUI.PrefixLabel(rect, new GUIContent("Actor 1"));
					buttonString = "No actor selected";
					if (!string.IsNullOrEmpty(UnitySettings.Actor1Name)) {
						buttonString = UnitySettings.Actor1Name;
					}
					if (fileMode == FileSystem.Mode.Web) {
						UnitySettings.Actor1Name = EditorGUI.TextField(rect, UnitySettings.Actor1Name);
					} else {
						EditorGUI.BeginDisabledGroup(UnitySettings.LoadFromMemory);
						if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
							if (ActorDropdown1 == null || ActorDropdown1.mode != UnitySettings.GameMode) {
								ActorDropdown1 = new PS1ActorSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), UnitySettings.GameMode, 0) {
									name = "Actors"
								};
							}
							ActorDropdown1.Show(rect);
						}
						EditorGUI.EndDisabledGroup();
					}
					if (ActorDropdown1 != null && ActorDropdown1.selection != null) {
						UnitySettings.Actor1Name = ActorDropdown1.selection;
						ActorDropdown1.selection = null;
						Dirty = true;
					}
					rect = GetNextRect(ref yPos, vPaddingBottom: 4f);
					rect = EditorGUI.PrefixLabel(rect, new GUIContent("Actor 2"));
					buttonString = "No actor selected";
					if (!string.IsNullOrEmpty(UnitySettings.Actor2Name)) {
						buttonString = UnitySettings.Actor2Name;
					}
					if (fileMode == FileSystem.Mode.Web) {
						UnitySettings.Actor2Name = EditorGUI.TextField(rect, UnitySettings.Actor2Name);
					} else {
						EditorGUI.BeginDisabledGroup(UnitySettings.LoadFromMemory);
						if (EditorGUI.DropdownButton(rect, new GUIContent(buttonString), FocusType.Passive)) {
							if (ActorDropdown2 == null || ActorDropdown2.mode != UnitySettings.GameMode) {
								ActorDropdown2 = new PS1ActorSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), UnitySettings.GameMode, 1) {
									name = "Actors"
								};
							}
							ActorDropdown2.Show(rect);
						}
						EditorGUI.EndDisabledGroup();
					}
					if (ActorDropdown2 != null && ActorDropdown2.selection != null) {
						UnitySettings.Actor2Name = ActorDropdown2.selection;
						ActorDropdown2.selection = null;
						Dirty = true;
					}
				}
			}
		}
		if (fileMode != FileSystem.Mode.Web) {
			rect = GetNextRect(ref yPos);
			rect = EditorGUI.PrefixLabel(rect, new GUIContent("Load Process"));
			bool loadFromMemory = UnitySettings.LoadFromMemory;
			rect = PrefixToggle(rect, ref loadFromMemory);
			UnitySettings.LoadFromMemory = loadFromMemory;
			if (UnitySettings.LoadFromMemory) {
				Rect extensionRect = new Rect(rect.x + rect.width - 32, rect.y, 32, rect.height);
				EditorGUI.LabelField(extensionRect, ".exe");
				rect = new Rect(rect.x, rect.y, rect.width - 32, rect.height);
				UnitySettings.ProcessName = EditorGUI.TextField(rect, UnitySettings.ProcessName);
			}
		}

		// Directories
		DrawHeader(ref yPos, "Directories" + (fileMode == FileSystem.Mode.Web ? " (Web)" : ""));
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		if (fileMode == FileSystem.Mode.Web) {
			foreach (Settings.Mode mode in modes) {
				UnitySettings.GameDirsWeb[mode] = EditorGUI.TextField(GetNextRect(ref yPos), mode.GetDescription(), UnitySettings.GameDirsWeb.ContainsKey(mode) ? UnitySettings.GameDirsWeb[mode] : "");
			}
		} else {
			foreach (Settings.Mode mode in modes) {
				UnitySettings.GameDirs[mode] = DirectoryField(GetNextRect(ref yPos), mode.GetDescription(), UnitySettings.GameDirs.ContainsKey(mode) ? UnitySettings.GameDirs[mode] : "");
			}
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

		UnitySettings.ExportPath = DirectoryField(GetNextRect(ref yPos), "Export Path", UnitySettings.ExportPath);
		UnitySettings.ScreenshotAfterLoad = (UnitySettings.ScreenshotAfterLoadSetting)EditorGUI.EnumPopup(GetNextRect(ref yPos), new GUIContent("Screenshot After Load"), UnitySettings.ScreenshotAfterLoad);

		/*if (UnitySettings.ExportAfterLoad) {
			UnitySettings.ExportPath = DirectoryField(rect, "Export Path", UnitySettings.ExportPath, includeLabel: false);
		}*/

		// Misc
		DrawHeader(ref yPos, "Miscellaneous Settings");
		UnitySettings.ScreenshotPath = DirectoryField(GetNextRect(ref yPos), "Screenshot Path", UnitySettings.ScreenshotPath);
		UnitySettings.AllowDeadPointers = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Allow Dead Pointers"), UnitySettings.AllowDeadPointers);
		UnitySettings.ForceDisplayBackfaces = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Force Display Backfaces"), UnitySettings.ForceDisplayBackfaces);
		UnitySettings.BlockyMode = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Blocky Mode"), UnitySettings.BlockyMode);
		UnitySettings.SaveTextures = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Save Textures"), UnitySettings.SaveTextures);
		UnitySettings.ExportText = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Export Text"), UnitySettings.ExportText);
		UnitySettings.UseLevelTranslation = EditorGUI.Toggle(GetNextRect(ref yPos), new GUIContent("Use Level Translation"), UnitySettings.UseLevelTranslation);



		totalyPos = yPos;
		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck() || Dirty) {
#if UNITY_EDITOR
			UnitySettings.Save();
#endif
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
	private MapSelectionDropdown MapDropdown { get; set; }
	private PS1ActorSelectionDropdown ActorDropdown1 { get; set; }
	private PS1ActorSelectionDropdown ActorDropdown2 { get; set; }

	private float totalyPos = 0f;
	private Vector2 scrollPosition = Vector2.zero;


	private string CurrentGameDataDir {
		get {
			Dictionary<Settings.Mode, string> GameDirs =
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL ? UnitySettings.GameDirsWeb : UnitySettings.GameDirs;
			if (GameDirs.ContainsKey(UnitySettings.GameMode)) {
				return (GameDirs[UnitySettings.GameMode] ?? "");
			} else {
				return "";
			}
		}
	}
	#endregion
}