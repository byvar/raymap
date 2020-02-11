
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OpenSpace;
using OpenSpace.Text;

class LocalizationDropdown : AdvancedDropdown {
	public int? selection = null;
	public string name;
	public Rect rect;

	public LocalizationDropdown(AdvancedDropdownState state) : base(state) {
		minimumSize = new UnityEngine.Vector2(400f, 400f);
	}

	protected override AdvancedDropdownItem BuildRoot() {
		var root = new AdvancedDropdownItem(name);
		LocalizationStructure loc = MapLoader.Loader.localization;
		root.AddChild(new AdvancedDropdownItem("-1 - None"));
		if (loc != null) {
			if (loc.languages != null && loc.languages.Length > 0 && loc.languages[0].entries.Length > 0) {
				AdvancedDropdownItem lang = new AdvancedDropdownItem("Localized Text");
				root.AddChild(lang);
				for (int i = 0; i < loc.languages[0].entries.Length; i++) {
					lang.AddChild(new AdvancedDropdownItem(
							i.ToString() // ID
							+ " - "
							+ (loc.languages[0].entries[i] == null ? "null" : loc.languages[0].entries[i].Replace("\n", "\\n")) // Text
						));
				}
			}
			if (loc.misc.entries != null && loc.misc.entries.Length > 0) {
				AdvancedDropdownItem misc = new AdvancedDropdownItem("Misc Text");
				root.AddChild(misc);
				for (int i = 0; i < loc.misc.entries.Length; i++) {
					misc.AddChild(new AdvancedDropdownItem(
							(20000 + i).ToString() // ID
							+ " - "
							+ loc.misc.entries[i].Replace("\n", "\\n") // Text
						));
				}
			}
		} else if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
			OpenSpace.Loader.R2ROMLoader l = MapLoader.Loader as OpenSpace.Loader.R2ROMLoader;
			OpenSpace.ROM.LanguageTable[] langs = l.localizationROM?.languageTables;
			if (langs != null && langs.Length > 0) {
				int i = 0;
				AdvancedDropdownItem misc = new AdvancedDropdownItem(langs[0].name);
				root.AddChild(misc);
				for (int j = 0; j < langs[0].num_txtTable; j++) {
					misc.AddChild(new AdvancedDropdownItem(
							i.ToString() // ID
							+ " - "
							+ langs[0].textTable.Value.strings[j].Value?.ToString().Replace("\n", "\\n") // Text
						));
					i++;
				}
				for (int j = 0; j < langs[0].num_binaryTable; j++) {
					misc.AddChild(new AdvancedDropdownItem(
							i.ToString() // ID
							+ " - "
							+ langs[0].binaryTable.Value.strings[j].Value?.ToString().Replace("\n", "\\n") // Text
						));
					i++;
				}
				if (langs.Length > 1) {
					AdvancedDropdownItem lang = new AdvancedDropdownItem(langs[1].name);
					root.AddChild(lang);
					for (int j = 0; j < langs[1].num_txtTable; j++) {
						lang.AddChild(new AdvancedDropdownItem(
								i.ToString() // ID
								+ " - "
								+ langs[1].textTable.Value.strings[j].Value?.ToString().Replace("\n", "\\n") // Text
							));
						i++;
					}
					for (int j = 0; j < langs[1].num_binaryTable; j++) {
						lang.AddChild(new AdvancedDropdownItem(
								i.ToString() // ID
								+ " - "
								+ langs[1].binaryTable.Value.strings[j].Value?.ToString().Replace("\n", "\\n") // Text
							));
						i++;
					}
				}
			}
		}

		return root;
	}

	protected override void ItemSelected(AdvancedDropdownItem item) {
		base.ItemSelected(item);
		if (item.children.Count() == 0) {
			string keyStr = item.name.Substring(0, item.name.IndexOf(' '));
			int key = int.Parse(keyStr);
			selection = key;
		}
	}
}
