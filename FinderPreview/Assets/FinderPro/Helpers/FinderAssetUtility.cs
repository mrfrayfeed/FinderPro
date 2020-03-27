#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Finder {
    public static class FinderAssetUtility {
        public static void SaveAsset<T> (string path, T asset, string fileName, string extencion = "asset") where T : Object {
            if (!AssetDatabase.Contains (asset)) {
                CreatePath (path);
                AssetDatabase.CreateAsset (asset, AssetDatabase.GenerateUniqueAssetPath (path + $"{fileName}.{extencion}"));
                Debug.Log ($"SaveAsset: \"{fileName}\" was saved on path: {path}{fileName}.{extencion}");
            } else {
                EditorUtility.SetDirty (asset);
                Debug.Log ($"SaveAsset: \"{fileName}\" was saved on default path");
            }
            AssetDatabase.SaveAssets ();
        }
        public static void CreatePath (string path) {
            if (!Directory.Exists (path)) {
                Directory.CreateDirectory (path);
                AssetDatabase.Refresh ();
            }
        }

        public static bool DeleteAsset (Object asset) {
            return DeleteAsset (AssetDatabase.GetAssetPath (asset));

        }

        public static bool DeleteAsset (string path) {
            Debug.Log ($"DeleteAsset: asset on path \"{path}\" was deleted");
            return AssetDatabase.DeleteAsset (path);
        }

        public static string GetAssetFloderPath (Object asset) {
            string path = AssetDatabase.GetAssetPath (asset);
            path = path.Substring (0, path.LastIndexOf ('/') + 1);

            return path;
        }

        public static Object LoadAssetAtPath<T> (string path) where T : Object {
            return AssetDatabase.LoadAssetAtPath (path, typeof (T));
        }

        public static string OpenFolderPanel (string title, string floder, string defaultName) {
            string path = EditorUtility.OpenFolderPanel (title, floder, defaultName) + "/";
            if (path != "")
                path = path.Substring (path.IndexOf ("Asset"));
            return path;
        }
    }
}
#endif