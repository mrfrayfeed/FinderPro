#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Finder {

    // [CreateAssetMenu (fileName = "FinderSettings", menuName = "Finder Pro/FinderSettings")]
    public class FinderSettings : ScriptableObject {
        public static UnityEngine.Events.UnityEvent OnSettingsUpdated = new UnityEngine.Events.UnityEvent ();
        [SerializeField] private List<FinderAssetType> ingoreTypes = new List<FinderAssetType> () {
            new FinderAssetType (typeof (FinderSettings)),
            new FinderAssetType (typeof (UnityEngine.Object)),
            new FinderAssetType (typeof (UnityEngine.QualitySettings)) { IsIgnored = true },
            new FinderAssetType (typeof (UnityEngine.Experimental.VFX.VFXManager)) { UseStandartIcon = true, IsCreatable = false },
            new FinderAssetType (typeof (UnityEngine.UIElements.StyleSheet)),
            new FinderAssetType (typeof (UnityEngine.UIElements.VisualTreeAsset)) { IsCreatable = false },
            new FinderAssetType ("MonoScript") { IsCreatable = false },
            new FinderAssetType ("DefaultAsset") { },
            new FinderAssetType ("PresetManager") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("PlayerSettings") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("EditorBuildSettings") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("EditorSettings") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("PhysicsManager") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("AudioManager") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("InputManager") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("GraphicsSettings") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("TagManager") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("TimeManager") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("Physics2DSettings") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("Physics2DSettings") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("AssemblyDefinitionAsset") { IsIgnored = true, IsCreatable = false },
            new FinderAssetType ("UnityConnectSettings") { UseStandartIcon = true, IsIgnored = true, IsCreatable = false },

        };
        [SerializeField] private List<FinderAssetType> findTypes = new List<FinderAssetType> () {
            new FinderAssetType (typeof (UnityEngine.ScriptableObject)) { UseStandartIcon = true },
            new FinderAssetType (typeof (UnityEngine.Texture)) { UseStandartIcon = true, IsCreatable = false },
            new FinderAssetType (typeof (UnityEngine.Texture2D)) { UseStandartIcon = true, IsCreatable = false },
            new FinderAssetType (typeof (UnityEngine.GameObject)) { UseStandartIcon = true },
            new FinderAssetType ("SceneAsset", "Scene") { UseStandartIcon = true, }
        };

        public List<FinderAssetType> IngoreTypes { get => ingoreTypes; }
        public List<string> IngoreTypesString { get => ingoreTypes.Select (x => x.Name).ToList (); }
        public List<FinderAssetType> FindTypes { get => findTypes; }

        private void OnEnable () {
            foreach (var type in ingoreTypes) {
                if (string.IsNullOrEmpty (type.FullName)) {
                    type.Type = FinderAssetTypeProvider.FindType (type.Name);
                }
            }
            foreach (var type in findTypes) {
                if (string.IsNullOrEmpty (type.FullName)) {
                    type.Type = FinderAssetTypeProvider.FindType (type.Name);
                }
            }
            Save ();

        }

        public static FinderSettings GetSettings () {
            FinderSettings settings = (FinderSettings) Resources.Load ("FinderSettings");
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<FinderSettings> ();
                settings.name = "FinderSettings";
                settings.Save ();
            }
            return settings;
        }

        public List<FinderAssetType> GetTypesByIngoreTypes (List<FinderAssetType> types) {
            List<FinderAssetType> result = new List<FinderAssetType> ();
            foreach (var type in types) {
                if (type != null && !IngoreTypes.Exists (x => x.Type == type.Type))
                    result.Add (type);
            }
            return result;
        }

        public List<FinderAssetType> GetTypesWithBaseTypes (List<FinderAssetType> types) {
            List<FinderAssetType> result = types;
            foreach (var type in FindTypes) {
                if (type != null && !result.Exists (x => x.Type == type.Type))
                    result.Add (type);
            }
            return result;
        }

        public void AddIngoreType (Type type) {
            if (!IngoreTypes.Exists (x => x.Name == type.Name) && type.IsSubclassOf (typeof (UnityEngine.Object))) {
                RemoveFindType (type);
                IngoreTypes.Add (new FinderAssetType (type));
                Save ();
            }
        }

        public void RemoveIngoreType (Type type) {
            if (IngoreTypes.Exists (x => x.Name == type.Name)) {
                IngoreTypes.RemoveAt (IngoreTypes.FindIndex (0, IngoreTypes.Count, x => x.Name == type.Name));
            }
        }

        public void AddFindType (Type type) {
            if (!FindTypes.Exists (x => x.Name == type.Name) && type.IsSubclassOf (typeof (UnityEngine.Object))) {
                RemoveIngoreType (type);
                FindTypes.Add (new FinderAssetType (type));
                Save ();
            }
        }

        public void RemoveFindType (Type type) {
            if (FindTypes.Exists (x => x.Name == type.Name)) {
                FindTypes.RemoveAt (FindTypes.FindIndex (0, FindTypes.Count, x => x.Name == type.Name));
            }
        }

        private void Save () {
            try {
                OnSettingsUpdated?.Invoke ();
                FinderAssetUtility.SaveAsset ("Assets/FinderPro/Resources/", this, "FinderSettings");
            } catch (System.Exception) { }
        }
    }
}
#endif