
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
		if (loc != null && loc.languages != null && loc.languages.Length > 0 && loc.languages[0].entries.Length > 0) {
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
