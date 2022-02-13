using Raymap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class MapSelectionDropdown : AdvancedDropdown {
    public MapSelectionDropdown(AdvancedDropdownState state, MapTreeNode mapTree) : base(state) {
        MapTree = mapTree;
        FlattenedMapTree = MapTree.GetMaps().ToArray();
        SelectedMap = FlattenedMapTree.FirstOrDefault(t => t.Id.ToLower() == UnitySettings.MapName.ToLower());
        if(SelectedMap == null) SelectedMap = FlattenedMapTree.FirstOrDefault();

        minimumSize = new Vector2(50, 400f);
    }

    protected override AdvancedDropdownItem BuildRoot() {
        var root = new AdvancedDropdownItem("Map");

        foreach (var node in MapTree.Children) {
            AddNode(root, node);
        }

        return root;
    }

    public string GetName(MapTreeNode node) {
        if (node.Id == null) {
            return node.DisplayName;
        } else if (node.DisplayName == null || node.DisplayName == node.Id) {
            return node.Id;
        } else {
            return $"{node.Id} - {node.DisplayName}";
        }
    }

    protected AdvancedDropdownItem AddNode(AdvancedDropdownItem parent, MapTreeNode node) {
        AdvancedDropdownItem curDropdownItem = new MapSelectionDropdownItem(GetName(node), node);
        if (node.Children == null) {
            curDropdownItem.id = Array.IndexOf(FlattenedMapTree, node);
        } else {
            curDropdownItem.id = -1;
        }
        parent.AddChild(curDropdownItem);
        if (node.Children != null) {
            foreach (var child in node.Children) AddNode(curDropdownItem, child);
        }

        return parent;
    }

    protected override void ItemSelected(AdvancedDropdownItem item) {
        base.ItemSelected(item);

        if (item.id == -1 || !(item is MapSelectionDropdownItem mapItem) || !mapItem.IsMap)
            return;

        SelectedMap = mapItem.MapTreeNode;
        HasChanged = true;
    }

    public void NextMap() {
        if(FlattenedMapTree == null || FlattenedMapTree.Length == 0) return;
        var index = (Array.IndexOf(FlattenedMapTree, SelectedMap) + 1) % FlattenedMapTree.Length;
        SelectedMap = FlattenedMapTree[index];
        HasChanged = true;
    }

    public void PreviousMap() {
        if (FlattenedMapTree == null || FlattenedMapTree.Length == 0) return;
        var index = (Array.IndexOf(FlattenedMapTree, SelectedMap) - 1 + FlattenedMapTree.Length) % FlattenedMapTree.Length;
        SelectedMap = FlattenedMapTree[index];
        HasChanged = true;
    }

    public MapTreeNode MapTree { get; set; }
    public MapTreeNode[] FlattenedMapTree { get; set; }

    public bool HasChanged { get; set; }

    public MapTreeNode SelectedMap { get; set; }

    public class MapSelectionDropdownItem : AdvancedDropdownItem {
        public MapSelectionDropdownItem(string name, MapTreeNode mapTreeNode) : base(name) {
            MapTreeNode = mapTreeNode;
        }

        public MapTreeNode MapTreeNode { get; set; }
        public bool IsMap => MapTreeNode.Children == null || !MapTreeNode.Children.Any();
    }
}