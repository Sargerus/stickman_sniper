using Currency;
using Cysharp.Threading.Tasks;
using Purchase;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using YG;
using static CustomizationIndexes;

public class CustomizationScreenCertainWeapon : MonoBehaviour
{
    [SerializeField] private PodiumController podiumController;
    [SerializeField] private UIElementInputProvider uiElementInputProvider;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private TMP_Text weaponText;

    [SerializeField, BoxGroup("Buttons")] private Button backButton;
    [SerializeField, BoxGroup("Buttons")] private Button chooseButton;
    [SerializeField, BoxGroup("Buttons")] private Button buyButton;
    [SerializeField, BoxGroup("Buttons")] private Button adButton;
    [SerializeField, BoxGroup("Buttons")] private TMP_Text buyButtonTextEnough;
    [SerializeField, BoxGroup("Buttons")] private TMP_Text buyButtonTextNotEnough;

    [SerializeField, BoxGroup("Context")] private Transform container;
    [SerializeField, BoxGroup("Context")] private CustomizationScreenShopCell cellPrefab;

    [SerializeField, BoxGroup("Tabs")] private List<CustomizationScreenTab> tabs;

    private IPurchaseService _purchaseService;
    private ICurrencyService _currencyService;
    private ShopPresentationConfig _shopProductConfig;
    private WeaponCharacteristicsContainer _weaponCharacteristicsContainer;
    private IObserver<string> _backClickHandler;
    private Dictionary<int, string> _currentContent = new();
    private string _key;
    private Subject<string> _cellClickHandler = new();
    private Subject<string> _tabClickHandler = new();

    private ShopProductVisuals _productVisualsContainer;
    private List<CustomizationScreenShopCell> _cells = new();
    private CompositeDisposable _disposables = new();
    private AttachmentsTab _currentTab = AttachmentsTab.None;
    public ReactiveProperty<int> _currentIndexSelectedReactive = new(-1);
    public ReactiveProperty<string> _currentIndexSelectedReactiveString = new();
    private CustomizationIndexes _customizationIndexes;
    private List<AsyncOperationHandle> _listOfDependencies = new();

    public void ResolveDependencies(ShopPresentationConfig shopProductConfig,
        WeaponCharacteristicsContainer weaponCharacteristicsContainer,
        IPurchaseService purchaseService, ICurrencyService currencyService)
    {
        _shopProductConfig = shopProductConfig;
        _weaponCharacteristicsContainer = weaponCharacteristicsContainer;
        _purchaseService = purchaseService;
        _currencyService = currencyService;
    }

    public async UniTask Init(string key, IObserver<string> backClickHandler)
    {
        _key = key;
        _backClickHandler = backClickHandler;
        _productVisualsContainer = _shopProductConfig.GetConfigByKey(key);
        var weaponItem = _productVisualsContainer.GetItemByProductKey(key);

        weaponText.SetText(weaponItem.ProductName);

        var charContainer = _weaponCharacteristicsContainer.Config.FirstOrDefault(g => g.WeaponKey.Equals(key));
        if (charContainer != null)
        {
            _customizationIndexes = charContainer.CurrentCustomizationData.CustomizationIndexes;
        }

        await PrelaodDependencies();
        await podiumController.Initialize(weaponItem, _customizationIndexes);
        Subscribe();
        RefreshButtonsState(_currentIndexSelectedReactive.Value);
        SubscribeRawImage();
        InitializeTabs();
        SwitchTab(AttachmentsTab.Scope);
    }

