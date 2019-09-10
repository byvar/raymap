using OpenSpace;
using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Settings for Raymap
/// </summary>
[InitializeOnLoad]
public class UnitySettings
{
	public static Dictionary<Settings.Mode, string> GameDirs = new Dictionary<Settings.Mode, string>();
	public static Dictionary<Settings.Mode, string> GameDirsWeb = new Dictionary<Settings.Mode, string>();

	public static Settings.Mode GameMode { get; set; } = Settings.Mode.Rayman3PC;

    public static string MapName { get; set; }
	public static string ProcessName { get; set; }

	// Misc
	public static bool LoadFromMemory { get; set; }
	public static bool AllowDeadPointers { get; set; }
	public static bool ForceDisplayBackfaces { get; set; }
	public static bool BlockyMode { get; set; }
	public static bool SaveTextures { get; set; }
	public static string ExportPath { get; set; } = ".\\exports\\";
	public static bool ExportAfterLoad { get; set; } // If set to true, exports the map after loading is finished and quits Raymap.

	/// <summary>
	/// Saves the settings
	/// </summary>
	public static void Save() {
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirs.ContainsKey(mode) ? GameDirs[mode] : "";
			EditorPrefs.SetString("Directory" + mode.ToString(), dir);
		}
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirsWeb.ContainsKey(mode) ? GameDirsWeb[mode] : "";
			EditorPrefs.SetString("WebDirectory" + mode.ToString(), dir);
		}
		EditorPrefs.SetString("GameMode", GameMode.ToString());
        EditorPrefs.SetString("MapName", MapName);

		// Memory loading
		EditorPrefs.SetString("ProcessName", ProcessName);
		EditorPrefs.SetBool("LoadFromMemory", LoadFromMemory);

		// Export
		EditorPrefs.SetString("ExportPath", ExportPath);
		EditorPrefs.SetBool("ExportAfterLoad", ExportAfterLoad);

		// Misc
		EditorPrefs.SetBool("AllowDeadPointers", AllowDeadPointers);
		EditorPrefs.SetBool("ForceDisplayBackfaces", ForceDisplayBackfaces);
		EditorPrefs.SetBool("BlockyMode", BlockyMode);
		EditorPrefs.SetBool("SaveTextures", SaveTextures);
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
			GameDirs[mode] = EditorPrefs.GetString("Directory" + mode.ToString(), dir);
		}
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirsWeb.ContainsKey(mode) ? GameDirsWeb[mode] : "";
			GameDirsWeb[mode] = EditorPrefs.GetString("WebDirectory" + mode.ToString(), dir);
		}
		GameMode = Enum.TryParse(EditorPrefs.GetString("GameMode", GameMode.ToString()), out Settings.Mode gameMode) ? gameMode : GameMode;
        MapName = EditorPrefs.GetString("MapName", MapName);

		// Memory loading
		ProcessName = EditorPrefs.GetString("ProcessName", ProcessName);
		LoadFromMemory = EditorPrefs.GetBool("LoadFromMemory", LoadFromMemory);

		// Export
		ExportPath = EditorPrefs.GetString("ExportPath", ExportPath);
		ExportAfterLoad = EditorPrefs.GetBool("ExportAfterLoad", ExportAfterLoad);

		// Misc
		AllowDeadPointers = EditorPrefs.GetBool("AllowDeadPointers", AllowDeadPointers);
		ForceDisplayBackfaces = EditorPrefs.GetBool("ForceDisplayBackfaces", ForceDisplayBackfaces);
		BlockyMode = EditorPrefs.GetBool("BlockyMode", BlockyMode);
		SaveTextures = EditorPrefs.GetBool("SaveTextures", SaveTextures);
	}
}