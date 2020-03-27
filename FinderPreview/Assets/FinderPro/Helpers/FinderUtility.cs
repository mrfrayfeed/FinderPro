#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Finder {
    public static class FinderUtility {

        [MenuItem ("Finder Pro/Settings")]
        private static void ShowSettings () {
            Selection.activeObject = FinderSettings.GetSettings ();
        }

        [MenuItem ("Finder Pro/Tools/AssetDatabase/Refresh")]
        private static void RefreshDatabase () {
            AssetDatabase.Refresh ();
        }

        [MenuItem ("Finder Pro/Tools/AssetDatabase/SaveAssets")]
        private static void SaveAssetsDatabase () {
            AssetDatabase.SaveAssets ();
        }

        [MenuItem ("CONTEXT/Finder Pro /Add Find Type"),
            MenuItem ("Assets /Finder Pro /Add Find Type")
        ]
        private static void AddFindType () {
            FinderSettings.GetSettings ().AddFindType (Selection.activeObject.GetTrueType ());
        }

        [MenuItem ("CONTEXT/Finder Pro/Add Ingore Type"),
            MenuItem ("Assets/Finder Pro/Add Ingore Type")
        ]
        private static void AddIngoreType () {
            FinderSettings.GetSettings ().AddIngoreType (Selection.activeObject.GetTrueType ());
        }

    }
}
#endif