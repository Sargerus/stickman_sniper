using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "[GameWeaponConfig]/ShopProductVisuals", fileName = "new ShopProductVisualsItem")]
public class ShopProductVisuals : ScriptableObject
{
    [TableList]
    public List<ShopProductVisual> Items;

    public ShopProductVisual GetItemByProductKey(string productKey)
    {
        return Items.FirstOrDefault(g => g.ProductKey.Equals(productKey));
    }

    public ShopProductVisual GetItemByHash(string hash)
    {
        return Items.FirstOrDefault(g => g.Hash.Equals(hash));
    }
}

[Serializable]
public class ShopProductVisualContainer
{
    public string Key;
    public ShopProductVisuals Visuals;
}