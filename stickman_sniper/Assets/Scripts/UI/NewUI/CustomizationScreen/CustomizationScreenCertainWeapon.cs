using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stickman_sniper.Currency;
using stickman_sniper.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static CustomizationIndexes;

public class CustomizationScreenCertainWeapon : MonoBehaviour
{
    [SerializeField] private PodiumController podiumController;
    [SerializeField] private UIElementInputProvider uiElementInputProvider;
    [SerializeField] private TMP_Text currencyText;

    [SerializeField, BoxGroup("Buttons")] private Button backButton;
    [SerializeField, BoxGroup("Buttons")] private Button chooseButton;
    [SerializeField, BoxGroup("Buttons")] private Button buyButton;
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

    private ShopProductVisual _productVisuals;
    private List<CustomizationScreenShopCell> _cells = new();
    private CompositeDisposable _disposables = new();
    private AttachmentsTab _currentTab = AttachmentsTab.None;
    public ReactiveProperty<int> _currentIndexSelectedReactive = new(-1);
    private CustomizationIndexes _customizationIndexes;

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
        _productVisuals = _shopProductConfig.GetConfigByKey(key);
        var charContainer = _weaponCharacteristicsContainer.Config.FirstOrDefault(g => g.WeaponKey.Equals(key));

        if (charContainer != null)
        {
            _customizationIndexes = charContainer.CurrentCustomizationData.CustomizationIndexes;
        }

        backButton.OnClickAsObservable().Subscribe(_ => { _backClickHandler.OnNext(_key); }).AddTo(_disposables);

        _currentIndexSelectedReactive.Subscribe(index =>
        {
            RefreshButtonsState(index);
        }).AddTo(_disposables);

        //Observable.EveryUpdate().Subscribe(_ =>
        //{
        //    chooseButton.interactable = _currentIndexSelectedReactive.Value >= 0 && _currentIndexSelectedReactive.Value != _customizationIndexes.GetIndex(_currentTab);
        //}).AddTo(_disposables);

        chooseButton.OnClickAsObservable().Subscribe(_ =>
        {
            _customizationIndexes.SetIndex(_currentTab, _currentIndexSelectedReactive.Value);
            RefreshButtonsState(_currentIndexSelectedReactive.Value);
        }).AddTo(_disposables);

        buyButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (!_currentContent.TryGetValue(_currentIndexSelectedReactive.Value, out string hash))
                return;

            ShopProductVisual visuals = _shopProductConfig.GetConfigByHash(hash);

            switch(visuals.ObtainBy) 
            {
                case ShopProductVisual.ObtainType.SoftCurrency:
                    {
                        if (visuals.Cost > _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Value)
                            return;

                        _currencyService.AddCurrency(CurrencyServiceConstants.GoldCurrency, -visuals.Cost);
                        _purchaseService.Purchase(hash);
                        RefreshButtonsState(_currentIndexSelectedReactive.Value);
                        break;
                    }
            }
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
            currencyText.SetText(x.ToString());
        }).AddTo(_disposables);

        await podiumController.Initialize(_productVisuals, _customizationIndexes);
        SubscribeRawImage();
        InitializeTabs();
        SwitchTab(AttachmentsTab.Scope);
    }

    private void InitializeTabs()
    {
        List<string> asd = new() { "scope", "muzzle", "laser", "grip", "magazine" };

        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetTabName(asd[i])
                .SetOnClickHandler(asd[i], _tabClickHandler);
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

        for (int i = 0; i < _currentContent.Count; i++)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            ShopProductVisual weaponInventoryVisuals = _shopProductConfig.GetConfigByHash(_currentContent[i]);

            if (weaponInventoryVisuals.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponInventoryVisuals.Hash);
            }

            cell.Item.ResolveDependencies(_purchaseService);
            cell.Item.Init(weaponInventoryVisuals);
            cell.Item.SetOnClickHandler(i.ToString(), _cellClickHandler);
        }

        _currentTab = tab;
        _currentIndexSelectedReactive.Value = _customizationIndexes.GetIndex(_currentTab);

        _cellClickHandler.Subscribe(indexString =>
        {
            if (!int.TryParse(indexString, out int index))
                return;

            _currentIndexSelectedReactive.SetValueAndForceNotify(index);
            SetPodiumAttachmentIndex(_currentTab, index);
        }).AddTo(_disposables);
    }

    private void SetBuyButtonText(ShopProductVisual visual)
    {
        switch (visual.ObtainBy)
        {
            case ShopProductVisual.ObtainType.SoftCurrency:
                {
                    bool isEnoughMoney = _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Value >= visual.Cost;

                    buyButtonTextEnough.gameObject.SetActive(isEnoughMoney);
                    buyButtonTextNotEnough.gameObject.SetActive(!isEnoughMoney);

                    buyButtonTextEnough.SetText(visual.Cost.ToString());
                    buyButtonTextNotEnough.SetText(visual.Cost.ToString());

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
            chooseButton.gameObject.SetActive(_currentIndexSelectedReactive.Value >= 0 && _currentIndexSelectedReactive.Value != _customizationIndexes.GetIndex(_currentTab));
        }
        else
        {
            ShopProductVisual visuals = _shopProductConfig.GetConfigByHash(hash);

            SetBuyButtonText(visuals);
            buyButton.gameObject.SetActive(true);
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
    }
}
