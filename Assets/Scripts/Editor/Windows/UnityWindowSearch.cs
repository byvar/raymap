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


    private float totalyPos = 0f;
    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Raymap/Search variables and scripts")]
	public static void ShowWindow() {
		GetWindow<UnityWindowSearch>(false, "Text Search", true);
	}
	private void OnEnable() {
		titleContent = EditorGUIUtility.IconContent("Text Icon");
        titleContent.text = "Text Search";
    }

    public void OnGUI() {

        if (!EditorApplication.isPlaying || Settings.s == null) {
            Results = null;
        }
        if (EditorApplication.isPlaying && Settings.s != null) {
            float yPos = 0f;
            Rect rect = GetNextRect(ref yPos);
            string newSearchString = EditorGUI.TextField(rect, SearchString, EditorStyles.toolbarSearchField);
            if (newSearchString != SearchString || Results == null) {
                SearchString = newSearchString;

                Results = Search(newSearchString);
            }

            float height = position.height - yPos;
            if (totalyPos == 0f) totalyPos = height;
            scrollbarShown = totalyPos > height;
            scrollPosition = GUI.BeginScrollView(new Rect(0, yPos, EditorGUIUtility.currentViewWidth, height), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - (scrollbarShown ? scrollbarWidth : 0f), totalyPos));

            yPos = 0f;
            Results.ForEach(s => {
                rect = GetNextRect(ref yPos);
                GUI.Label(rect, s.String);
                string locStr = s.LocationString;
                float w;
                EditorStyles.label.CalcMinMaxWidth(new GUIContent(locStr), out w, out _);
                w = Mathf.Min(w + 10, rect.width);
                Rect r = new Rect(rect.x + rect.width - w, rect.y, w, rect.height);
                if (GUI.Button(r, locStr)) {
                    EditorGUIUtility.PingObject(s.RelatedGameObject);
                }
            });
            totalyPos = yPos;
            GUI.EndScrollView();
        } else {
            EditorGUILayout.HelpBox("Please start the scene to use this window.", MessageType.Info);
        }
    }

    private List<SearchableString> Search(string query)
    {
        if (!EditorApplication.isPlaying || Settings.s == null) return new List<SearchableString>();
        var results = new List<SearchableString>();
        return MapLoader.Loader?.searchableStrings.Where(s => s.String.ToLower().Contains(query.ToLower())).ToList() ?? new List<SearchableString>();

    }
}