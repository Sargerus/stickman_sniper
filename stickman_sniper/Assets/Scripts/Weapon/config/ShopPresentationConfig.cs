using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "[GameWeaponConfig]/ShopPresentation", fileName = "new ShopPresentationConfig")]
public class ShopPresentationConfig : ScriptableObject
{
    public List<ShopPresentationItem> ShopPresentationItems;
    public List<ShopProductVisual> ShopProductVisual;

    public ShopProductVisual GetConfig(string key)
    {
        return ShopProductVisual.FirstOrDefault(g => g.ProductKey.Equals(key));
    }
}

[Serializable]
public class ShopPresentationItem
{
    public string TagName;
    public string TabName;
}

[Serializable]
public class ShopProductVisual
{
    public string ProductKey;
    public string ProductName;
    public AssetReference ProductImage;
    public AssetReference ProductBackground;
    public AssetReference Product3DModel;
}