using StickmanSniper.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Weapon
{

    [CreateAssetMenu(menuName = "[GameWeaponConfig]/ShopPresentation", fileName = "new ShopPresentationConfig")]
    public class ShopPresentationConfig : ScriptableObject
    {
        public List<ShopPresentationItem> ShopPresentationItems;
        public List<ShopProductVisualContainer> ShopProductVisual;

        public ShopProductVisuals GetConfigByKey(string productKey)
        {
            return ShopProductVisual.FirstOrDefault(g => g.Key.Equals(productKey))?.Visuals;
        }
    }

    [Serializable]
    public class ShopPresentationItem
    {
        public string TagName;
        public TranslationData TabName;
        public List<string> Weapons;
    }

    [Serializable]
    public class ShopProductVisual
    {
        public enum ObtainType
        {
            None = 0,
            SoftCurrency = 1,
            HardCurrency = 2,
            Ad = 3,
            Money = 4
        }

        public string ProductKey;
        public string ProductName;
        public string Hash;
        public bool IsBoughtByDefault;
        public AssetReference ProductImage;
        public AssetReference ProductBackground;
        public AssetReference Product3DModel;

        public ObtainType ObtainBy;
        [Sirenix.OdinInspector.ShowIf("@this.ObtainBy == ObtainType.SoftCurrency || this.ObtainBy == ObtainType.HardCurrency || this.ObtainBy == ObtainType.Money")]
        public float Cost;
    }
}