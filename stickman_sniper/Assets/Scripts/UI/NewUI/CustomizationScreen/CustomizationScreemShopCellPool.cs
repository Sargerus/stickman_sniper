using DWTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationScreemShopCellPool : AbstractMonoPool<CustomizationScreenShopCell>
{
    [SerializeField] private int defaultItemsCount = 5;
    [SerializeField] private CustomizationScreenShopCell cellPrefab;

    private List<CustomizationScreenShopCell> _items = new();

    public override IPooledItem<CustomizationScreenShopCell> CreateItem()
    {
        CustomizationScreenShopCell item = Instantiate(cellPrefab, transform);
        item.Pool = this;
        item.gameObject.SetActive(false);
        return item;
    }

    public override void DeactivateAllItems()
    {
        _trackingList.ForEach(item => item.ReturnToPool());
    }

    protected override void InitializePool()
    {
        for (int i = 0; i < defaultItemsCount; i++)
        {
            var item = CreateItem();
            _pool.Add(item);
            _trackingList.Add(item);
        }
    }
}
