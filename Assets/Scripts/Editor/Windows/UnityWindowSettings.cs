using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenSpace;
using OpenSpace.Exporter;
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

	protected override void UpdateEditorFields() {
		FileSystem.Mode fileMode = FileSystem.Mode.Normal;
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
			fileMode = FileSystem.Mode.Web;
		}

		// Increase label width due to it being cut off otherwise
		EditorGUIUtility.labelWidth = 192;

		EditorGUI.BeginChangeCheck();
		// Game Mode
		DrawHeader(ref YPos, "Mode");
		UnitySettings.GameMode = (Settings.Mode)EditorGUI.EnumPopup(GetNextRect(ref YPos), new GUIContent("Game"), UnitySettings.GameMode);

		// Scene file
		DrawHeader(ref YPos, "Map");

		string buttonString = "No map selected";
		if (!string.IsNullOrEmpty(UnitySettings.MapName)) {
			buttonString = UnitySettings.MapName;
			if (UnitySettings.UseLevelTranslation && Settings.settingsDict[UnitySettings.GameMode].levelTranslation != null) {
				buttonString = Settings.settingsDict[UnitySettings.GameMode].levelTranslation.Translate(UnitySettings.MapName);
			}
		}
		Rect rect = GetNextRect(ref YPos, vPaddingBottom: 4f);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Map name"));
		if (fileMode == FileSystem.Mode.Web) {
			UnitySettings.MapName = EditorGUI.TextField(rect, UnitySettings.MapName);
			EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "Your build target is configured as WebGL. Raymap will attempt to load from the server. Make sure the caps in the map name are correct.", MessageType.Warning);
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
					if (game.actors?.Where(a => a.isSelectable && a.isSelectableActor1).Count() > 0) {
						rect = GetNextRect(ref YPos, vPaddingBottom: 4f);
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
					}
					if (game.actors?.Where(a => a.isSelectable && a.isSelectableActor2).Count() > 0) {
						rect = GetNextRect(ref YPos, vPaddingBottom: 4f);
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

			UnitySettings.ExportPS1Files = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Export PS1 Files"), UnitySettings.ExportPS1Files);
		}
		if (fileMode != FileSystem.Mode.Web) {
			rect = GetNextRect(ref YPos);
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
		DrawHeader(ref YPos, "Directories" + (fileMode == FileSystem.Mode.Web ? " (Web)" : ""));
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		if (fileMode == FileSystem.Mode.Web) {
			foreach (Settings.Mode mode in modes) {
				UnitySettings.GameDirectoriesWeb[mode] = EditorGUI.TextField(GetNextRect(ref YPos), mode.GetDescription(), UnitySettings.GameDirectoriesWeb.ContainsKey(mode) ? UnitySettings.GameDirectoriesWeb[mode] : "");
			}
		} else {
			foreach (Settings.Mode mode in modes) {
				UnitySettings.GameDirectories[mode] = DirectoryField(GetNextRect(ref YPos), mode.GetDescription(), UnitySettings.GameDirectories.ContainsKey(mode) ? UnitySettings.GameDirectories[mode] : "");
			}
		}
		/*if (GUILayout.Button("Update available scenes")) {
			string path = EditorUtility.OpenFilePanel("Scene files", "", "isc.ckd");
			if (path.Length != 0) {
				//UbiCanvasSettings.SelectedLevelFile = AvailableFiles.ElementAtOrDefault(SelectedLevelFileIndex);
			}
		}*/


		// Serialization
		DrawHeader("Serialization");
		UnitySettings.BackupFiles = EditorField("Create .BAK backup files", UnitySettings.BackupFiles);

		rect = GetNextRect(ref YPos);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Serialization log"));
		bool log = UnitySettings.Log;
		rect = PrefixToggle(rect, ref log);
		UnitySettings.Log = log;

		if (UnitySettings.Log)
			UnitySettings.LogFile = FileField(rect, "Serialization log File", UnitySettings.LogFile, true, "txt", includeLabel: false);

		// Export
		DrawHeader(ref YPos, "Export Settings");
		rect = GetNextRect(ref YPos);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Export After Load"));
		bool export = UnitySettings.ExportAfterLoad;
		rect = PrefixToggle(rect, ref export);
		UnitySettings.ExportAfterLoad = export;

        rect = GetNextRect(ref YPos);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Export Flags"));
		Enum exportFlags = UnitySettings.ExportFlags;
        rect = EnumFlagsToggle(rect, ref exportFlags);
        UnitySettings.ExportFlags = (MapExporter.ExportFlags) exportFlags;

        UnitySettings.ExportPath = DirectoryField(GetNextRect(ref YPos), "Export Path", UnitySettings.ExportPath);
        
        if (GUI.Button(GetNextRect(ref YPos), "Copy export commands for all levels to clipboard...")) {
            GUIUtility.systemCopyBuffer = GenerateExportScript((f) => $"Raymap.exe -batchmode " +
				$"--export {UnitySettings.ExportPath} " +
				$"--flags {(int) UnitySettings.ExportFlags} " +
				$"--mode {UnitySettings.GameMode} " +
				$"--dir \"{UnitySettings.GameDirectories[UnitySettings.GameMode]}\" " +
				$"--level {f}");
        }

		if (GUI.Button(GetNextRect(ref YPos), "Copy blend export commands for all levels to clipboard...")) {
			GUIUtility.systemCopyBuffer = GenerateExportScript((f) => $"blender --background --python generate_maps_blend.py -- {UnitySettings.ExportPath} {f} {UnitySettings.ExportPath}/BlendFiles/Levels");
        }

		UnitySettings.ScreenshotAfterLoad = (UnitySettings.ScreenshotAfterLoadSetting)EditorGUI.EnumPopup(GetNextRect(ref YPos), new GUIContent("Screenshot After Load"), UnitySettings.ScreenshotAfterLoad);

        string screenShotScaleString = EditorGUI.TextField(GetNextRect(ref YPos), "Screenshot Scale", UnitySettings.ScreenshotScale.ToString(CultureInfo.InvariantCulture));
        if (float.TryParse(screenShotScaleString, out var screenshotScale)) {
            UnitySettings.ScreenshotScale = screenshotScale;
        } else {
            UnitySettings.ScreenshotScale = 1;
        }

        UnitySettings.HighlightObjectsFilter = EditorGUI.TextField(GetNextRect(ref YPos), "Highlight objects filter", UnitySettings.HighlightObjectsFilter);
        EditorGUI.LabelField(GetNextRect(ref YPos), "Comma separated list of Model/Family names, or * to highlight all");
		UnitySettings.HighlightObjectsTextFormat = EditorGUI.TextField(GetNextRect(ref YPos), "Highlight format string", UnitySettings.HighlightObjectsTextFormat);
        EditorGUI.LabelField(GetNextRect(ref YPos), "($f=family, $m=model, $i=instance name, $c=count)");

        if (GUI.Button(GetNextRect(ref YPos), "Copy screenshot commands for all levels to clipboard...")) {
            GUIUtility.systemCopyBuffer = GenerateExportScript((f) => $"Raymap.exe -batchmode " +
                $"--mode {UnitySettings.GameMode} " +
				$"--dir \"{UnitySettings.GameDirectories[UnitySettings.GameMode]}\" " +
                $"--level {f} " +
                $"--ScreenshotPath \"{UnitySettings.ScreenshotPath}\" " +
                $"--ScreenshotAfterLoad {UnitySettings.ScreenshotAfterLoad} " +
                $"--ScreenshotScale {UnitySettings.ScreenshotScale} " +
                $"--HighlightObjectsFilter \"{UnitySettings.HighlightObjectsFilter}\" " +
                $"--HighlightObjectsTextFormat \"{UnitySettings.HighlightObjectsTextFormat}\"");
		}

		/*if (UnitySettings.ExportAfterLoad) {
            UnitySettings.ExportPath = DirectoryField(rect, "Export Path", UnitySettings.ExportPath, includeLabel: false);
        }*/

		// Misc
		DrawHeader(ref YPos, "Miscellaneous Settings");
		UnitySettings.ScreenshotPath = DirectoryField(GetNextRect(ref YPos), "Screenshot Path", UnitySettings.ScreenshotPath);
		UnitySettings.AllowDeadPointers = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Allow Dead Pointers"), UnitySettings.AllowDeadPointers);
		UnitySettings.ForceDisplayBackfaces = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Force Display Backfaces"), UnitySettings.ForceDisplayBackfaces);
		UnitySettings.BlockyMode = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Blocky Mode"), UnitySettings.BlockyMode);
		UnitySettings.TracePointers = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Trace Pointers (slow!)"), UnitySettings.TracePointers);
		UnitySettings.SaveTextures = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Save Textures"), UnitySettings.SaveTextures);
		UnitySettings.ExportText = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Export Text"), UnitySettings.ExportText);
		UnitySettings.UseLevelTranslation = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Use Level Translation"), UnitySettings.UseLevelTranslation);
		UnitySettings.VisualizeSectorBorders = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Visualize Sector Borders"), UnitySettings.VisualizeSectorBorders);
		UnitySettings.CreateFamilyGameObjects = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Create Family GameObjects"), UnitySettings.CreateFamilyGameObjects);
		UnitySettings.ShowCollisionDataForNoCollisionObjects = EditorGUI.Toggle(GetNextRect(ref YPos), new GUIContent("Show Collision Data For NoCollision SPOs"), UnitySettings.ShowCollisionDataForNoCollisionObjects);

		if (EditorGUI.EndChangeCheck() || Dirty) {
#if UNITY_EDITOR
			UnitySettings.Save();
#endif
			Dirty = false;
		}
	}

    private string GenerateExportScript(Func<string, string> lineDelegate)
    {
        Settings.Init(UnitySettings.GameMode);
        string commands = "";

        var tempDropDown =
            new MapSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), CurrentGameDataDir)
            {
                name = "Maps",
                mode = UnitySettings.GameMode
            };

        foreach (var f in tempDropDown.files) {
            commands += lineDelegate.Invoke(f) + Environment.NewLine;
        }

        return commands;
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

	private string CurrentGameDataDir {
		get {
			Dictionary<Settings.Mode, string> GameDirs =
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL ? UnitySettings.GameDirectoriesWeb : UnitySettings.GameDirectories;
			if (GameDirs.ContainsKey(UnitySettings.GameMode)) {
				return (GameDirs[UnitySettings.GameMode] ?? "");
			} else {
				return "";
			}
		}
	}
	#endregion
}