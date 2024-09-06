using Cysharp.Threading.Tasks;
using DWTools;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UniRx;
using UnityEngine;

public enum CellState
{
    None = 0,
    Available = 1,
    Purchased = 2,
    Selected = 3
}

public class CustomizationScreenShopCell : MonoBehaviour, IPooledItem<CustomizationScreenShopCell>
{
    [SerializeField] private Transform bgImageParent;
    [SerializeField] private Transform itemImageParent;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text productText;
    [SerializeField] private UnityEngine.UI.Button button;
    [SerializeField, BoxGroup("Panels")] private GameObject purchasedPanel;
    [SerializeField, BoxGroup("Panels")] private GameObject selectedPanel;

    private GameObject _bgImage;
    private GameObject _itemImage;
    private UISpriteSwaper _swaper;
    private IReadOnlyReactiveProperty<string> _selectedItem;
    private CompositeDisposable _disposables = new();

    public ShopProductVisual Visual;
    public CustomizationScreenShopCell Item => this;

    private ReactiveProperty<CellState> _state = new();
    public IReadOnlyReactiveProperty<CellState> State => _state;

    public IPool<CustomizationScreenShopCell> Pool { get; set; }

    public void ResolveDependencies()
    {
    }

    public async UniTask Init(ShopProductVisual visual, IReadOnlyReactiveProperty<string> selectedItem)
    {
        Visual = visual;
        _selectedItem = selectedItem;

        if (visual.ProductBackground.RuntimeKeyIsValid())
        {
            SetBackground(await visual.ProductBackground.InstantiateAsync(), false);
        }

        if (visual.ProductImage.RuntimeKeyIsValid())
        {
            SetItemImage(await visual.ProductImage.InstantiateAsync(), false);
        }

        Item.SetProductText(visual.ProductName);

        if (selectedItem != null)
            _selectedItem.SkipLatestValueOnSubscribe().Subscribe(x =>
            {
                _swaper.SetSelected(x.Equals(visual.ProductKey));
            }).AddTo(_disposables);
    }

    private CustomizationScreenShopCell SetBackground(GameObject image, bool worldPositionStays)
    {
        _bgImage = image;
        image.transform.SetParent(bgImageParent, worldPositionStays);

        _swaper = image.GetComponent<UISpriteSwaper>();

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
        costText.gameObject.SetActive(true);
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

    private CustomizationScreenShopCell SetAvailable()
    {
        string GetCostText(ShopProductVisual visual) => visual.ObtainBy switch
        {
            ShopProductVisual.ObtainType.SoftCurrency => "<sprite name=\"ic_coin\"> " + visual.Cost.ToString(),
            ShopProductVisual.ObtainType.HardCurrency => visual.Cost.ToString(),
            ShopProductVisual.ObtainType.Money => "$" + visual.Cost.ToString(),
            ShopProductVisual.ObtainType.Ad => "<sprite name=\"ic_ad\">",
            _ => string.Empty
        };

        Item.SetCostText(GetCostText(Visual));
        costText.gameObject.SetActive(true);
        selectedPanel.SetActive(false);
        purchasedPanel.SetActive(false);

        _state.Value = CellState.Available;
        return this;
    }

    private CustomizationScreenShopCell SetPurchased()
    {
        costText.gameObject.SetActive(false);
        selectedPanel.SetActive(false);
        purchasedPanel.SetActive(true);

        _state.Value = CellState.Purchased;
        return this;
    }

    private CustomizationScreenShopCell SetSelected()
    {
        costText.gameObject.SetActive(false);
        selectedPanel.SetActive(true);
        purchasedPanel.SetActive(false);

        _state.Value = CellState.Selected;
        return this;
    }

    public void SetState(CellState state)
    {
        switch (state)
        {
            case CellState.Available:
                SetAvailable(); return;
            case CellState.Selected:
                SetSelected(); return;
            case CellState.Purchased:
                SetPurchased(); return;
        }
    }

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
