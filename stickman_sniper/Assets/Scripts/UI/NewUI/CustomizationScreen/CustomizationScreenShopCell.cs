using Cysharp.Threading.Tasks;
using DWTools;
using stickman_sniper.Purchases;
using System;
using TMPro;
using UniRx;
using UnityEngine;

public class CustomizationScreenShopCell : MonoBehaviour, IPooledItem<CustomizationScreenShopCell>
{
    [SerializeField] private Transform bgImageParent;
    [SerializeField] private Transform itemImageParent;
    [SerializeField] private TMP_Text text;
    [SerializeField] private UnityEngine.UI.Button button;

    private IPurchaseService _purchaseService;

    private GameObject _bgImage;
    private GameObject _itemImage;
    private CompositeDisposable _disposables = new();

    public CustomizationScreenShopCell Item => this;
    public IPool<CustomizationScreenShopCell> Pool { get; set; }

    public void ResolveDependencies(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    public async UniTask Init(ShopProductVisual visual)
    {
        string GetProductText(ShopProductVisual visual) => visual.ObtainBy switch
        {
            ShopProductVisual.ObtainType.SoftCurrency => visual.Cost.ToString(),
            _ => visual.Cost.ToString()
        };

        Item.SetText(GetProductText(visual));

        if (visual.ProductBackground.RuntimeKeyIsValid())
        {
            SetBackground(await visual.ProductBackground.InstantiateAsync(), false);
        }

        if (visual.ProductImage.RuntimeKeyIsValid())
        {
            SetItemImage(await visual.ProductImage.InstantiateAsync(), false);
        }
    }

    private CustomizationScreenShopCell SetBackground(GameObject image, bool worldPositionStays)
    {
        _bgImage = image;
        image.transform.SetParent(bgImageParent, worldPositionStays);
        return this;
    }

    private CustomizationScreenShopCell SetItemImage(GameObject image, bool worldPositionStays)
    {
        _itemImage = image;
        image.transform.SetParent(itemImageParent, worldPositionStays);
        return this;
    }

    private CustomizationScreenShopCell SetText(string text)
    {
        this.text.SetText(text);
        return this;
    }

    public void SetOnClickHandler(string key, IObserver<string> onClickHandler)
    {
        button.OnClickAsObservable().SubscribeWithState2(key, onClickHandler, (_, key, onClickHandler) =>
        {
            onClickHandler.OnNext(key);
        }).AddTo(_disposables);
    }

    //public CustomizationScreenShopCell ContinueWith(Action<CustomizationScreenShopCell> action)
    //{
    //    action?.Invoke(this);
    //    return this;
    //}

    public void ReturnToPool()
    {
        text.SetText(string.Empty);

        if (_bgImage != null)
            Destroy(_bgImage);
        if (_itemImage != null)
            Destroy(_itemImage);

        gameObject.SetActive(false);
        Pool.Add(this);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
