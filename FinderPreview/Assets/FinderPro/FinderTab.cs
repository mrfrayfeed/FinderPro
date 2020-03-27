#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Finder {

    [Serializable]
    public class FinderTab {
        [SerializeField] private FinderAssetType selectedType = new FinderAssetType (typeof (GameObject));
        [SerializeField] public string searchString;
        [SerializeField] public int currentPage = 0;

        [SerializeField] public UnityEngine.Object[] foundedObjects;

        public Type SelectedType { get => selectedType.Type; set => selectedType.Type = value; }
        public FinderAssetType SelectedAssetType { get => selectedType; set => selectedType = value; }
    }
}
#endif