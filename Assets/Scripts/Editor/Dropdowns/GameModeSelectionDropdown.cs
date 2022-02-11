using BinarySerializer;
using BinarySerializer.Unity;
using OpenSpace;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class GameModeSelectionDropdown : AdvancedDropdown {
    public GameModeSelectionDropdown(AdvancedDropdownState state) : base(state) {
        minimumSize = new Vector2(50, 500f);
    }

    protected override AdvancedDropdownItem BuildRoot() {
        var modes = EnumHelpers.GetValues<CPA_Settings.Mode>().Select(x => new {
            Mode = x,
            Settings = CPA_Settings.GetSettings(x)
        }).GroupBy(x => x.Settings.engineVersion);

        var root = new AdvancedDropdownItem("Game");

        foreach (var mode in modes) {
            var group = new AdvancedDropdownItem(mode.Key.ToString()) {
                id = -1
            };

            foreach (var selectionGroup in mode.GroupBy(x => x.Settings.game)) {
                foreach (var selection in selectionGroup) {
                    group.AddChild(new AdvancedDropdownItem(selection.Settings.DisplayName) {
                        id = (int)selection.Mode
                    });
                }

                group.AddSeparator();
            }

            root.AddChild(group);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item) {
        base.ItemSelected(item);

        if (item.id != -1 && ((CPA_Settings.Mode)item.id != Selection)) {
            Selection = (CPA_Settings.Mode)item.id;
            SelectionName = Selection.GetDescription();
            HasChanged = true;
        }
    }

    public bool HasChanged { get; set; }
    public CPA_Settings.Mode Selection { get; set; } = UnitySettings.GameMode;
    public string SelectionName { get; set; } = UnitySettings.GameMode.GetDescription();
}