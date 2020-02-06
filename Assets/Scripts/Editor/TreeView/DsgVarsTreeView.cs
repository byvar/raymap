using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using DsgVarType = OpenSpace.AI.DsgVarInfoEntry.DsgVarType;

public class DsgVarsTreeView : TreeViewWithTreeModel<DsgVarsTreeElement>
{
	const float kRowHeights = 20f;
	const float kToggleWidth = 18f;
	public bool showControls = true;

	public DsgVarComponent target;

	private static GUIStyle miniButton;
	GUIStyle MiniButton {
		get {
			if (miniButton == null) {
				miniButton = new GUIStyle(EditorStyles.toolbarButton);
				miniButton.padding = new RectOffset(0, 0, 0, 0);
				miniButton.margin = new RectOffset(0, 0, 0, 0);
				miniButton.alignment = TextAnchor.MiddleLeft;
				miniButton.normal.background = null;
				/*miniButton.normal = new GUIStyleState() {
					background = EditorStyles.miniLabel.normal.background,
					textColor = Color.blue
				};*/
				/*miniButton.active = new GUIStyleState() {
					background = EditorStyles.miniLabel.normal.background,
					textColor = Color.red
				};
				miniButton.onActive = new GUIStyleState() {
					background = EditorStyles.miniLabel.normal.background,
					textColor = Color.green
				};*/
			}
			return miniButton;
		}
	}
	private static GUIStyle centeredLabel;
	GUIStyle CenteredLabel {
		get {
			if (centeredLabel == null) {
				centeredLabel = new GUIStyle(EditorStyles.label);
				centeredLabel.alignment = TextAnchor.MiddleCenter;
			}
			return centeredLabel;
		}
	}



	static Dictionary<DsgVarType, GUIContent> icons = new Dictionary<DsgVarType, GUIContent>() {
		{ DsgVarType.SoundEvent,		EditorGUIUtility.IconContent("AudioSource Icon") },
		{ DsgVarType.SoundEventArray,	EditorGUIUtility.IconContent("AudioSource Icon") },
		{ DsgVarType.WayPoint,			new GUIContent(EditorGUIUtility.FindTexture("WPtex.png")) },
		{ DsgVarType.WayPointArray,     new GUIContent(EditorGUIUtility.FindTexture("WPtex.png")) },
		{ DsgVarType.Text,				EditorGUIUtility.IconContent("Text Icon") },
		{ DsgVarType.TextArray,			EditorGUIUtility.IconContent("Text Icon") },
		{ DsgVarType.TextRefArray,		EditorGUIUtility.IconContent("Text Icon") },
		{ DsgVarType.Action,			EditorGUIUtility.IconContent("Animation Icon") },
		{ DsgVarType.ActionArray,		EditorGUIUtility.IconContent("Animation Icon") },
		{ DsgVarType.Boolean,			EditorGUIUtility.IconContent("Toggle Icon") },
		/*EditorGUIUtility.FindTexture ("Folder Icon"),
		EditorGUIUtility.FindTexture ("AudioSource Icon"),
		EditorGUIUtility.FindTexture ("Camera Icon"),
		EditorGUIUtility.FindTexture ("Windzone Icon"),
		EditorGUIUtility.FindTexture ("GameObject Icon")*/
	};

	// All columns
	enum Columns
	{
		Icon,
		Name,
		//CurrentValueInstance,
		CurrentValue,
		//InitialValueInstance,
		InitialValue
	}

	public static void TreeToList (TreeViewItem root, IList<TreeViewItem> result)
	{
		if (root == null)
			throw new NullReferenceException("root");
		if (result == null)
			throw new NullReferenceException("result");

		result.Clear();
	
		if (root.children == null)
			return;

		Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
		for (int i = root.children.Count - 1; i >= 0; i--)
			stack.Push(root.children[i]);

		while (stack.Count > 0)
		{
			TreeViewItem current = stack.Pop();
			result.Add(current);

			if (current.hasChildren && current.children[0] != null)
			{
				for (int i = current.children.Count - 1; i >= 0; i--)
				{
					stack.Push(current.children[i]);
				}
			}
		}
	}

