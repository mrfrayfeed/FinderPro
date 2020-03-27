#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Finder;

public class FinderEditor : EditorWindow {
    private EditorGridUtility gridDrawer = new EditorGridUtility ();

    private List<Object> foundedObjects;
    private List<Object> displayObjects;

    private GUISkin guiSkin;
    private GUIStyle gridItemStyle;
    private GUIContent plusButtonContent;
    private GUIContent gridItemGUIContent;
    private GUIContent selectedTypeGUIContent;
    private GUIContent selectedObjectGUIContent;

    private Vector2 tabScrollPos;

    private Object selectedObject;

    [SerializeField] private int selectedTabIndex = 0;
    private List<FinderTab> tabs = new List<FinderTab> ();

    public string SearchString { get => CurrentTab.searchString; set => CurrentTab.searchString = value; }
    public Type SelectedType { get => CurrentTab.SelectedType; set => CurrentTab.SelectedType = value; }
    public FinderAssetType SelectedAssetType { get => CurrentTab.SelectedAssetType; set => CurrentTab.SelectedAssetType = value; }
    public FinderTab CurrentTab {
        get => tabs.ElementAt (selectedTabIndex);
        set {
            int index = tabs.IndexOf (value);
            selectedTabIndex = index == -1 ? 0 : index;
        }
    }

    [MenuItem ("Window/Finder Pro")]
    private static void ShowWindow () {
        var window = GetWindow<FinderEditor> ();
        window.titleContent = new GUIContent ("Finder Pro", EditorGUIUtility.IconContent ("d_UnityEditor.LookDevView").image);
        window.Show ();
    }

    private void OnEnable () {
        if (tabs.Count == 0) {
            AddTab ();
        } else {
            SetTab (tabs[selectedTabIndex]);
        }
        Init ();
        EditorApplication.projectChanged += Refresh;
    }

    private void OnDisable () {
        EditorApplication.projectChanged -= Refresh;
    }

    private void Init () {
        gridDrawer = new EditorGridUtility ();
        GUISkin guiSkin = Resources.Load<GUISkin> ("FinderProGUISkin");
        gridItemStyle = guiSkin.GetStyle ("FinderProGridItem");
        plusButtonContent = new GUIContent () { image = (Texture) Resources.Load ("Textures/ToolbarPlus.png"), text = "+" };
        gridItemGUIContent = new GUIContent ();
        selectedTypeGUIContent = new GUIContent ();
        selectedObjectGUIContent = new GUIContent ();
    }

    private void SelectObject (Object obj) {
        Selection.activeObject = obj;
        selectedObject = obj;
    }

    private void SetType (FinderAssetType type) {
        SelectedAssetType = type;
        SearchString = string.Empty;
        selectedObject = null;
        foundedObjects = FinderAssetTypeProvider.FindAssetsByType (type.Type).ToList ();
        foundedObjects.Sort ((a, b) => {
            return a.name.CompareTo (b.name);
        });
        displayObjects = foundedObjects;
        gridDrawer.pagesCount = (int) (foundedObjects.Count / gridDrawer.buttonsOnPage);
    }

    private void Refresh () {
        SetType (SelectedAssetType);
    }

    private FinderTab AddTab () {
        FinderTab currentTab = new FinderTab ();
        tabs.Add (currentTab);
        SetTab (currentTab);
        return currentTab;
    }

    private void RemoveTab () {
        tabs.Remove (CurrentTab);
        if (tabs.Count > 0) {
            SetTab (tabs[tabs.Count - 1]);
        } else {
            AddTab ();
        }
    }

    private void SetTab (FinderTab tab) {
        CurrentTab = tab;
        SetType (CurrentTab.SelectedAssetType);
    }

