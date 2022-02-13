using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.Unity;
using OpenSpace;
using OpenSpace.Exporter;
using Raymap;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
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

		bool refreshGameActions = (GameModeDropdown?.HasChanged ?? false) || (MapSelectionDropdown?.HasChanged ?? false);

		// Increase label width due to it being cut off otherwise
		EditorGUIUtility.labelWidth = 192;


		if (CategorizedGameModes == null) {
			var modes = EnumHelpers.GetValues<GameModeSelection>().Select(x => new {
				GameModeSelection = x,
				GameModeAttribute = x.GetAttribute<GameModeAttribute>(),
				EngineCategoryAttribute = x.GetAttribute<GameModeAttribute>()
					.Category.GetAttribute<EngineCategoryAttribute>(),
				EngineAttribute = x.GetAttribute<GameModeAttribute>()
					.Category.GetAttribute<EngineCategoryAttribute>()
					.Engine.GetAttribute<EngineAttribute>(),

			})
				.GroupBy(x => x.GameModeAttribute.Category)
				.GroupBy(x => x.FirstOrDefault().EngineCategoryAttribute.Engine);
			CategorizedGameModes = modes.ToDictionary(
				engineGroup => engineGroup.Key,
				engineGroup => engineGroup.ToDictionary(
					categoryGroup => categoryGroup.Key,
					categoryGroup => categoryGroup.Select(x => x.GameModeSelection)));
		}


		EditorGUI.BeginChangeCheck();


		if (fileMode == FileSystem.Mode.Web) {
			EditorGUI.HelpBox(GetNextRect(ref YPos, height: 40f), "Your build target is configured as WebGL. Raymap will attempt to load from the server. Make sure the caps in the map name are correct.", MessageType.Warning);
		}

		// Game Mode
		DrawHeader(ref YPos, "Mode");

		if (GameModeDropdown == null)
			GameModeDropdown = new GameModeSelectionDropdown(new AdvancedDropdownState());
		var rectTemp = GetNextRect(ref YPos);
		var rbutton = EditorGUI.PrefixLabel(rectTemp, new GUIContent("Game"));
		rectTemp = new Rect(rbutton.x + rbutton.width - Mathf.Max(400f, rbutton.width), rbutton.y, Mathf.Max(400f, rbutton.width), rbutton.height);
		if (EditorGUI.DropdownButton(rbutton, new GUIContent(GameModeDropdown.SelectionName), FocusType.Passive))
			GameModeDropdown.Show(rectTemp);

		// Scene file
		DrawHeader(ref YPos, "Map");


		Rect rect;
		string buttonString;

		if (fileMode == FileSystem.Mode.Normal) {
			rectTemp = GetNextRect(ref YPos);
			rbutton = EditorGUI.PrefixLabel(rectTemp, new GUIContent("Map"));
			rectTemp = new Rect(rbutton.x + rbutton.width - Mathf.Max(400f, rbutton.width), rbutton.y, Mathf.Max(400f, rbutton.width), rbutton.height);

			// Map selection dropdown
			if (MapSelectionDropdown == null || GameModeDropdown.HasChanged) {
				if (GameModeDropdown.HasChanged) {
					UnitySettings.SelectedGameMode = GameModeDropdown.Selection;
					GameModeDropdown.HasChanged = false;
					Dirty = true;
				}

				MapTreeNode mapTree;

				try {
					var manager = UnitySettings.GetGameManager;
					var settings = UnitySettings.GetGameSettings;

					mapTree = manager.GetLevels(settings);
				} catch (Exception ex) {
					mapTree = new MapTreeNode(null,null);
					Debug.LogWarning(ex.Message);
				}

				MapSelectionDropdown = new MapSelectionDropdown(new AdvancedDropdownState(), mapTree);

				// Debug.Log($"Map selection updated with {volumes.Length} volumes");
			}

			// Next & previous map buttons
			BrowseButton(rbutton, "Next map", EditorGUIUtility.IconContent("Profiler.NextFrame"), () => {
				MapSelectionDropdown.NextMap();
			}, ButtonWidth);
			rbutton = new Rect(rbutton.x, rbutton.y, rbutton.width - ButtonWidth, rbutton.height);
			BrowseButton(rbutton, "Previous map", EditorGUIUtility.IconContent("Profiler.PrevFrame"), () => {
				MapSelectionDropdown.PreviousMap();
			}, ButtonWidth);
			rbutton = new Rect(rbutton.x, rbutton.y, rbutton.width - ButtonWidth, rbutton.height);

			// Map selection dropdown button
			buttonString = "No map selected";
			if (!string.IsNullOrEmpty(UnitySettings.MapName)) {
				buttonString = UnitySettings.MapName;
			}
			if (MapSelectionDropdown.SelectedMap != null) {
				buttonString = MapSelectionDropdown.GetName(MapSelectionDropdown.SelectedMap);
			}
			if (EditorGUI.DropdownButton(rbutton, new GUIContent(buttonString), FocusType.Passive))
				MapSelectionDropdown.Show(rectTemp);
		} else if (fileMode == FileSystem.Mode.Web) {
			if (GameModeDropdown.HasChanged) {
				UnitySettings.SelectedGameMode = GameModeDropdown.Selection;
				GameModeDropdown.HasChanged = false;
				Dirty = true;
			}
			UnitySettings.MapName = EditorField("Map", UnitySettings.MapName);
		}
		
		var managerTmp = UnitySettings.SelectedGameMode.GetManager();
		if (managerTmp is PS1LegacyGameManager ps1manager) {
			var LegacyMode = ps1manager.GetLegacyMode(UnitySettings.GetGameSettings);
			OpenSpace.PS1.PS1GameInfo.Games.TryGetValue(LegacyMode, out OpenSpace.PS1.PS1GameInfo game);
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
								if (ActorDropdown1 == null || ActorDropdown1.mode != LegacyMode) {
									ActorDropdown1 = new PS1ActorSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), LegacyMode, 0) {
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
								if (ActorDropdown2 == null || ActorDropdown2.mode != LegacyMode) {
									ActorDropdown2 = new PS1ActorSelectionDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState(), LegacyMode, 1) {
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

			UnitySettings.ExportPS1Files = EditorField("Export PS1 Files", UnitySettings.ExportPS1Files);
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

		foreach (var engine in CategorizedGameModes) {
			UnitySettings.HideDirectoriesEngine[engine.Key] = !EditorGUI.Foldout(GetNextRect(ref YPos), !UnitySettings.HideDirectoriesEngine.TryGetItem(engine.Key, true), $"Directories ({engine.Key.GetAttribute<EngineAttribute>().DisplayName})", true);
			if (UnitySettings.HideDirectoriesEngine[engine.Key]) continue;
			foreach (var category in engine.Value) {
				IndentLevel++;
				UnitySettings.HideDirectoriesCategory[category.Key] = !EditorGUI.Foldout(GetNextRect(ref YPos), !UnitySettings.HideDirectoriesCategory.TryGetItem(category.Key, true), $"Directories ({category.Key.GetAttribute<EngineCategoryAttribute>().DisplayName})", true);
				IndentLevel--;
				if(UnitySettings.HideDirectoriesCategory[category.Key]) continue;
				YPos += 4;
				foreach (var mode in category.Value) {
					//YPos += 8;
					var displayName = mode.GetAttribute<GameModeAttribute>().DisplayName;
					if (fileMode == FileSystem.Mode.Web) {
						UnitySettings.GameDirectoriesWeb[mode] = EditorField(displayName ?? "N/A", UnitySettings.GameDirectoriesWeb.TryGetItem(mode, String.Empty));
					} else {
						UnitySettings.GameDirectories[mode] = DirectoryField(GetNextRect(ref YPos), displayName ?? "N/A", UnitySettings.GameDirectories.TryGetItem(mode, String.Empty));
					}
				}

				YPos += 8;
			}
		}


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
		UnitySettings.ExportAfterLoad = EditorField("Export After Load", UnitySettings.ExportAfterLoad);

        rect = GetNextRect(ref YPos);
		rect = EditorGUI.PrefixLabel(rect, new GUIContent("Export Flags"));
		Enum exportFlags = UnitySettings.ExportFlags;
        rect = EnumFlagsToggle(rect, ref exportFlags);
        UnitySettings.ExportFlags = (MapExporter.ExportFlags) exportFlags;

        UnitySettings.ExportPath = DirectoryField(GetNextRect(ref YPos), "Export Path", UnitySettings.ExportPath);

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

		/*if (UnitySettings.ExportAfterLoad) {
            UnitySettings.ExportPath = DirectoryField(rect, "Export Path", UnitySettings.ExportPath, includeLabel: false);
        }*/

		// Misc
		DrawHeader(ref YPos, "Miscellaneous Settings");
		UnitySettings.ScreenshotPath = DirectoryField(GetNextRect(ref YPos), "Screenshot Path", UnitySettings.ScreenshotPath);
		UnitySettings.AllowDeadPointers = EditorField("Allow Dead Pointers", UnitySettings.AllowDeadPointers);
		UnitySettings.ForceDisplayBackfaces = EditorField("Force Display Backfaces", UnitySettings.ForceDisplayBackfaces);
		UnitySettings.BlockyMode = EditorField("Blocky Mode", UnitySettings.BlockyMode);
		UnitySettings.TracePointers = EditorField("Trace Pointers (slow!)", UnitySettings.TracePointers);
		UnitySettings.SaveTextures = EditorField("Save Textures", UnitySettings.SaveTextures);
		UnitySettings.ExportText = EditorField("Export Text", UnitySettings.ExportText);
		UnitySettings.UseLevelTranslation = EditorField("Use Level Translation", UnitySettings.UseLevelTranslation);
		UnitySettings.VisualizeSectorBorders = EditorField("Visualize Sector Borders", UnitySettings.VisualizeSectorBorders);
		UnitySettings.CreateFamilyGameObjects = EditorField("Create Family GameObjects", UnitySettings.CreateFamilyGameObjects);
		UnitySettings.ShowCollisionDataForNoCollisionObjects = EditorField("Show Collision Data For NoCollision SPOs", UnitySettings.ShowCollisionDataForNoCollisionObjects);

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

	// Categorized game modes
	public Dictionary<Engine, Dictionary<EngineCategory, IEnumerable<GameModeSelection>>> CategorizedGameModes { get; set; }

	/// <summary>
	/// The file selection dropdown
	/// </summary>
	private GameModeSelectionDropdown GameModeDropdown { get; set; }
	private MapSelectionDropdown MapSelectionDropdown { get; set; }
	private PS1ActorSelectionDropdown ActorDropdown1 { get; set; }
	private PS1ActorSelectionDropdown ActorDropdown2 { get; set; }

	private string CurrentGameDataDir {
		get {
			Dictionary<GameModeSelection, string> GameDirs =
				EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL ? UnitySettings.GameDirectoriesWeb : UnitySettings.GameDirectories;
			if (GameDirs.ContainsKey(UnitySettings.SelectedGameMode)) {
				return (GameDirs[UnitySettings.SelectedGameMode] ?? "");
			} else {
				return "";
			}
		}
	}
	#endregion
}