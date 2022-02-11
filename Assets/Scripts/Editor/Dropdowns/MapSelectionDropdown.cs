
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using OpenSpace;
using System.IO;
using UnityEngine;
using OpenSpace.Loader;

class MapSelectionDropdown : AdvancedDropdown {
	public static string[] filterPaths = new string[] {
		"fix",
		/*"ani0",
		"ani1",
		"ani2",
		"ani3",
		"ani4",
		"ani5",
		"ani6",
		"ani7",
		"ani8",
		"ani9",
		"ani10",*/
	};
	public string selection = null;
	public string directory;
	public CPA_Settings.Mode mode;
	public string[] files;
	public string[] translatedFiles;
	public string name;
	//public SerializedProperty property;

	public MapSelectionDropdown(AdvancedDropdownState state) : base(state) {
		minimumSize = new UnityEngine.Vector2(50, 400f);
	}
	public MapSelectionDropdown(AdvancedDropdownState state, string directory) : this(state) {
		this.directory = directory;
		BuildFileList();
	}

	private void BuildFileList() {
		List<string> filesUnprocessed = FindFiles();
		List<string> filesSorted = filesUnprocessed;
		if (UnitySettings.UseLevelTranslation && CPA_Settings.s.levelTranslation != null) {
			filesSorted = CPA_Settings.s.levelTranslation.Sort(filesSorted);
			translatedFiles = CPA_Settings.s.levelTranslation.Translate(filesSorted).ToArray();
		}
		files = filesSorted.ToArray();
	}

	private List<string> FindFiles() {
		// Create the output
		var output = new List<string>();
		
		// If the directory does not exist, return the empty list
		if (!Directory.Exists(directory))
			return output;

		// Add the found files containing the correct file extension
		string extension = null;
		string[] levels;
		switch (CPA_Settings.s.platform) {
			case CPA_Settings.Platform.PC:
			case CPA_Settings.Platform.iOS:
			case CPA_Settings.Platform.GC:
			case CPA_Settings.Platform.Xbox:
			case CPA_Settings.Platform.Xbox360:
			case CPA_Settings.Platform.PS3:
			case CPA_Settings.Platform.MacOS:
				if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
					extension = "*.sna";
				} else {
					extension = "*.lvl";
				}
				break;
			case CPA_Settings.Platform.DC: extension = "*.DAT"; break;
			case CPA_Settings.Platform.PS2:
				if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
					if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R2) {
						extension = "*.sna";
					} else {
						extension = "*.lv2";
					}
				} else {
					extension = "*.lvl";
				}
				break;
			case CPA_Settings.Platform.PS1:
				MapLoader.Reset();
				R2PS1Loader l1 = MapLoader.Loader as R2PS1Loader;
				l1.gameDataBinFolder = directory;
				levels = l1.LoadLevelList();
				MapLoader.Reset();
				output.AddRange(levels);
				break;
			case CPA_Settings.Platform.DS:
			case CPA_Settings.Platform.N64:
			case CPA_Settings.Platform._3DS:
				MapLoader.Reset();
				R2ROMLoader lr = MapLoader.Loader as R2ROMLoader;
				lr.gameDataBinFolder = directory;
				levels = lr.LoadLevelList();
				MapLoader.Reset();
				output.AddRange(levels);
				break;
		}
		if (extension != null) {
			output.AddRange(
				from file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories)
				let filename = Path.GetFileNameWithoutExtension(file)
				let dirname = new DirectoryInfo(file).Parent.Name
				where ((!filterPaths.Contains(filename.ToLower()))
				&& dirname.ToLower() == filename.ToLower())
				select dirname
				
			);
		}
		//Debug.Log(string.Join("\n",output));

		// Return the output
		return output;
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new AdvancedDropdownItem(name);
		for (int i = 0; i < files.Length; i++) {
			Add(root, files[i], files[i], translatedFiles?[i], i);
		}

		return root;
	}

	protected void Add(AdvancedDropdownItem parent, string path, string fullPath, string translation, int id) {
		if (path.Contains("/")) {
			// Folder
			string folder = path.Substring(0, path.IndexOf("/"));
			string rest = path.Substring(path.IndexOf("/") + 1);
			AdvancedDropdownItem folderNode = parent.children.FirstOrDefault(c => c.name == folder);
			if (folderNode == null) {
				folderNode = new AdvancedDropdownItem(folder);
				parent.AddChild(folderNode);
			}
			Add(folderNode, rest, fullPath, translation, id);
		} else {
			// File
			parent.AddChild(new AdvancedDropdownItem(translation ?? path) {
				id = id
			});
		}
	}

	protected override void ItemSelected(AdvancedDropdownItem item) {
		base.ItemSelected(item);
		if (item.children.Count() == 0 && files != null && files.Length > item.id) {
			selection = files[item.id];
			//property.stringValue = selection;
		}
	}
}
