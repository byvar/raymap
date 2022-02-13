using BinarySerializer;
using BinarySerializer.Unity;
using Raymap;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class GameModeSelectionDropdown : AdvancedDropdown {
    public GameModeSelectionDropdown(AdvancedDropdownState state) : base(state) {
        minimumSize = new Vector2(50, 500f);
    }

    protected override AdvancedDropdownItem BuildRoot() {
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

        var root = new AdvancedDropdownItem("Game");

        foreach (var engineGroup in modes) {
            var engineNode = new AdvancedDropdownItem(engineGroup.Key.GetAttribute<EngineAttribute>().DisplayName) {
                id = -1
            };

            foreach (var categoryGroup in engineGroup) {
                var categoryNode = new AdvancedDropdownItem(categoryGroup.Key.GetAttribute<EngineCategoryAttribute>().DisplayName) {
                    id = -1
                };
                foreach (var mode in categoryGroup) {
                    categoryNode.AddChild(new AdvancedDropdownItem(mode.GameModeAttribute.DisplayName) {
                        id = (int)mode.GameModeSelection
                    });
                }
                // engineNode.AddSeparator();
                engineNode.AddChild(categoryNode);
            }

            root.AddChild(engineNode);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item) {
        base.ItemSelected(item);

        if (item.id != -1 && ((GameModeSelection)item.id != Selection)) {
            Selection = (GameModeSelection)item.id;
            SelectionName = Selection.GetAttribute<GameModeAttribute>().DisplayName;
            HasChanged = true;
        }
    }

    public bool HasChanged { get; set; }
    public GameModeSelection Selection { get; set; } = UnitySettings.SelectedGameMode;
    public string SelectionName { get; set; } = UnitySettings.SelectedGameMode.GetAttribute<GameModeAttribute>().DisplayName;
}