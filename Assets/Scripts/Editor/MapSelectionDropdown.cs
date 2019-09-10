
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using OpenSpace;
using System.IO;
using UnityEngine;

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
	public string extension;
	public Settings.Mode mode;
	public string[] files;
	public string name;
	//public SerializedProperty property;

	public MapSelectionDropdown(AdvancedDropdownState state) : base(state) {
		minimumSize = new UnityEngine.Vector2(50, 400f);
	}
	public MapSelectionDropdown(AdvancedDropdownState state, string directory) : this(state) {
		this.directory = directory;
		files = FindFiles().ToArray();
	}

	private List<string> FindFiles() {
		// Create the output
		var output = new List<string>();
		
		// If the directory does not exist, return the empty list
		if (!Directory.Exists(directory))
			return output;

		// Add the found files containing the correct file extension
		string extension = null;
		switch (Settings.s.platform) {
			case Settings.Platform.PC:
			case Settings.Platform.iOS:
			case Settings.Platform.GC:
				if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
					extension = "*.sna";
				} else {
					extension = "*.lvl";
				}
				break;
			case Settings.Platform.DC: extension = "*.DAT"; break;
			case Settings.Platform.PS2: extension = "*.lv2"; break;
		}
		if (extension != null) {
			output.AddRange(
				from file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories)
				let filename = Path.GetFileNameWithoutExtension(file)
				where ((!filterPaths.Contains(filename.ToLower()))
				&& new DirectoryInfo(file).Parent.Name.ToLower() == filename.ToLower())
				select filename
				
			);
		}

		// Return the output
		return output;
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new AdvancedDropdownItem(name);
		for (int i = 0; i < files.Length; i++) {
			Add(root, files[i], files[i], i);
		}

		return root;
	}

	protected void Add(AdvancedDropdownItem parent, string path, string fullPath, int id) {
		if (path.Contains("/")) {
			// Folder
			string folder = path.Substring(0, path.IndexOf("/"));
			string rest = path.Substring(path.IndexOf("/") + 1);
			AdvancedDropdownItem folderNode = parent.children.FirstOrDefault(c => c.name == folder);
			if (folderNode == null) {
				folderNode = new AdvancedDropdownItem(folder);
				parent.AddChild(folderNode);
			}
			Add(folderNode, rest, fullPath, id);
		} else {
			// File
			parent.AddChild(new AdvancedDropdownItem(path) {
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