    private void Tabs () {
        tabScrollPos = GUILayout.BeginScrollView (tabScrollPos, GUI.skin.FindStyle ("Toolbar"), GUILayout.ExpandWidth (true));
        GUILayout.BeginHorizontal ();
        GUILayout.Space (5f);

        foreach (var tab in tabs.Take (tabs.Count)) {
            var tabContent = EditorGUIUtility.ObjectContent (null, tab.SelectedType);
            tabContent.text = tab.SelectedAssetType.DisplayName;
            tabContent.image = FinderAssetTypeProvider.GetTypeIcon (tab.SelectedType);
            if (GUILayout.Button (tabContent,
                    CurrentTab == tab ? GUI.skin.FindStyle ("ToolbarButton") : GUI.skin.FindStyle ("ToolbarButton"),
                    CurrentTab == tab ? GUILayout.Width (100f) : GUILayout.Width (125f))) {
                SetTab (tab);
            }
            if (CurrentTab == tab) {
                if (GUILayout.Button (EditorGUIUtility.IconContent ("LookDevClose"), GUI.skin.FindStyle ("ToolbarButton"), GUILayout.Width (25f))) {
                    RemoveTab ();
                }
            }
        }
        GUILayout.Space (5f);
        if (GUILayout.Button (plusButtonContent, GUI.skin.FindStyle ("ToolbarButton"), GUILayout.Width (25f))) {
            AddTab ();
        }
        GUILayout.Space (5f);
        GUILayout.EndHorizontal ();
        GUILayout.EndScrollView ();
    }

    private void PreviewSelected () {
        if (selectedObject != null) {
            selectedObjectGUIContent = EditorGUIUtility.ObjectContent (null, SelectedType);
            selectedObjectGUIContent.image = AssetPreview.GetMiniThumbnail (selectedObject);
            selectedObjectGUIContent.text = selectedObject.name;
        } else {
            selectedTypeGUIContent = EditorGUIUtility.ObjectContent (null, SelectedType);
            selectedTypeGUIContent.image = FinderAssetTypeProvider.GetTypeIcon (SelectedType);
            selectedTypeGUIContent.text = SelectedAssetType.DisplayName;
        }
        EditorGUILayout.BeginHorizontal ();
        GUILayout.Space (5f);
        EditorGUILayout.LabelField (selectedObject != null?selectedObjectGUIContent : selectedTypeGUIContent,
            GUI.skin.FindStyle ("WordWrappedLabel"),
            GUILayout.Width (250f),
            position.height < 300 ? GUILayout.Height (25f) : GUILayout.Height (50f));
        EditorGUILayout.EndHorizontal ();
    }

    private void Toolbar () {
        GUILayout.BeginHorizontal (GUI.skin.FindStyle ("Toolbar"), GUILayout.ExpandWidth (true));

        GUILayout.Space (5f);

        GUILayout.BeginHorizontal ();
        selectedTypeGUIContent = new GUIContent (EditorGUIUtility.ObjectContent (null, SelectedType));
        selectedTypeGUIContent.image = FinderAssetTypeProvider.GetTypeIcon (SelectedType);
        selectedTypeGUIContent.tooltip = "Click to change type";
        if (position.width > 450) {
            selectedTypeGUIContent.text = SelectedAssetType.DisplayName;
        } else {
            selectedTypeGUIContent.text = "";
        }
        if (GUILayout.Button (selectedTypeGUIContent, GUI.skin.FindStyle ("toolbarPopup"), position.width > 450 ? GUILayout.Width (175f) : GUILayout.Width (35f))) {
            var popup = FinderTypePickerEditor.ShowAsPopup ();
            popup.OnTypeSelected += (type) => {
                SetType (type);
            };
        }
        GUILayout.Space (5f);
        if (GUILayout.Button (EditorGUIUtility.IconContent ("d_Refresh"), GUI.skin.FindStyle ("ToolbarButton"), GUILayout.Width (25f))) {
            Refresh ();
        }
        if (SelectedAssetType.Type.IsSubclassOf (typeof (ScriptableObject)) || SelectedType == typeof (ScriptableObject)) {
            var createButtonContent = new GUIContent (EditorGUIUtility.IconContent ("BillboardAsset Icon"));
            createButtonContent.tooltip = "Create instance of selected type";
            createButtonContent.text = "Create";
            if (GUILayout.Button (createButtonContent, GUI.skin.FindStyle ("ToolbarButton"), GUILayout.Width (75f))) {
                CreateObject (SelectedType);
            }
        }
        GUILayout.EndHorizontal ();

        GUILayout.BeginHorizontal ();
        SearchString = GUILayout.TextField (SearchString, GUI.skin.FindStyle ("ToolbarSeachTextField"), GUILayout.ExpandWidth (true));
        if (GUILayout.Button ("", GUI.skin.FindStyle ("ToolbarSeachCancelButton"))) {
            SearchString = "";
            GUI.FocusControl (null);
        }
        GUILayout.EndHorizontal ();

        GUILayout.Space (15f);

        GUILayout.EndHorizontal ();
    }

