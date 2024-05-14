using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DWTools.Customization
{
    [CreateAssetMenu(menuName = "[CUSTOMIZATION]Customization/CustomizationInfo", fileName = "new ConcreteCusomizationInfo")]
    public class CustomizationDataSO : ScriptableObject, ICustomizationDataProvider
    {
        [field: SerializeField] public string CustomizableKey { get; private set; }
        public IReadOnlyList<CustomizeItem> CustomizeItems => _customizeItems;

        [SerializeField] private List<CustomizeItem> _customizeItems;
    }

    [Serializable]
    public class CustomizeItem
    {
        public string PointKey;
        public BaseCustomizationActionSO CustomizationAction;
        public List<AssetReference> AssetsGUID;
    }

    [Serializable]
    public enum CustomizationType
    {
        None = 0,
        ObjectReplacement = 10,
        MaterialReplacement = 20,
        TextureReplacement = 30
    }

    public interface ICustomizationDataProvider
    {
        string CustomizableKey { get; }
        IReadOnlyList<CustomizeItem> CustomizeItems { get; }
    }
}