using OpenSpace;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Settings for Raymap
/// </summary>
#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class UnitySettings {
	private const string editorPrefsPrefix = "Raymap.";
	private const string settingsFile = "Settings.txt";

	public static Dictionary<Settings.Mode, string> GameDirs = new Dictionary<Settings.Mode, string>();
	public static Dictionary<Settings.Mode, string> GameDirsWeb = new Dictionary<Settings.Mode, string>();

	public static Settings.Mode GameMode { get; set; } = Settings.Mode.Rayman2PC;

    public static string MapName { get; set; }
	public static string ProcessName { get; set; }

	// PS1
	public static string Actor1Name { get; set; }
	public static string Actor2Name { get; set; }

	// Misc
	public static string ScreenshotPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Raymap/";
	public static bool LoadFromMemory { get; set; }
	public static bool AllowDeadPointers { get; set; }
	public static bool ForceDisplayBackfaces { get; set; }
	public static bool BlockyMode { get; set; }
	public static bool UseLevelTranslation { get; set; } = true;
	public static bool TracePointers { get; set; } = false;

	// Export
	public static bool ExportText { get; set; }
	public static bool SaveTextures { get; set; }
	public static string ExportPath { get; set; } = "./exports/";
	public static bool ExportAfterLoad { get; set; } // If set to true, exports the map after loading is finished and quits Raymap.
    public static ScreenshotAfterLoadSetting ScreenshotAfterLoad { get; set; } // If set to true, exports the map after loading is finished and quits Raymap.

    /// <summary>
    /// Static constructor loads in editor data at editor startup.
    /// This way, the data loads even if the editor window isn't active.
    /// </summary>
    static UnitySettings() {
		Load();
	}

    public enum ScreenshotAfterLoadSetting {
         None,
         TopDownOnly,
         OrthographicOnly,
         TopDownAndOrthographic
    }

	private static void SerializeSettings(ISerializer s) {
		Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
		foreach (Settings.Mode mode in modes) {
			string dir = GameDirs.ContainsKey(mode) ? GameDirs[mode] : "";
			GameDirs[mode] = s.SerializeString("Directory" + mode.ToString(), dir);
		}
		if (UnityEngine.Application.isEditor) {
			foreach (Settings.Mode mode in modes) {
				string dir = GameDirsWeb.ContainsKey(mode) ? GameDirsWeb[mode] : "";
				GameDirsWeb[mode] = s.SerializeString("WebDirectory" + mode.ToString(), dir);
			}
		}
		string modeString = s.SerializeString("GameMode", GameMode.ToString());
		GameMode = Enum.TryParse(modeString, out Settings.Mode gameMode) ? gameMode : GameMode;
		MapName = s.SerializeString("MapName", MapName);

		// PS1
		Actor1Name = s.SerializeString("Actor1Name", Actor1Name);
		Actor2Name = s.SerializeString("Actor2Name", Actor2Name);

		// Memory loading
		ProcessName = s.SerializeString("ProcessName", ProcessName);
		LoadFromMemory = s.SerializeBool("LoadFromMemory", LoadFromMemory);

		// Export
		ExportPath = s.SerializeString("ExportPath", ExportPath);
		ExportAfterLoad = s.SerializeBool("ExportAfterLoad", ExportAfterLoad);
		string screenshotAfterLoadString = s.SerializeString("ScreenshotAfterLoad", ScreenshotAfterLoad.ToString());
        ScreenshotAfterLoad = Enum.TryParse(screenshotAfterLoadString, out ScreenshotAfterLoadSetting setting) ? setting : ScreenshotAfterLoad;

        // Misc
        ScreenshotPath = s.SerializeString("ScreenshotPath", ScreenshotPath);
		AllowDeadPointers = s.SerializeBool("AllowDeadPointers", AllowDeadPointers);
		ForceDisplayBackfaces = s.SerializeBool("ForceDisplayBackfaces", ForceDisplayBackfaces);
		BlockyMode = s.SerializeBool("BlockyMode", BlockyMode);
		TracePointers = s.SerializeBool("TracePointers", TracePointers);
		SaveTextures = s.SerializeBool("SaveTextures", SaveTextures);
		ExportText = s.SerializeBool("ExportText", ExportText);
		UseLevelTranslation = s.SerializeBool("UseLevelTranslation", UseLevelTranslation);
	}


	/// <summary>
	/// Saves the settings
	/// </summary>
	public static void Save() {
		if (UnityEngine.Application.isEditor) {
#if UNITY_EDITOR
			ISerializer s = new EditorWriteSerializer();
			SerializeSettings(s);
#endif
		} else if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WebGLPlayer) {
			using (SettingsFileWriteSerializer s = new SettingsFileWriteSerializer(settingsFile)) {
				SerializeSettings(s);
			}
		}
	}

	/// <summary>
	/// Loads the settings
	/// </summary>
	public static void Load() {
		if (UnityEngine.Application.isEditor) {
#if UNITY_EDITOR
			ISerializer s = new EditorReadSerializer();
			SerializeSettings(s);
#endif
		} else if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WebGLPlayer) {
			if (!File.Exists(settingsFile)) {
				Save();
			}
			ISerializer s = new SettingsFileReadSerializer(settingsFile);
			SerializeSettings(s);
		}
	}

	private interface ISerializer {
		string SerializeString(string key, string value);
		bool SerializeBool(string key, bool value);
		int SerializeInt(string key, int value);
	}

