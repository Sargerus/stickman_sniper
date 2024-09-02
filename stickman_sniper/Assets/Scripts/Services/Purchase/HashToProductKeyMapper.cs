using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HashToProductKeyMapper : MonoBehaviour
{
    [SerializeField]
    private List<ShopProductVisuals> _visuals;

    private Dictionary<string, string> _hashToProductKey = new();

    public string GetProductKeyByHash(string hash)
    {
        string result = string.Empty;
        _hashToProductKey.TryGetValue(hash, out result);
        return result;
    }

    private void Start()
    {
        foreach (var visual in _visuals.SelectMany(g => g.Items))
        {
            _hashToProductKey.Add(visual.Hash, visual.ProductKey);
        }
    }
}
