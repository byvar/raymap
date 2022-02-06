using OpenSpace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenSpace.Exporter;
using UnityEngine;
using BinarySerializer;

/// <summary>
/// Settings for Raymap
/// </summary>
#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class UnitySettings {
	private const string editorPrefsPrefix = "Raymap.";
	private const string settingsFile = "Settings.txt";
	public static bool IsRaymapGame { get; set; } = false;

	public static Dictionary<Settings.Mode, string> GameDirectories = new Dictionary<Settings.Mode, string>();
	public static Dictionary<Settings.Mode, string> GameDirectoriesWeb = new Dictionary<Settings.Mode, string>();

    public static Dictionary<Settings.EngineVersion, bool> HideDirectories { get; set; } = new Dictionary<Settings.EngineVersion, bool>();


    /// <summary>
    /// Serialization log file
    /// </summary>
    public static string LogFile { get; set; }

    /// <summary>
    /// Whether to log to the serialization log file
    /// </summary>
    public static bool Log { get; set; }

    /// <summary>
    /// Indicates if .BAK backup files should be created before writing
    /// </summary>
    public static bool BackupFiles { get; set; }

    /// <summary>
    /// Gets the current directory based on the selected mode
    /// </summary>
    public static string CurrentDirectory {
        get {
            if (FileSystem.mode == FileSystem.Mode.Web) {
                return GameDirectoriesWeb.TryGetValue(GameMode, out string value) ? value : String.Empty;
            } else {
                return GameDirectories.TryGetValue(GameMode, out string value) ? value : String.Empty;
            }
        }
    }

    public static Settings.Mode GameMode { get; set; } = Settings.Mode.Rayman2PC;

    public static string MapName { get; set; }
	public static string ProcessName { get; set; }

	// PS1
	public static string Actor1Name { get; set; }
	public static string Actor2Name { get; set; }
	public static bool ExportPS1Files { get; set; } = false;

	// Misc
	public static string ScreenshotPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/Raymap/";
	public static bool LoadFromMemory { get; set; }
	public static bool AllowDeadPointers { get; set; }
	public static bool ForceDisplayBackfaces { get; set; }
	public static bool BlockyMode { get; set; }
	public static bool UseLevelTranslation { get; set; } = true;
	public static bool TracePointers { get; set; } = false;
	public static bool VisualizeSectorBorders { get; set; } = false;
	public static bool CreateFamilyGameObjects { get; set; } = false;
    public static bool ShowCollisionDataForNoCollisionObjects { get; set; } = false;

    // Export
    public static bool ExportText { get; set; }
	public static bool SaveTextures { get; set; }
	public static string ExportPath { get; set; } = "./exports/";
	public static bool ExportAfterLoad { get; set; } // If set to true, exports the map after loading is finished and quits Raymap.
    public static MapExporter.ExportFlags ExportFlags { get; set; }
    public static ScreenshotAfterLoadSetting ScreenshotAfterLoad { get; set; } // If set to true, screenshots the map after loading is finished and quits Raymap.
    public static float ScreenshotScale { get; set; }
    public static string HighlightObjectsFilter { get; set; }
    public static string HighlightObjectsTextFormat { get; set; }

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

	private static void SerializeSettings(ISerializer s, bool cmdLine = false) {
        if (!cmdLine) {
            Settings.Mode[] modes = (Settings.Mode[])Enum.GetValues(typeof(Settings.Mode));
            foreach (Settings.Mode mode in modes) {
                string dir = GameDirectories.ContainsKey(mode) ? GameDirectories[mode] : "";
                GameDirectories[mode] = s.SerializeString("Directory" + mode.ToString(), dir);
            }
            if (UnityEngine.Application.isEditor) {
                foreach (Settings.Mode mode in modes) {
                    string dir = GameDirectoriesWeb.ContainsKey(mode) ? GameDirectoriesWeb[mode] : "";
                    GameDirectoriesWeb[mode] = s.SerializeString("WebDirectory" + mode.ToString(), dir);
                }
            }
        }
		string modeString = s.SerializeString("GameMode", GameMode.ToString(), "mode", "m");
        GameMode = Enum.TryParse(modeString, out Settings.Mode gameMode) ? gameMode : GameMode;
        if (cmdLine) {
            if (Settings.cmdModeNameDict.ContainsKey(modeString)) {
                GameMode = Settings.cmdModeNameDict[modeString];
            }
            if (FileSystem.mode == FileSystem.Mode.Web) {
                string dir = GameDirectoriesWeb.ContainsKey(GameMode) ? GameDirectoriesWeb[GameMode] : "";
                GameDirectoriesWeb[GameMode] = s.SerializeString("WebDirectory", dir, "dir", "directory", "folder", "f", "d");
            } else {
                string dir = GameDirectories.ContainsKey(GameMode) ? GameDirectories[GameMode] : "";
                GameDirectories[GameMode] = s.SerializeString("Directory", dir, "dir", "directory", "folder", "f", "d");
            }
        }
        MapName = s.SerializeString("MapName", MapName, "level", "lvl", "map");

        // PS1
        Actor1Name = s.SerializeString("Actor1Name", Actor1Name, "a1", "act1", "actor1");
		Actor2Name = s.SerializeString("Actor2Name", Actor2Name, "a2", "act2", "actor2");
		ExportPS1Files = s.SerializeBool("ExportPS1Files", ExportPS1Files);

		// Memory loading
		ProcessName = s.SerializeString("ProcessName", ProcessName);
		LoadFromMemory = s.SerializeBool("LoadFromMemory", LoadFromMemory);

        // Export
        if (cmdLine) {
            string p = s.SerializeString("export", null, "export");
            if (!string.IsNullOrEmpty(p)) {
                ExportAfterLoad = true;
                ExportPath = p;
                ExportFlags = MapExporter.ExportFlags.All;

                string f = s.SerializeString("flags", null, "flags");
                if (!string.IsNullOrEmpty(f)) {
                    int.TryParse(f, out var flagsInt);
                    ExportFlags = (MapExporter.ExportFlags) flagsInt;
                }
            }
        }
        ExportPath = s.SerializeString("ExportPath", ExportPath);
		ExportAfterLoad = s.SerializeBool("ExportAfterLoad", ExportAfterLoad);
        ExportFlags = (MapExporter.ExportFlags)s.SerializeInt("ExportFlags", (int)ExportFlags);

        if (cmdLine) {
            string p = s.SerializeString("screenshot", null);
            if (!string.IsNullOrEmpty(p)) {
                ScreenshotAfterLoad = ScreenshotAfterLoadSetting.TopDownAndOrthographic;
                ScreenshotPath = p;
            }
        }
        string screenshotAfterLoadString = s.SerializeString("ScreenshotAfterLoad", ScreenshotAfterLoad.ToString(), "ScreenshotAfterLoad");
        ScreenshotAfterLoad = Enum.TryParse(screenshotAfterLoadString, out ScreenshotAfterLoadSetting setting) ? setting : ScreenshotAfterLoad;

        float.TryParse(s.SerializeString("ScreenshotScale", ScreenshotScale.ToString(CultureInfo.InvariantCulture), "ScreenshotScale"), out float screenshotScale);
        ScreenshotScale = screenshotScale;

        HighlightObjectsFilter = s.SerializeString("HighlightObjectsFilter", HighlightObjectsFilter, "HighlightObjectsFilter");
        HighlightObjectsTextFormat = s.SerializeString("HighlightObjectsTextFormat", HighlightObjectsTextFormat, "HighlightObjectsTextFormat");

        // Misc
        ScreenshotPath = s.SerializeString("ScreenshotPath", ScreenshotPath, "ScreenshotPath");
		AllowDeadPointers = s.SerializeBool("AllowDeadPointers", AllowDeadPointers, "allowDeadPointers");
		ForceDisplayBackfaces = s.SerializeBool("ForceDisplayBackfaces", ForceDisplayBackfaces);
		BlockyMode = s.SerializeBool("BlockyMode", BlockyMode);
		TracePointers = s.SerializeBool("TracePointers", TracePointers);
		SaveTextures = s.SerializeBool("SaveTextures", SaveTextures);
		ExportText = s.SerializeBool("ExportText", ExportText);
		UseLevelTranslation = s.SerializeBool("UseLevelTranslation", UseLevelTranslation);
		VisualizeSectorBorders = s.SerializeBool("VisualizeSectorBorders", VisualizeSectorBorders);
		CreateFamilyGameObjects = s.SerializeBool("CreateFamilyGameObjects", CreateFamilyGameObjects);
		ShowCollisionDataForNoCollisionObjects = s.SerializeBool("ShowCollisionDataForNoCollisionObjects", ShowCollisionDataForNoCollisionObjects);



        Settings.EngineVersion[] engines = EnumHelpers.GetValues<Settings.EngineVersion>();
        foreach (Settings.EngineVersion engine in engines) {
            bool v = HideDirectories.ContainsKey(engine) && HideDirectories[engine];
            HideDirectories[engine] = s.SerializeBool("HideDirectory" + engine.ToString(), v);
        }
        Log = s.SerializeBool("Log", Log);
        LogFile = s.SerializeString("LogFile", LogFile);
        BackupFiles = s.SerializeBool("BackupFiles", BackupFiles);
    }


    /// <summary>
    /// Saves the settings
    /// </summary>
    public static void Save() {
        if (Application.isEditor) {
#if UNITY_EDITOR
            ISerializer s = new EditorWriteSerializer();
            SerializeSettings(s);
#endif
        } else if (Application.platform != RuntimePlatform.WebGLPlayer) {
            using (SettingsFileWriteSerializer s = new SettingsFileWriteSerializer(settingsFile)) {
                SerializeSettings(s);
            }
        }
    }

    /// <summary>
    /// Loads the settings
    /// </summary>
    public static void Load() {
        if (Application.isEditor) {
#if UNITY_EDITOR
            ISerializer s = new EditorReadSerializer();
            SerializeSettings(s);
#endif
        } else if (Application.platform != RuntimePlatform.WebGLPlayer) {
            if (!File.Exists(settingsFile)) {
                Save();
            }
            ISerializer s = new SettingsFileReadSerializer(settingsFile);
            SerializeSettings(s);
        }
        ConfigureFileSystem();
        if (!Application.isEditor) {
            ParseCommandLineArguments();
        }
    }

    public static void ConfigureFileSystem() {

        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            FileSystem.mode = FileSystem.Mode.Web;
        }
