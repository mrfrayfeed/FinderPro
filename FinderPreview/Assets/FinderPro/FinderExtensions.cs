#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Finder {
    public static class FinderExtensions {
        public static Type GetTrueType (this UnityEngine.Object obj) {
            if (obj is MonoScript) {
                return (obj as MonoScript).GetClass ();
            }

            if (obj is ScriptableObject) {
                return (obj as ScriptableObject).GetType ();
            }

            return obj.GetType ();
        }
    }
}
#endif