    private void Subscribe()
    {
        backButton.OnClickAsObservable().Subscribe(_ => { _backClickHandler.OnNext(_key); }).AddTo(_disposables);

        _currentIndexSelectedReactive.SkipLatestValueOnSubscribe().Subscribe(index =>
        {
            if (index >= 0)
            {
                var i = _productVisualsContainer.GetItemByHash(_currentContent[index]);
                _currentIndexSelectedReactiveString.SetValueAndForceNotify(i.ProductKey);
            }
            else _currentIndexSelectedReactiveString.Value = string.Empty;

            RefreshButtonsState(index);
        }).AddTo(_disposables);

        chooseButton.OnClickAsObservable().Subscribe(_ =>
        {
            int newIndex = _currentIndexSelectedReactive.Value;

            _customizationIndexes.SetIndex(_currentTab, newIndex);

            if (newIndex >= 0)
            {
                var currentSelected = _cells.FirstOrDefault(g => g.State.Value == CellState.Selected);
                if (currentSelected != null)
                    currentSelected.SetState(CellState.Purchased);

                _cells[newIndex].SetState(CellState.Selected);
            }
            else
            {
                _cells.ForEach(g => g.SetState(GetCellState(g)));
            }

            RefreshButtonsState(_currentIndexSelectedReactive.Value);
        }).AddTo(_disposables);

        buyButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (!_currentContent.TryGetValue(_currentIndexSelectedReactive.Value, out string hash))
                return;

            ShopProductVisual visuals = _productVisualsContainer.GetItemByHash(hash);

            switch (visuals.ObtainBy)
            {
                case ShopProductVisual.ObtainType.SoftCurrency:
                    {
                        if (visuals.Cost > _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Value)
                            return;

                        _currencyService.AddCurrency(CurrencyServiceConstants.GoldCurrency, -visuals.Cost);
                        _purchaseService.Purchase(hash);
                        break;
                    }
            }

            var selectedCell = _cells[_currentIndexSelectedReactive.Value];
            selectedCell.SetState(GetCellState(selectedCell));
        }).AddTo(_disposables);

        adButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (!_currentContent.TryGetValue(_currentIndexSelectedReactive.Value, out string hash))
                return;

            ShopProductVisual visuals = _productVisualsContainer.GetItemByHash(hash);

            switch (visuals.ObtainBy)
            {
                case ShopProductVisual.ObtainType.Ad:
                    {
                        Debug.Log($"AAA pure hash {hash}");

                        _purchaseService.SetNextBuyByRewardedAd(hash);
                        YandexGame.RewVideoShow(1);

                        break;
                    }
            }

            var selectedCell = _cells[_currentIndexSelectedReactive.Value];
            selectedCell.SetState(GetCellState(selectedCell));
        }).AddTo(_disposables);

        _tabClickHandler.Subscribe(tabKey =>
        {
            AttachmentsTab ConvertStringToTab(string key) => key switch
            {
                "scope" => AttachmentsTab.Scope,
                "muzzle" => AttachmentsTab.Muzzle,
                "laser" => AttachmentsTab.Laser,
                "grip" => AttachmentsTab.Grip,
                "magazine" => AttachmentsTab.Magazine
            };

            SwitchTab(ConvertStringToTab(tabKey));
        }).AddTo(_disposables);

        _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Subscribe(x =>
        {
            currencyText.SetText($"<sprite name=\"ic_coin\"> {x}");
        }).AddTo(_disposables);

        _purchaseService.OnPurchaseComplete.Subscribe(_ =>
        {
            RefreshButtonsState(_currentIndexSelectedReactive.Value);
        }).AddTo(_disposables);
    }

    private async UniTask PrelaodDependencies()
    {
        IReadOnlyList<AssetReference> GetUniqueReferences(IReadOnlyList<AssetReference> references)
        {
            HashSet<object> uniqueKeys = new();
            List<AssetReference> uniqueReferences = new();

            foreach (var reference in references)
            {
                if (uniqueKeys.Add(reference.RuntimeKey))
                {
                    uniqueReferences.Add(reference);
                }
            }

            return uniqueReferences;
        }

        var images = GetUniqueReferences(_productVisualsContainer.Items.Select(g => g.ProductImage).ToList());
        var backgrounds = GetUniqueReferences(_productVisualsContainer.Items.Select(g => g.ProductBackground).ToList());
        var models = GetUniqueReferences(_productVisualsContainer.Items.Select(g => g.Product3DModel).ToList());

        foreach (var item in images.Concat(backgrounds).Concat(models))
        {
            _listOfDependencies.Add(Addressables.LoadAssetAsync<GameObject>(item));
        }

        await UniTask.WhenAll(_listOfDependencies.Select(g => g.ToUniTask()));
    }

    private void InitializeTabs()
    {
        List<string> asd = new() { "scope", "muzzle", "laser", "grip", "magazine" };

        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetOnClickHandler(asd[i], _tabClickHandler);
        }
    }

    private void SwitchTab(AttachmentsTab tab)
    {
        if (tab == _currentTab)
            return;

        if (_currentTab != AttachmentsTab.None)
            SetPodiumAttachmentIndex(_currentTab, _customizationIndexes.GetIndex(_currentTab));

        ClearCells();

        switch (tab)
        {
            case AttachmentsTab.Scope: _currentContent = podiumController.AttachmentManager.GetScopes(); break;
            case AttachmentsTab.Muzzle: _currentContent = podiumController.AttachmentManager.GetMuzzles(); break;
            case AttachmentsTab.Laser: _currentContent = podiumController.AttachmentManager.GetLasers(); break;
            case AttachmentsTab.Grip: _currentContent = podiumController.AttachmentManager.GetGrips(); break;
            case AttachmentsTab.Magazine: _currentContent = podiumController.AttachmentManager.GetMagazines(); break;
        }

        _currentTab = tab;
        _currentIndexSelectedReactive.Value = _customizationIndexes.GetIndex(_currentTab);

        for (int i = 0; i < _currentContent.Count; i++)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            ShopProductVisual weaponInventoryVisuals = _productVisualsContainer.GetItemByHash(_currentContent[i]);

            if (weaponInventoryVisuals.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponInventoryVisuals.Hash);
            }

            InitCellState(cell, weaponInventoryVisuals, i);
        }

        _cellClickHandler.Subscribe(indexString =>
        {
            if (!int.TryParse(indexString, out int index))
                return;

            _currentIndexSelectedReactive.SetValueAndForceNotify(index);
            SetPodiumAttachmentIndex(_currentTab, index);
        }).AddTo(_disposables);
    }

    private void InitCellState(CustomizationScreenShopCell cell, ShopProductVisual weaponInventoryVisuals, int orderNumber)
    {
        cell.Item.ResolveDependencies();
        cell.Item.Init(weaponInventoryVisuals, _currentIndexSelectedReactiveString);
        cell.Item.SetOnClickHandler(orderNumber.ToString(), _cellClickHandler);

        CellState state = GetCellState(cell);
        cell.SetState(state);

        var purchasedProprerty = _purchaseService.GetIsPurchasedReactiveProperty(weaponInventoryVisuals.Hash);
        if (!purchasedProprerty.Value)
        {
            purchasedProprerty.Where(x => x).SubscribeWithState(cell, (_, cell) => cell.Item.SetState(GetCellState(cell))).AddTo(_disposables);
        }
    }

    private CellState GetCellState(CustomizationScreenShopCell cell)
    {
        CellState result = CellState.None;

        int selectedIndex = _customizationIndexes.GetIndex(_currentTab);
        var purchasedProprerty = _purchaseService.GetIsPurchasedReactiveProperty(cell.Visual.Hash);
        bool isPurchased = purchasedProprerty.Value;
        bool isSelected = _cells.IndexOf(cell) == selectedIndex;

        if (isSelected)
        {
            result = CellState.Selected;
        }
        else if (isPurchased)
        {
            result = CellState.Purchased;
        }
        else result = CellState.Available;

        return result;
    }

    private void SetBuyButtonText(ShopProductVisual visual)
    {
        switch (visual.ObtainBy)
        {
            case ShopProductVisual.ObtainType.SoftCurrency:
                {
                    bool isEnoughMoney = _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Value >= visual.Cost;

                    adButton.gameObject.SetActive(false);
                    buyButtonTextEnough.gameObject.SetActive(isEnoughMoney);
                    buyButtonTextNotEnough.gameObject.SetActive(!isEnoughMoney);

                    buyButtonTextEnough.SetText($"<sprite name=\"ic_coin\"> {visual.Cost}");
                    buyButtonTextNotEnough.SetText($"<sprite name=\"ic_coin\"> {visual.Cost}");

                    break;
                }
        }
    }

    private void SetPodiumAttachmentIndex(AttachmentsTab tab, int index)
    {
        switch (tab)
        {
            case AttachmentsTab.Scope: podiumController.AttachmentManager.SetScopeIndex(index); return;
            case AttachmentsTab.Muzzle: podiumController.AttachmentManager.SetMuzzleIndex(index); return;
            case AttachmentsTab.Laser: podiumController.AttachmentManager.SetLaserIndex(index); return;
            case AttachmentsTab.Grip: podiumController.AttachmentManager.SetGripIndex(index); return;
            case AttachmentsTab.Magazine: podiumController.AttachmentManager.SetMagazineIndex(index); return;
        }
    }

    private void RefreshButtonsState(int index)
    {
        if (!_currentContent.TryGetValue(index, out string hash))
            return;

        if (_purchaseService.GetIsPurchasedReactiveProperty(hash).Value)
        {
            buyButton.gameObject.SetActive(false);
            adButton.gameObject.SetActive(false);
            chooseButton.gameObject.SetActive(_currentIndexSelectedReactive.Value >= 0 && _currentIndexSelectedReactive.Value != _customizationIndexes.GetIndex(_currentTab));
        }
        else
        {
            ShopProductVisual visuals = _productVisualsContainer.GetItemByHash(hash);

            switch (visuals.ObtainBy)
            {
                case ShopProductVisual.ObtainType.SoftCurrency:
                    buyButton.gameObject.SetActive(true);
                    adButton.gameObject.SetActive(false);
                    SetBuyButtonText(visuals);
                    break;
                case ShopProductVisual.ObtainType.Ad:
                    buyButton.gameObject.SetActive(false);
                    adButton.gameObject.SetActive(true);
                    break;
            }

            chooseButton.gameObject.SetActive(false);
        }
    }

    private void ClearCells()
    {
        _cells.ForEach(g => Destroy(g.gameObject));
        _cells.Clear();
    }

    private void SubscribeRawImage()
    {
        uiElementInputProvider.MouseMoveObservable.Subscribe(delta =>
        {
            podiumController.ApplyInput(delta);
        }).AddTo(_disposables);
    }

    public async UniTask DeInit()
    {
        _currentTab = AttachmentsTab.None;
        podiumController.Clear();
        ClearCells();
        _disposables.Clear();
        _listOfDependencies.ForEach(g => Addressables.Release(g));
        _listOfDependencies.Clear();
        _currentIndexSelectedReactive.Value = -1;
    }
}