#if UNITY_EDITOR
        if (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL) {
            FileSystem.mode = FileSystem.Mode.Web;
        }
#endif
    }

    static void ParseCommandLineArguments() {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            // Read URL arguments
            ISerializer s = new WebArgumentsReadSerializer();
            SerializeSettings(s, cmdLine: true);
        } else {
            // Read command line arguments
            ISerializer s = new CmdLineReadSerializer();
            SerializeSettings(s, cmdLine: true);
        }
    }


    #region Subclasses (Settings serialization)

    private interface ISerializer {
        string SerializeString(string key, string value, params string[] cmdlineKeys);
        bool SerializeBool(string key, bool value, params string[] cmdlineKeys);
        int SerializeInt(string key, int value, params string[] cmdlineKeys);
    }

#if UNITY_EDITOR
    private class EditorReadSerializer : ISerializer {
        public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
            return UnityEditor.EditorPrefs.GetBool(editorPrefsPrefix + key, value);
        }

        public string SerializeString(string key, string value, params string[] cmdlineKeys) {
            return UnityEditor.EditorPrefs.GetString(editorPrefsPrefix + key, value);
        }

        public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
            return UnityEditor.EditorPrefs.GetInt(editorPrefsPrefix + key, value);
        }
    }

    private class EditorWriteSerializer : ISerializer {
        public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
            UnityEditor.EditorPrefs.SetBool(editorPrefsPrefix + key, value);
            return value;
        }

        public string SerializeString(string key, string value, params string[] cmdlineKeys) {
            UnityEditor.EditorPrefs.SetString(editorPrefsPrefix + key, value);
            return value;
        }

        public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
            UnityEditor.EditorPrefs.SetInt(editorPrefsPrefix + key, value);
            return value;
        }
    }
