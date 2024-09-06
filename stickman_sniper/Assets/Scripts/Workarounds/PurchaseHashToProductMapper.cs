#if UNITY_EDITOR
using Purchase;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PurchaseHashToProductMapper : MonoBehaviour
{
    [SerializeField] private List<ShopProductVisuals> _visuals;
    [SerializeField] private HashToProductKeyMapper _keyMapper;

    [Button]
    private void FillCache()
    {
        foreach (var visual in _visuals.SelectMany(g => g.Items))
        {
            _keyMapper.Values.Add(new(visual.Hash, visual.ProductKey));
        }
    }
}
#endif