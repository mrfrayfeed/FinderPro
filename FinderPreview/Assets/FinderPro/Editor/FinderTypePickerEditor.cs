#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Finder;
using UnityEditor;
using UnityEngine;

public class FinderTypePickerEditor : EditorWindow {
    public Action<FinderAssetType> OnTypeSelected;

    private EditorGridUtility gridDrawer = new EditorGridUtility ();

    private string searchString = "";
    private Type searchType;
    private bool showIgnoredTypes = false;

    private GUIStyle gridItemStyle;
    private GUIStyle toolbarButtonImageOnlyStyle;
    private GUIContent gridItemContent;

    private FinderSettings finderSettings;
    private List<FinderAssetType> foundedTypes;
    private List<FinderAssetType> displayTypes;

    public static FinderTypePickerEditor ShowAsPopup () {
        var window = GetWindow<FinderTypePickerEditor> ();
        window.titleContent = new GUIContent ("Type Picker");
        window.minSize = new Vector2 (350, 150);
        window.ShowPopup ();
        return window;
    }

    private void OnEnable () {
        finderSettings = FinderSettings.GetSettings ();
        Init ();
        EditorApplication.projectChanged += Refresh;
        FinderSettings.OnSettingsUpdated.AddListener (Refresh);
    }
    private void OnDisable () {
        EditorApplication.projectChanged -= Refresh;
        FinderSettings.OnSettingsUpdated.RemoveListener (Refresh);
    }

    private void Init () {
        gridDrawer = new EditorGridUtility ();
        GUISkin guiSkin = Resources.Load<GUISkin> ("FinderProGUISkin");
        gridItemStyle = guiSkin.GetStyle ("FinderProGridItem");
        toolbarButtonImageOnlyStyle = new GUIStyle (GUI.skin.FindStyle ("ToolbarButton"));
        toolbarButtonImageOnlyStyle.imagePosition = ImagePosition.ImageOnly;
        gridItemContent = new GUIContent ();
        SetFindType (null);
    }

    private void SetFindType (Type type) {
        searchType = type;
        if (type != null) {
            searchString = type.Name;
            foundedTypes = FinderAssetTypeProvider.GetAllTypesAssignableFrom (searchType).ToList ();
        } else {
            searchString = "";
            foundedTypes = FinderAssetTypeProvider.GetAllTypes ();
        }

        foundedTypes = finderSettings.GetTypesByIngoreTypes (foundedTypes);
        foundedTypes = finderSettings.GetTypesWithBaseTypes (foundedTypes);
        foundedTypes = foundedTypes.Where (x => x != null).ToList ();
        foundedTypes.Sort ((a, b) => {
            return a.Type.BaseType.Name.CompareTo (b.Type.BaseType.Name);
        });
        gridDrawer.pagesCount = (int) (foundedTypes.Count () / gridDrawer.buttonsOnPage - 0.1f);
        gridDrawer.currentPage = 0;
    }

    private void Refresh () {
        SetFindType (searchType);
    }

    private void Toolbar () {
        GUILayout.BeginHorizontal (GUI.skin.FindStyle ("Toolbar"));
        GUILayout.Space (5f);

        if (GUILayout.Button (EditorGUIUtility.IconContent ("d_Refresh"), GUI.skin.FindStyle ("ToolbarButton"), GUILayout.Width (25f))) {
            Refresh ();
        }
        GUILayout.Space (5f);
        searchString = GUILayout.TextField (searchString, GUI.skin.FindStyle ("ToolbarSeachTextField"));
        if (GUILayout.Button ("", GUI.skin.FindStyle ("ToolbarSeachCancelButton"))) {
            searchString = "";
            GUI.FocusControl (null);
        }
        GUILayout.Space (50f);

        var showHiddenTypeButtonContent = new GUIContent (EditorGUIUtility.IconContent ("animationvisibilitytoggleon"));
        showHiddenTypeButtonContent.tooltip = "Show hidden types from Settings";
        showIgnoredTypes = GUILayout.Toggle (showIgnoredTypes, showHiddenTypeButtonContent, toolbarButtonImageOnlyStyle, GUILayout.Width (25f));
        var settingsButtonContent = new GUIContent (EditorGUIUtility.IconContent ("ClothInspector.SettingsTool"));

        settingsButtonContent.tooltip = "Open FinderSettings";
        if (GUILayout.Button (settingsButtonContent, toolbarButtonImageOnlyStyle, GUILayout.Width (25f))) {
            Selection.activeObject = FinderSettings.GetSettings ();
        }
        GUILayout.Space (5f);
        GUILayout.EndHorizontal ();
    }

    private void Grid () {
        try {
            gridDrawer.Display (displayTypes.ToArray (), GridElement, true, !showIgnoredTypes);
            if (showIgnoredTypes) {
                EditorGUILayout.Space ();
                EditorGUILayout.Space ();
                GUILayout.BeginHorizontal ();
                GUIContent hiddenLabelContent = new GUIContent (EditorGUIUtility.IconContent ("animationvisibilitytoggleon"));
                hiddenLabelContent.text = "Hidden types:";
                EditorGUILayout.LabelField (hiddenLabelContent, GUI.skin.FindStyle ("ToolbarButton"));
                GUILayout.EndHorizontal ();
                EditorGUILayout.Space ();
                EditorGUILayout.Space ();
                if (string.IsNullOrEmpty (searchString)) {
                    gridDrawer.Display (finderSettings
                        .IngoreTypes
                        .ToArray (), GridHiddenElement, false, true);
                } else {
                    gridDrawer.Display (finderSettings
                        .IngoreTypes
                        .Where (x => x.Name.ToLower ().Contains (searchString.ToLower ()))
                        .ToArray (), GridHiddenElement, false, true);
                }
                //finderSettings.IngoreTypes.Where (x => x.Name.ToLower ().Contains (searchString.ToLower ());)
            }
        } catch (System.Exception ex) {
            Debug.Log (ex);
        }
    }

    private void GridElement (FinderAssetType type) {
        gridItemContent.text = type.DisplayName;
        gridItemContent.image = FinderAssetTypeProvider.GetTypeIcon (type.Type);
        GUILayout.Space (gridDrawer.margin);
        if (GUILayout.Button (gridItemContent, gridItemStyle, GUILayout.Width (gridDrawer.buttonSize), GUILayout.Height (gridDrawer.buttonSize))) {
            OnTypeSelected?.Invoke (type);
            this.Close ();
        }
    }
    private void GridHiddenElement (FinderAssetType type) {
        gridItemContent.text = type.DisplayName;
        gridItemContent.image = FinderAssetTypeProvider.GetTypeIcon (type.Type) ?? EditorGUIUtility.IconContent ("DefaultAsset Icon").image;
        GUILayout.Space (gridDrawer.margin);
        if (GUILayout.Button (gridItemContent, gridItemStyle, GUILayout.Width (gridDrawer.buttonSize), GUILayout.Height (gridDrawer.buttonSize))) {
            OnTypeSelected?.Invoke (type);
            this.Close ();
        }
    }

    private void OnGUI () {
        gridDrawer.position = position;
        if (foundedTypes == null)
            SetFindType (null);

        Toolbar ();

        if (!string.IsNullOrEmpty (searchString)) {
            displayTypes = foundedTypes
                .Where (x => x.Name.ToLower ().Contains (searchString.ToLower ()))
                .ToList ();
        } else {
            displayTypes = foundedTypes;
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        Grid ();
    }

}
#endif