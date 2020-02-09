using OpenSpace;
using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Settings for Raymap
/// </summary>
[InitializeOnLoad]
public class UnitySettings {
	private static string editorPrefsPrefix = "Raymap.";
	public static Dictionary<Settings.Mode, string> GameDirs = new Dictionary<Settings.Mode, string>();
	public static Dictionary<Settings.Mode, string> GameDirsWeb = new Dictionary<Settings.Mode, string>();

	public static Settings.Mode GameMode { get; set; } = Settings.Mode.Rayman3PC;

    public static string MapName { get; set; }
	public static string ProcessName { get; set; }

	// Misc
	public static string ScreenshotPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Raymap/";
	public static bool LoadFromMemory { get; set; }
	public static bool AllowDeadPointers { get; set; }
	public static bool ForceDisplayBackfaces { get; set; }
	public static bool BlockyMode { get; set; }
	public static bool SaveTextures { get; set; }
	public static string ExportPath { get; set; } = "./exports/";
	public static bool ExportAfterLoad { get; set; } // If set to true, exports the map after loading is finished and quits Raymap.

	/// <summary>
	/// Saves the settings
	/// </summary>
	public static void Save() {
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirs.ContainsKey(mode) ? GameDirs[mode] : "";
			EditorPrefs.SetString(editorPrefsPrefix + "Directory" + mode.ToString(), dir);
		}
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirsWeb.ContainsKey(mode) ? GameDirsWeb[mode] : "";
			EditorPrefs.SetString(editorPrefsPrefix + "WebDirectory" + mode.ToString(), dir);
		}
		EditorPrefs.SetString(editorPrefsPrefix + "GameMode", GameMode.ToString());
        EditorPrefs.SetString(editorPrefsPrefix + "MapName", MapName);

		// Memory loading
		EditorPrefs.SetString(editorPrefsPrefix + "ProcessName", ProcessName);
		EditorPrefs.SetBool(editorPrefsPrefix + "LoadFromMemory", LoadFromMemory);

		// Export
		EditorPrefs.SetString(editorPrefsPrefix + "ExportPath", ExportPath);
		EditorPrefs.SetBool(editorPrefsPrefix + "ExportAfterLoad", ExportAfterLoad);

		// Misc
		EditorPrefs.SetString(editorPrefsPrefix + "ScreenshotPath", ScreenshotPath);
		EditorPrefs.SetBool(editorPrefsPrefix + "AllowDeadPointers", AllowDeadPointers);
		EditorPrefs.SetBool(editorPrefsPrefix + "ForceDisplayBackfaces", ForceDisplayBackfaces);
		EditorPrefs.SetBool(editorPrefsPrefix + "BlockyMode", BlockyMode);
		EditorPrefs.SetBool(editorPrefsPrefix + "SaveTextures", SaveTextures);
	}

	/// <summary>
	/// Static constructor loads in editor data at editor startup.
	/// This way, the data loads even if the editor window isn't active.
	/// </summary>
	static UnitySettings() {
		Load();
	}

    /// <summary>
    /// Loads the settings
    /// </summary>
    public static void Load()
    {
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirs.ContainsKey(mode) ? GameDirs[mode] : "";
			GameDirs[mode] = EditorPrefs.GetString(editorPrefsPrefix + "Directory" + mode.ToString(), dir);
		}
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirsWeb.ContainsKey(mode) ? GameDirsWeb[mode] : "";
			GameDirsWeb[mode] = EditorPrefs.GetString(editorPrefsPrefix + "WebDirectory" + mode.ToString(), dir);
		}

		string modeString = EditorPrefs.GetString(editorPrefsPrefix + "GameMode", GameMode.ToString());
		GameMode = Enum.TryParse(modeString, out Settings.Mode gameMode) ? gameMode : GameMode;
        MapName = EditorPrefs.GetString(editorPrefsPrefix + "MapName", MapName);

		// Memory loading
		ProcessName = EditorPrefs.GetString(editorPrefsPrefix + "ProcessName", ProcessName);
		LoadFromMemory = EditorPrefs.GetBool(editorPrefsPrefix + "LoadFromMemory", LoadFromMemory);

		// Export
		ExportPath = EditorPrefs.GetString(editorPrefsPrefix + "ExportPath", ExportPath);
		ExportAfterLoad = EditorPrefs.GetBool(editorPrefsPrefix + "ExportAfterLoad", ExportAfterLoad);

		// Misc
		ScreenshotPath = EditorPrefs.GetString(editorPrefsPrefix + "ScreenshotPath", ScreenshotPath);
		AllowDeadPointers = EditorPrefs.GetBool(editorPrefsPrefix + "AllowDeadPointers", AllowDeadPointers);
		ForceDisplayBackfaces = EditorPrefs.GetBool(editorPrefsPrefix + "ForceDisplayBackfaces", ForceDisplayBackfaces);
		BlockyMode = EditorPrefs.GetBool(editorPrefsPrefix + "BlockyMode", BlockyMode);
		SaveTextures = EditorPrefs.GetBool(editorPrefsPrefix + "SaveTextures", SaveTextures);
	}
}