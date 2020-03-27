#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Finder {
    public static class FinderAssetTypeProvider {
        private static Dictionary<FinderAssetType, Texture2D> types = new Dictionary<FinderAssetType, Texture2D> ();

        public static Dictionary<FinderAssetType, Texture2D> Types {
            get {
                if (types == null || types.Count == 0)
                    GetAllTypes ();
                return types;
            }
            private set => types = value;
        }

        public static FinderAssetType GetFinderAssetType (Type type) {
            var assetType = Types.Keys.FirstOrDefault (x => x.Type == type);
            if (assetType == null) {
                assetType = new FinderAssetType (type);

            }
            return assetType;
        }

        public static System.Type FindType (string typeName, bool useFullName = false, bool ignoreCase = false) {
            if (string.IsNullOrEmpty (typeName)) return null;

            bool isArray = typeName.EndsWith ("[]");
            if (isArray)
                typeName = typeName.Substring (0, typeName.Length - 2);

            StringComparison e = (ignoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (useFullName) {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies ()) {
                    foreach (var t in assemb.GetTypes ()) {
                        if (string.Equals (t.FullName, typeName, e)) {
                            if (isArray)
                                return t.MakeArrayType ();
                            else
                                return t;
                        }
                    }
                }
            } else {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies ()) {
                    foreach (var t in assemb.GetTypes ()) {
                        if (string.Equals (t.Name, typeName, e) || string.Equals (t.FullName, typeName, e)) {
                            if (isArray)
                                return t.MakeArrayType ();
                            else
                                return t;
                        }
                    }
                }
            }
            return null;
        }

        //TODO Можно попробовать начать искать объекты только в папке Assets (включительно)
        public static List<FinderAssetType> GetAllTypes () {
            var settings = FinderSettings.GetSettings ();
            foreach (var type in settings.IngoreTypes) {
                if (!types.Any (t => t.Key.Type == type.Type)) {
                    types.Add (type, type.UseStandartIcon? AssetPreview.GetMiniTypeThumbnail (type.Type) : null);
                }
            }
            foreach (var type in settings.FindTypes) {
                if (!types.Any (t => t.Key.Type == type.Type)) {
                    types.Add (type, type.UseStandartIcon? AssetPreview.GetMiniTypeThumbnail (type.Type) : null);
                }
            }
            var assets = AssetDatabase.GetAllAssetPaths (); //Directory.GetFiles (path, "*.csv", SearchOption.AllDirectories);
            foreach (var asset in assets) {
                var assetObj = FinderAssetUtility.LoadAssetAtPath<Object> (asset);
                if (assetObj != null) {
                    Type type = assetObj.GetType ();
                    if (type == null)
                        continue;
                    var assetType = GetFinderAssetType (type);
                    if (assetType != null) {
                        string assetExtension = Path.GetExtension (Path.Combine (Directory.GetCurrentDirectory (), AssetDatabase.GetAssetPath (assetObj)).Replace ("/", "\\"));
                        if (types.Any (t => t.Key.Type == assetType.Type)) {
                            types[assetType] = types[assetType] ?? AssetPreview.GetMiniThumbnail (assetObj) ?? AssetPreview.GetMiniTypeThumbnail (assetType.Type);
                            assetType.Extension = assetExtension;
                        } else if (assetType.Type.IsSubclassOf (typeof (Object))) {
                            types.Add (assetType, AssetPreview.GetMiniThumbnail (assetObj) ?? AssetPreview.GetMiniTypeThumbnail (assetType.Type));
                            assetType.Extension = assetExtension;
                        }
                    }
                }
            }
            return types.Keys.ToList ();
        }

        public static IEnumerable<FinderAssetType> GetAllTypesAssignableFrom (Type parentType) {
            var types = GetAllTypes ();
            foreach (var type in types) {
                if (type.Type.IsSubclassOf (parentType)) {
                    yield return type;
                }
            }
        }

        public static Texture2D GetTypeIcon (Type type) {
            var assetType = GetFinderAssetType (type);
            return assetType != null? Types[assetType] : AssetPreview.GetMiniTypeThumbnail (type);
        }
        public static Texture2D GetTypeIcon (FinderAssetType type) {
            return Types[type];
        }
        public static Texture2D GetTypeIcon (string type) {
            var result = Types.FirstOrDefault (x => x.Key.Name == type).Value;
            if (result != null) {
                return result;
            }
            Debug.Log ("GetTypeIcon (string type), find icon with FindType (type)");
            return GetTypeIcon (FindType (type));
        }

        public static Texture2D GetObjectIcon (Object obj) {
            return AssetPreview.GetAssetPreview (obj) ?? AssetPreview.GetMiniThumbnail (obj);
        }

        public static List<Object> FindAssetsByType (Type type) {
            List<Object> assets = new List<Object> ();
            string[] guids = AssetDatabase.FindAssets (string.Format ("t:{0}", type.Name), new string[] { "Assets" });
            for (int i = 0; i < guids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath (guids[i]);
                Object asset = AssetDatabase.LoadAssetAtPath<Object> (assetPath);
                // Debug.Log (asset.ToString ());
                if (asset != null) {
                    assets.Add (asset);
                }
            }
            return assets;
        }
    }
}
#endif