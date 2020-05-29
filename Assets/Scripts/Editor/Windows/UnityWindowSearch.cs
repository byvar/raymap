using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Object;
using UnityEditor;
using UnityEngine;

using Path = System.IO.Path;

public class UnityWindowSearch : UnityWindow {

    public string SearchString = "";
    public List<SearchableString> Results = null;
    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Raymap/Search variables and scripts...")]
	public static void ShowWindow() {
		GetWindow<UnityWindowSearch>(false, "Search", true);
	}
	private void OnEnable() {
		titleContent = EditorGUIUtility.IconContent("Search");
        titleContent.text = "Search";
    }

    public void OnGUI() {

        if (MapLoader.Loader == null) {
            Results = null;
        }

        string newSearchString = GUILayout.TextField(SearchString, EditorStyles.toolbarSearchField);
        if (newSearchString != SearchString || Results == null) {
            SearchString = newSearchString;

            Results = Search(newSearchString);
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        Results.ForEach(s =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(s.String, GUILayout.MaxWidth(300));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(s.LocationString)) {
                EditorGUIUtility.PingObject(s.RelatedGameObject);
            }
            GUILayout.EndHorizontal();
        });
        GUILayout.EndScrollView();
    }

    private List<SearchableString> Search(string query)
    {
        var results = new List<SearchableString>();

        return MapLoader.Loader?.searchableStrings.Where(s => s.String.ToLower().Contains(query.ToLower())).ToList() ?? new List<SearchableString>();

    }
}