#endif

    private class CmdLineReadSerializer : ISerializer {
        string[] args;
        public CmdLineReadSerializer() {
            args = Environment.GetCommandLineArgs();
        }

        public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
            if (args == null || args.Length == 0 || cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
            for (int c = 0; c < cmdlineKeys.Length; c++) {
                string cmdKey = cmdlineKeys[c];
                if (cmdKey.Length == 1) {
                    cmdKey = "-" + cmdKey;
                } else {
                    cmdKey = "--" + cmdKey;
                }
                int ind = Array.IndexOf(args, cmdKey);
                if (ind > -1 && ind + 1 < args.Length) {
                    if (bool.TryParse(args[ind + 1], out bool b)) {
                        return b;
                    }
                }
            }
            return value;
        }

        public string SerializeString(string key, string value, params string[] cmdlineKeys) {
            if (args == null || args.Length == 0 || cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
            for (int c = 0; c < cmdlineKeys.Length; c++) {
                string cmdKey = cmdlineKeys[c];
                if (cmdKey.Length == 1) {
                    cmdKey = "-" + cmdKey;
                } else {
                    cmdKey = "--" + cmdKey;
                }
                int ind = Array.IndexOf(args, cmdKey);
                if (ind > -1 && ind + 1 < args.Length) {
                    return args[ind + 1];
                }
            }
            return value;
        }

        public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
            if (args == null || args.Length == 0 || cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
            for (int c = 0; c < cmdlineKeys.Length; c++) {
                string cmdKey = cmdlineKeys[c];
                if (cmdKey.Length == 1) {
                    cmdKey = "-" + cmdKey;
                } else {
                    cmdKey = "--" + cmdKey;
                }
                int ind = Array.IndexOf(args, cmdKey);
                if (ind > -1 && ind + 1 < args.Length) {
                    if (int.TryParse(args[ind + 1], out int val)) {
                        return val;
                    }
                }
            }
            return value;
        }
    }

    private class WebArgumentsReadSerializer : ISerializer {
        Dictionary<string, string> settings = new Dictionary<string, string>();
        public WebArgumentsReadSerializer() {
            string url = Application.absoluteURL;
            if (url.IndexOf('?') > 0) {
                string urlArgsStr = url.Split('?')[1].Split('#')[0];
                if (urlArgsStr.Length > 0) {
                    string[] urlArgs = urlArgsStr.Split('&');
                    foreach (string arg in urlArgs) {
                        string[] argKeyVal = arg.Split('=');
                        if (argKeyVal.Length > 1) {
                            settings.Add(argKeyVal[0], argKeyVal[1]);
                        }
                    }
                }
            }
        }

        public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
            if (settings.ContainsKey(key)) {
                if (bool.TryParse(settings[key], out bool b)) {
                    return b;
                }
            }
            if (cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
            foreach (string cmdKey in cmdlineKeys) {
                if (settings.ContainsKey(cmdKey)) {
                    if (bool.TryParse(settings[cmdKey], out bool b)) {
                        return b;
                    }
                }
            }
            return value;
        }

        public string SerializeString(string key, string value, params string[] cmdlineKeys) {
            if (settings.ContainsKey(key)) {
                return settings[key];
            }
            if (cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
            foreach (string cmdKey in cmdlineKeys) {
                if (settings.ContainsKey(cmdKey)) {
                    return settings[cmdKey];
                }
            }
            return value;
        }

        public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
            if (settings.ContainsKey(key)) {
                if (int.TryParse(settings[key], out int i)) {
                    return i;
                }
            }
            if (cmdlineKeys == null || cmdlineKeys.Length == 0) return value;
            foreach (string cmdKey in cmdlineKeys) {
                if (settings.ContainsKey(cmdKey)) {
                    if (int.TryParse(settings[cmdKey], out int i)) {
                        return i;
                    }
                }
            }
            return value;
        }
    }

    private class SettingsFileWriteSerializer : ISerializer, IDisposable {
        StreamWriter writer;
        public SettingsFileWriteSerializer(string path) {
            writer = new StreamWriter(path);
        }

        public void Dispose() {
            writer?.Flush();
            ((IDisposable)writer)?.Dispose();
        }

        public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
            writer.WriteLine(key + "=" + value.ToString());
            return value;
        }

        public string SerializeString(string key, string value, params string[] cmdlineKeys) {
            writer.WriteLine(key + "=" + value.ToString());
            return value;
        }

        public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
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

        public bool SerializeBool(string key, bool value, params string[] cmdlineKeys) {
            if (settings.ContainsKey(key)) {
                if (bool.TryParse(settings[key], out bool b)) {
                    return b;
                }
            }
            return value;
        }

        public string SerializeString(string key, string value, params string[] cmdlineKeys) {
            if (settings.ContainsKey(key)) {
                return settings[key];
            }
            return value;
        }

        public int SerializeInt(string key, int value, params string[] cmdlineKeys) {
            if (settings.ContainsKey(key)) {
                if (int.TryParse(settings[key], out int i)) {
                    return i;
                }
            }
            return value;
        }
    }
    #endregion
}