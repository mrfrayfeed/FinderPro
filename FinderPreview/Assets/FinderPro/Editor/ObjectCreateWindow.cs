#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Finder;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

internal class EndNameEdit : EndNameEditAction {
	public override void Action (int instanceId, string pathName, string resourceFile) {
		AssetDatabase.CreateAsset (EditorUtility.InstanceIDToObject (instanceId), AssetDatabase.GenerateUniqueAssetPath (pathName));
		AssetDatabase.SaveAssets ();
	}
}

public class ObjectCreateWindow : EditorWindow {
	private int selectedIndex = 0;
	private string[] names;
	private List<FinderAssetType> types;
	public List<FinderAssetType> Types {
		get { return types; }
		set {
			types = value;
			names = types.Select (t => t.DisplayName).ToArray ();
		}
	}

	public void OnGUI () {
		selectedIndex = EditorGUILayout.Popup ("Type:", selectedIndex, names);
		EditorGUILayout.HelpBox ($"{types[selectedIndex].DisplayName} will be created in current Directory", MessageType.Warning);
		if (GUILayout.Button ("Create")) {
			var asset = ScriptableObject.CreateInstance (types[selectedIndex].Name);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists (
				asset.GetInstanceID (),
				ScriptableObject.CreateInstance<EndNameEdit> (),
				string.Format ($"{names[selectedIndex]}{Types[selectedIndex].Extension}"),
				AssetPreview.GetMiniThumbnail (asset),
				null);

			Close ();
		}
	}
}
#endif