	public DsgVarsTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<DsgVarsTreeElement> model) : base (state, multicolumnHeader, model)
	{

		// Custom setup
		rowHeight = kRowHeights;
		columnIndexForTreeFoldouts = 0;
		showAlternatingRowBackgrounds = true;
		showBorder = true;
		customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
		extraSpaceBeforeIconAndLabel = kToggleWidth;
		multiColumnHeader.canSort = false;
			
		Reload();
	}


	// Note we We only build the visible rows, only the backend has the full tree information. 
	// The treeview only creates info for the row list.
	protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
	{
		var rows = base.BuildRows (root);
		return rows;
	}

	protected override void SelectionChanged(IList<int> selectedIds) {
		// No selection
		if (selectedIds != null && selectedIds.Count > 0) {
			SetSelection(new List<int>());
		}
	}

	/*int GetIcon1Index(TreeViewItem<StateTransitionTreeElement> item)
	{
		return (int)(Mathf.Min(0.99f, item.data.floatValue1) * s_TestIcons.Length);
	}

	int GetIcon2Index (TreeViewItem<StateTransitionTreeElement> item)
	{
		return Mathf.Min(item.data.text.Length, s_TestIcons.Length-1);
	}*/

	protected override void RowGUI (RowGUIArgs args)
	{
		var item = (TreeViewItem<DsgVarsTreeElement>) args.item;
		if (item.data.arrayIndex.HasValue) {
			EditorGUI.DrawRect(args.rowRect, new Color(0, 0, 0, 0.1f));
		}
		for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
		{
			CellGUI(args.GetCellRect(i), item, (Columns)args.GetColumn(i), ref args);
		}
	}

	void CellGUI (Rect cellRect, TreeViewItem<DsgVarsTreeElement> item, Columns column, ref RowGUIArgs args)
	{
		// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
		CenterRectUsingSingleLineHeight(ref cellRect);
		switch (column)
		{
			case Columns.Icon:
				DsgVarType type = item.data.entry.Type;
				if (item.data.arrayIndex.HasValue) {
					type = DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type);
				}
				if (icons.ContainsKey(type)) {
					GUI.Label(cellRect, icons[type], CenteredLabel);
				}
				break;
			case Columns.Name:
				if (item.data.arrayIndex.HasValue) {
					Rect indentedRect = cellRect;
					float indentValue = Mathf.Min(10f, indentedRect.width);
					indentedRect.x += indentValue;
					indentedRect.width -= indentValue;
					GUI.Label(indentedRect, new GUIContent("[" + item.data.arrayIndex.Value + "]"), EditorStyles.miniLabel);
				} else {
					GUI.Label(cellRect, new GUIContent(item.data.entry.Name), EditorStyles.miniLabel);
				}
				break;
			/*case Columns.CurrentValueInstance:
				item.data.sourceCurrentFromDSGMem = GUI.Toggle(cellRect, item.data.sourceCurrentFromDSGMem, new GUIContent("","Item is sourced from DSGMem"));
				break;*/
			case Columns.CurrentValue:
				DsgVarComponentEditor.DrawDsgVarCurrent(cellRect, item.data.entry, item.data.sourceCurrentFromDSGMem, item.data.arrayIndex);
				break;
			/*case Columns.InitialValueInstance:
				item.data.sourceInitialFromDSGMem = GUI.Toggle(cellRect, item.data.sourceInitialFromDSGMem, new GUIContent("","Item is sourced from DSGMem"));
				break;*/
			case Columns.InitialValue:
				DsgVarComponentEditor.DrawDsgVarInitial(cellRect, item.data.entry, item.data.sourceCurrentFromDSGMem, item.data.arrayIndex);
				break;
		}
	}



	// Misc
	//--------

	protected override bool CanRename(TreeViewItem item) {
		return false;
	}
	protected override void RenameEnded(RenameEndedArgs args) {
		return;
	}
	protected override bool CanStartDrag(CanStartDragArgs args) {
		return false;
	}
	protected override bool CanBeParent(TreeViewItem item) {
		return item.hasChildren;
	}
	protected override bool CanChangeExpandedState(TreeViewItem item) {
		return item.hasChildren;
	}
	protected override bool CanMultiSelect (TreeViewItem item) {
		return false;
	}

	public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
	{
		var columns = new[]
		{
			new MultiColumnHeaderState.Column
			{
				headerContent = new GUIContent("Type", ""),
				headerTextAlignment = TextAlignment.Center,
				canSort = false,
				width = 30,
				minWidth = 30,
				maxWidth = 30,
				autoResize = true,
				allowToggleVisibility = true
			},
			new MultiColumnHeaderState.Column
			{
				headerContent = new GUIContent("Name", ""),
				headerTextAlignment = TextAlignment.Left,
				canSort = false,
				width = 60,
				minWidth = 40,
				autoResize = true,
				allowToggleVisibility = false
			},
			/*new MultiColumnHeaderState.Column
			{
				headerContent = new GUIContent("Mem", "Use DSGMem (Instance memory)"),
				headerTextAlignment = TextAlignment.Center,
				canSort = false,
				width = 30,
				minWidth = 30,
				maxWidth = 30,
				autoResize = true,
				allowToggleVisibility = false
			},*/
			new MultiColumnHeaderState.Column 
			{
				headerContent = new GUIContent("Current Value", ""),
				headerTextAlignment = TextAlignment.Left,
				canSort = false,
				width = 100, 
				minWidth = 60,
				autoResize = true,
				allowToggleVisibility = false
			},
			/*new MultiColumnHeaderState.Column
			{
				headerContent = new GUIContent("Mem", "Use DSGMem (Instance memory"),
				headerTextAlignment = TextAlignment.Center,
				canSort = false,
				width = 30,
				minWidth = 30,
				maxWidth = 30,
				autoResize = true,
				allowToggleVisibility = false
			},*/
			new MultiColumnHeaderState.Column 
			{
				headerContent = new GUIContent("Initial Value", ""),
				headerTextAlignment = TextAlignment.Left,
				canSort = false,
				width = 100,
				minWidth = 60,
				autoResize = true,
				allowToggleVisibility = false
			},
		};

		Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Columns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

		var state =  new MultiColumnHeaderState(columns);
		return state;
	}
}
