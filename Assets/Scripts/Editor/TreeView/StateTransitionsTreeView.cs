using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

public class StateTransitionsTreeView : TreeViewWithTreeModel<StateTransitionTreeElement>
{
	const float kRowHeights = 20f;
	const float kToggleWidth = 18f;
	public bool showControls = true;

	public PersoBehaviour perso;
	public ROMPersoBehaviour persoROM;
	public int stateIndex;

	GUIStyle miniButton;
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


	/*static Texture2D[] s_TestIcons =
	{
		EditorGUIUtility.FindTexture ("Folder Icon"),
		EditorGUIUtility.FindTexture ("AudioSource Icon"),
		EditorGUIUtility.FindTexture ("Camera Icon"),
		EditorGUIUtility.FindTexture ("Windzone Icon"),
		EditorGUIUtility.FindTexture ("GameObject Icon")

	};*/

	// All columns
	enum Columns
	{
		TargetState,
		StateToGo,
		LinkingType
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

	public StateTransitionsTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, TreeModel<StateTransitionTreeElement> model) : base (state, multicolumnHeader, model)
	{

		// Custom setup
		rowHeight = kRowHeights;
		columnIndexForTreeFoldouts = 2;
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
		var item = (TreeViewItem<StateTransitionTreeElement>) args.item;

		for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
		{
			CellGUI(args.GetCellRect(i), item, (Columns)args.GetColumn(i), ref args);
		}
	}

	void CellGUI (Rect cellRect, TreeViewItem<StateTransitionTreeElement> item, Columns column, ref RowGUIArgs args)
	{
		// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
		CenterRectUsingSingleLineHeight(ref cellRect);
		switch (column)
		{
			case Columns.TargetState:
				if (GUI.Button(cellRect, item.data.targetStateName, MiniButton)) {
					perso.SetState(item.data.targetStateIndex);
				}
				break;
			case Columns.StateToGo:
				if (GUI.Button(cellRect, item.data.stateToGoName, MiniButton) && perso != null) {
					perso.SetState(item.data.stateToGoIndex);
				}
				break;
			case Columns.LinkingType:
				GUI.Label(cellRect, item.data.linkingType.ToString(), EditorStyles.centeredGreyMiniLabel);
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
		return false;
	}
	protected override bool CanChangeExpandedState(TreeViewItem item) {
		return false;
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
				headerContent = new GUIContent("Target State", "State you want to go to"),
				headerTextAlignment = TextAlignment.Left,
				canSort = false,
				width = 100, 
				minWidth = 60,
				autoResize = true,
				allowToggleVisibility = false
			},
			new MultiColumnHeaderState.Column 
			{
				headerContent = new GUIContent("State To Go", "This state will be used as transition between current and target state"),
				headerTextAlignment = TextAlignment.Left,
				canSort = false,
				width = 100,
				minWidth = 60,
				autoResize = true,
				allowToggleVisibility = false
			},
			new MultiColumnHeaderState.Column 
			{
				headerContent = new GUIContent("Link", "???"),
				headerTextAlignment = TextAlignment.Right,
				canSort = false,
				width = 30,
				minWidth = 30,
				maxWidth = 30,
				autoResize = true,
				allowToggleVisibility = true
			},
		};

		Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Columns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

		var state =  new MultiColumnHeaderState(columns);
		return state;
	}
}