    private void Grid () {
        try {
            gridDrawer.Display (displayObjects.ToArray (), GridElement);
        } catch (System.Exception ex) {
            Debug.Log (ex);
        }
    }

    private void GridElement (Object obj) {
        gridItemGUIContent.text = (obj as Object).name;
        gridItemGUIContent.image = FinderAssetTypeProvider.GetObjectIcon (obj); //AssetPreview.GetAssetPreview (item) ?? 
        GUILayout.Space (gridDrawer.margin);
        if (GUILayout.Button (gridItemGUIContent, gridItemStyle, GUILayout.Width (gridDrawer.buttonSize), GUILayout.Height (gridDrawer.buttonSize))) {
            SelectObject (obj);
        }
    }

    private void SelectedObjectPath () {
        if (selectedObject != null) {
            GUILayout.FlexibleSpace ();
            var pathContent = EditorGUIUtility.ObjectContent (selectedObject, typeof (Object));
            pathContent.text = $"{FinderAssetUtility.GetAssetFloderPath (selectedObject)}{selectedObject.name}{SelectedAssetType.Extension}";
            pathContent.image = FinderAssetTypeProvider.GetTypeIcon (selectedObject.GetType ());
            EditorGUILayout.LabelField (pathContent, GUI.skin.FindStyle ("Toolbar"), GUILayout.Height (20f));
        }
    }

    private void OnGUI () {
        gridDrawer.position = position;

        Tabs ();

        EditorGUILayout.Separator ();

        PreviewSelected ();

        Toolbar ();

        if (foundedObjects == null || foundedObjects.Count == 0) {
            EditorGUILayout.LabelField ($"Assets with type {SelectedType} not found");
            return;
        }

        if (!string.IsNullOrEmpty (SearchString)) {
            displayObjects = foundedObjects
                .Where (x => x.name.ToLower ().Contains (SearchString.ToLower ()))
                .ToList ();
        } else {
            displayObjects = foundedObjects;
        }

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        Grid ();

        SelectedObjectPath ();
    }

    // [MenuItem ("Window/Finder Pro/Tools/ScriptableObject/Create")]
    [MenuItem ("Finder Pro/Create/ScriptableObject", false, 0)]
    [MenuItem ("Assets/Finder Pro/Create/ScriptableObject", false, 0)]
    public static void CreateObject () {
        CreateObject (null);
    }
    public static void CreateObject (Type type = null) {
        Type selectedType = type?? typeof (ScriptableObject);
        if (selectedType == null ||
            (!selectedType.IsSubclassOf (typeof (ScriptableObject)) &&
                selectedType != typeof (ScriptableObject))) {
            selectedType = typeof (ScriptableObject);
        }

        var objectsOfType = FinderSettings.GetSettings ().GetTypesByIngoreTypes (FinderAssetTypeProvider.GetAllTypesAssignableFrom (selectedType).ToList ());
        if (selectedType != typeof (ScriptableObject)) {
            objectsOfType.Add (FinderAssetTypeProvider.GetFinderAssetType (selectedType));
        }
        var window = EditorWindow.GetWindow<ObjectCreateWindow> (true, $"Create a new {selectedType.Name}", true);
        window.maxSize = new Vector2 (500f, 500f);
        window.Types = objectsOfType;
        window.ShowPopup ();
    }
}
#endif