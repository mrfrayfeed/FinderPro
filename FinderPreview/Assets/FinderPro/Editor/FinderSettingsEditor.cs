#if UNITY_EDITOR
using Finder;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor (typeof (FinderSettings))]
public class FinderSettingsEditor : Editor {
    private SerializedProperty _ignoreProperty;
    private ReorderableList _ignorelist;
    private SerializedProperty _findProperty;
    private ReorderableList _findlist;
    FinderSettings settings;
    private void OnEnable () {
        settings = (FinderSettings) target;
        _ignoreProperty = serializedObject.FindProperty ("ingoreTypes");
        _ignorelist = new ReorderableList (serializedObject, _ignoreProperty, true, true, false, true) {
            drawHeaderCallback = (rect) => { GUI.Label (rect, "IgnoreTypes"); },
                onRemoveCallback = (ReorderableList list) => { settings.IngoreTypes.RemoveAt (list.index); },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    var type = settings.IngoreTypes[index];
                    EditorGUI.LabelField (rect, new GUIContent () { text = type.DisplayName, image = FinderAssetTypeProvider.GetTypeIcon (type.Type) });
                }
        };
        _findProperty = serializedObject.FindProperty ("findTypes");
        _findlist = new ReorderableList (serializedObject, _findProperty, true, true, false, true) {
            drawHeaderCallback = (rect) => { GUI.Label (rect, "FindTypes"); },
                onRemoveCallback = (ReorderableList list) => { settings.FindTypes.RemoveAt (list.index); },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    var type = settings.FindTypes[index];
                    EditorGUI.LabelField (rect,
                        new GUIContent () { text = type.DisplayName, image = FinderAssetTypeProvider.GetTypeIcon (type.Type) }); //*item.IsNameOnly? null : FinderAssetUtility.GetTypeIcon (item.Type)*/
                }
        };
    }

    public override void OnInspectorGUI () {
        // serializedObject.Update ();
        EditorGUILayout.Space ();
        _ignorelist.DoLayoutList ();
        _findlist.DoLayoutList ();
        if (GUI.changed)
            serializedObject.ApplyModifiedProperties ();
    }
}
#endif