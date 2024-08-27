using Cysharp.Threading.Tasks;
using DWTools;
using stickman_sniper.Purchases;
using System;
using TMPro;
using UniRx;
using UnityEngine;

public class CustomizationScreenShopCell : MonoBehaviour, IPooledItem<CustomizationScreenShopCell>
{
    public enum CellState
    {
        None = 0,
        Open = 1,
        Closed = 2,
        Blocked = 3
    }

    [SerializeField] private Transform bgImageParent;
    [SerializeField] private Transform itemImageParent;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text productText;
    [SerializeField] private UnityEngine.UI.Button button;

    private IPurchaseService _purchaseService;

    private GameObject _bgImage;
    private GameObject _itemImage;
    private CellState _cellState;
    private CompositeDisposable _disposables = new();

    public CustomizationScreenShopCell Item => this;
    private ReactiveProperty<CellState> _cellStateReactive = new();
    public IReadOnlyReactiveProperty<CellState> State => _cellStateReactive;
    public IPool<CustomizationScreenShopCell> Pool { get; set; }

    public void ResolveDependencies(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    public async UniTask Init(ShopProductVisual visual)
    {
        string GetCostText(ShopProductVisual visual) => visual.ObtainBy switch
        {
            ShopProductVisual.ObtainType.SoftCurrency => "<sprite name=\"ic_coin\"> " + visual.Cost.ToString(),
            ShopProductVisual.ObtainType.HardCurrency => visual.Cost.ToString(),
            ShopProductVisual.ObtainType.Money => "$" + visual.Cost.ToString(),
            ShopProductVisual.ObtainType.Ad => "<sprite name=\"ic_ad\">",
            _ => string.Empty
        };

        if (visual.ProductBackground.RuntimeKeyIsValid())
        {
            SetBackground(await visual.ProductBackground.InstantiateAsync(), false);
        }

        if (visual.ProductImage.RuntimeKeyIsValid())
        {
            SetItemImage(await visual.ProductImage.InstantiateAsync(), false);
        }

        var boughtProperty = _purchaseService.GetIsPurchasedReactiveProperty(visual.Hash);
        Item.SetCostText(GetCostText(visual));
        Item.SetProductText(visual.ProductName);
        if (!boughtProperty.Value)
        {            
            boughtProperty.Where(x => x == true).Subscribe(_ =>
            {
                _cellStateReactive.Value = CellState.Open;
            }).AddTo(_disposables);
        }

        _cellStateReactive.Value = boughtProperty.Value ? CellState.Open : CellState.Closed;
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

    private CustomizationScreenShopCell SetCostText(string text)
    {
        costText.SetText(text);
        return this;
    }

    private CustomizationScreenShopCell SetProductText(string text)
    {
        productText.SetText(text);
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
        costText.SetText(string.Empty);

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
