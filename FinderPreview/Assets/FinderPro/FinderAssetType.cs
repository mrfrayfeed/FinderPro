#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Finder {

    [Serializable]
    public class FinderAssetType {
        [SerializeField] private string displayName;
        [SerializeField] private string fullName;
        [SerializeField] private string extension;
        [SerializeField] private bool useStandartIcon = false;
        [SerializeField] private bool isIgnored = false;
        [SerializeField] private bool isCreatable = true;
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty (displayName)) {
                    return Name;
                }
                return displayName;
            }
            set => displayName = value;
        }
        public string Name {
            get => Type.Name;
            set {
                Type type = FinderAssetTypeProvider.FindType (value);
                if (type == null) {
                    Debug.LogError ($"Type: {value} was not found!");
                    return;
                }
                Type = type;
                if (string.IsNullOrEmpty (displayName)) {
                    displayName = Type.Name;
                }
            }
        }
        public string FullName { get => fullName; private set => fullName = value; }
        public string Extension { get => extension; set => extension = value; }
        public bool UseStandartIcon { get => useStandartIcon; set => useStandartIcon = value; }
        public bool IsIgnored { get => isIgnored; set => isIgnored = value; }
        public bool IsCreatable { get => isCreatable; set => isCreatable = value; }
        public Type Type {
            get {
                return Type.GetType (fullName);
            }
            set {
                displayName = value.Name;
                fullName = value.AssemblyQualifiedName;
            }
        }

        public FinderAssetType () {
            Type = typeof (GameObject);
        }

        public FinderAssetType (string typeName) {
            this.Name = typeName;
        }

        public FinderAssetType (string typeName, string displayName) {
            this.Name = typeName;
            this.displayName = displayName;
        }

        public FinderAssetType (Type type) {
            this.Type = type;
        }

        public FinderAssetType (Type type, string displayName) {
            this.Type = type;
            this.displayName = displayName;

        }

        public bool Equals (Type type) {
            if (type == null) {
                return false;
            }

            return FullName == type.FullName;
        }

    }
}
#endif