#if UNITY_EDITOR
	private class EditorReadSerializer : ISerializer {
		public bool SerializeBool(string key, bool value) {
			return UnityEditor.EditorPrefs.GetBool(editorPrefsPrefix + key, value);
		}

		public string SerializeString(string key, string value) {
			return UnityEditor.EditorPrefs.GetString(editorPrefsPrefix + key, value);
		}

		public int SerializeInt(string key, int value) {
			return UnityEditor.EditorPrefs.GetInt(editorPrefsPrefix + key, value);
		}
	}

	private class EditorWriteSerializer : ISerializer {
		public bool SerializeBool(string key, bool value) {
			UnityEditor.EditorPrefs.SetBool(editorPrefsPrefix + key, value);
			return value;
		}

		public string SerializeString(string key, string value) {
			UnityEditor.EditorPrefs.SetString(editorPrefsPrefix + key, value);
			return value;
		}

		public int SerializeInt(string key, int value) {
			UnityEditor.EditorPrefs.SetInt(editorPrefsPrefix + key, value);
			return value;
		}
	}
#endif

	private class SettingsFileWriteSerializer : ISerializer, IDisposable {
		StreamWriter writer;
		public SettingsFileWriteSerializer(string path) {
			writer = new StreamWriter(path);
		}

		public void Dispose() {
			((IDisposable)writer).Dispose();
		}

		public bool SerializeBool(string key, bool value) {
			writer.WriteLine(key + "=" + value.ToString());
			return value;
		}

		public string SerializeString(string key, string value) {
			writer.WriteLine(key + "=" + value.ToString());
			return value;
		}

		public int SerializeInt(string key, int value) {
			writer.WriteLine(key + "=" + value.ToString());
			return value;
		}
	}

	private class SettingsFileReadSerializer : ISerializer {
		Dictionary<string, string> settings = new Dictionary<string, string>();
		public SettingsFileReadSerializer(string path) {
			string[] lines = File.ReadAllLines(path);
			for (int i = 0; i < lines.Length; i++) {
				// Not using split, just in case any of the values contain a =
				int index = lines[i].IndexOf('=');
				if (index >= 0 && index < lines[i].Length) {
					settings.Add(lines[i].Substring(0, index), lines[i].Substring(index + 1));
				}
			}
		}

		public bool SerializeBool(string key, bool value) {
			if (settings.ContainsKey(key)) {
				if (bool.TryParse(settings[key], out bool b)) {
					return b;
				}
			}
			return value;
		}

		public string SerializeString(string key, string value) {
			if (settings.ContainsKey(key)) {
				return settings[key];
			}
			return value;
		}

		public int SerializeInt(string key, int value) {
			if (settings.ContainsKey(key)) {
				if (int.TryParse(settings[key], out int i)) {
					return i;
				}
			}
			return value;
		}
	}
}