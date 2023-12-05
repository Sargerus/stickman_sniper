using DWTools;
using UnityEngine;

public class BulletPool : AbstractMonoPool<IBulletView>
{
    [SerializeField] private BulletView _prefab;

    public override IPooledItem<IBulletView> CreateItem()
    {
        var go = Instantiate(_prefab, transform);
        go.gameObject.SetActive(false);
        go.Pool = this;

        return go.GetComponent<IBulletView>();
    }

    public override void DeactivateAllItems()
    {
        _trackingList.ForEach(item => item.ReturnToPool());
    }

    protected override void InitializePool()
    {
        for (int i = 0; i < 2; i++)
        {
            var item = CreateItem();
            _pool.Add(item);
            _trackingList.Add(item);
        }
    }
}
