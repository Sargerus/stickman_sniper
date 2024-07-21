using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "[GameWeaponConfig]/ShopPresentationItem", fileName = "new ShopPresentationConfigItem")]
public class ShopItemPresentationConfig : ScriptableObject
{
    [TableList]
    public List<ShopProductVisual> Items;
}