using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HashToProductKeyMapper : MonoBehaviour
{
    [SerializeField]
    private List<ShopProductVisuals> _visuals;

    private Dictionary<string, string> _hashToProductKey;

    public void FilLCache()
    {
        _hashToProductKey = new();

        foreach (var visual in _visuals.SelectMany(g => g.Items))
        {
            _hashToProductKey.Add(visual.Hash, visual.ProductKey);
        }
    }

    public string GetProductKeyByHash(string hash)
    {
        if (string.IsNullOrEmpty(hash))
            return null;

        string result = string.Empty;
        _hashToProductKey.TryGetValue(hash, out result);
        return result;
    }
}
