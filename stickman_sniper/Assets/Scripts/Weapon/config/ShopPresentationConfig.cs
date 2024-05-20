using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "[GameWeaponConfig]/ShopPresentation", fileName = "new ShopPresentationConfig")]
public class ShopPresentationConfig : ScriptableObject
{
    public List<ShopPresentationItem> ShopPresentationItems;
    public List<ShopProductVisual> ShopProductVisual;
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
}