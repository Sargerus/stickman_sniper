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
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using YG;
using static BootstrapSceneState;

public class CustomizationScreenCertainWeapon : MonoBehaviour
{
    [SerializeField] private PodiumController podiumController;
    [SerializeField] private UIElementInputProvider uiElementInputProvider;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private TMP_Text weaponText;
    [SerializeField] private CustomizationScreenCertainWeaponItemsHandler weaponItemsHandler;

    [SerializeField, BoxGroup("Buttons")] private Button backButton;
    [SerializeField, BoxGroup("Buttons")] private Button chooseButton;
    [SerializeField, BoxGroup("Buttons")] private Button buyButton;
    [SerializeField, BoxGroup("Buttons")] private Button adButton;
    [SerializeField, BoxGroup("Buttons")] private TMP_Text buyButtonTextEnough;
    [SerializeField, BoxGroup("Buttons")] private TMP_Text buyButtonTextNotEnough;

    [SerializeField, BoxGroup("Tabs")] private Transform tabsContainer;
    [SerializeField, BoxGroup("Tabs")] private CustomizationScreenTab tabPrefab;

    private ReactiveProperty<AttachmentsTab> _currentTab = new(AttachmentsTab.None);
    public ReactiveProperty<int> _currentChosen = new();
    public Dictionary<AttachmentsTab, ReactiveProperty<int>> _selectedItems = new();
    public ReactiveProperty<string> _currentIndexChosenReactiveString = new();

    private ReactiveProperty<int> _currentSelected => _selectedItems[_currentTab.Value];
    private IPurchaseService _purchaseService;
    private ICurrencyService _currencyService;
    private ShopPresentationConfig _shopProductConfig;
    private ISaveService _saveService;
    private WeaponCharacteristicsContainer _weaponCharacteristicsContainer;
    private IObserver<string> _backClickHandler;
    private Dictionary<int, string> _currentTabAttachments = new();
    private string _key;
    private Subject<string> _cellClickHandler = new();
    private Subject<AttachmentsTab> _tabClickHandler = new();

    private ShopProductVisuals _productVisualsContainer;
    private CompositeDisposable _disposables = new();
    private CustomizationIndexes _customizationIndexes;
    private List<AsyncOperationHandle> _listOfDependencies = new();
    private List<CustomizationScreenTab> _tabs = new();
    private WeaponIndexes _weaponIndexes;

    public void ResolveDependencies(ShopPresentationConfig shopProductConfig,
        WeaponCharacteristicsContainer weaponCharacteristicsContainer,
        IPurchaseService purchaseService, ICurrencyService currencyService,
        ISaveService saveService)
    {
        _shopProductConfig = shopProductConfig;
        _weaponCharacteristicsContainer = weaponCharacteristicsContainer;
        _purchaseService = purchaseService;
        _currencyService = currencyService;
        _saveService = saveService;
    }

    public async UniTask Init(string key, IObserver<string> backClickHandler)
    {
        _key = key;
        _backClickHandler = backClickHandler;
        _productVisualsContainer = _shopProductConfig.GetConfigByKey(key);

        var weaponItem = _productVisualsContainer.GetItemByProductKey(key);
        _weaponIndexes = _saveService.GetWeaponIndexes().FirstOrDefault(g => g.WeaponKey.Equals(key));
        
        weaponText.SetText(weaponItem.ProductName);        

        var charContainer = _weaponCharacteristicsContainer.Config.FirstOrDefault(g => g.WeaponKey.Equals(key));
        if (charContainer != null)
        {
            _customizationIndexes = charContainer.CurrentCustomizationData.CustomizationIndexes;
        }        

        await PrelaodDependencies();
        await podiumController.Initialize(weaponItem, _customizationIndexes);
        await weaponItemsHandler.Initialize(_currentTab,
            _selectedItems,
            _currentChosen,
            podiumController.AttachmentManager,
            _productVisualsContainer,
            _purchaseService,
            _customizationIndexes);

        InitializeTabs();
        SubscribeRawImage();
        SwitchTab(AttachmentsTab.Scope);
        RefreshButtonsState(_currentChosen.Value);
        Subscribe();
    }

    private void Subscribe()
    {
        backButton.OnClickAsObservable().Subscribe(_ => { _backClickHandler.OnNext(_key); }).AddTo(_disposables);

        chooseButton.OnClickAsObservable().Subscribe(_ =>
        {
            int newIndex = _currentSelected.Value;

            //this workaround was set for WebGL for saving, because WebGL doesnt suppor save through scriptables: they are cleared at start
            _weaponIndexes.Indexes.SetIndex(_currentTab.Value, newIndex);
            _saveService.SaveProgress();
            _customizationIndexes.SetIndex(_currentTab.Value, newIndex);
            podiumController.SetPodiumAttachmentIndex(_currentTab.Value, newIndex);
            _currentChosen.Value = newIndex;
        }).AddTo(_disposables);

        buyButton.OnClickAsObservable().Subscribe(_ =>
        {
            var hash = _currentTabAttachments[_currentSelected.Value];
            ShopProductVisual visuals = _productVisualsContainer.GetItemByHash(hash);

            switch (visuals.ObtainBy)
            {
                case ShopProductVisual.ObtainType.SoftCurrency:
                    {
                        if (visuals.Cost > _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Value)
                            return;

                        _currencyService.AddCurrency(CurrencyServiceConstants.GoldCurrency, -visuals.Cost);
                        _purchaseService.Purchase(visuals.Hash);
                        break;
                    }
            }
        }).AddTo(_disposables);

        adButton.OnClickAsObservable().Subscribe(_ =>
        {
            var hash = _currentTabAttachments[_currentSelected.Value];
            ShopProductVisual visuals = _productVisualsContainer.GetItemByHash(hash);

            switch (visuals.ObtainBy)
            {
                case ShopProductVisual.ObtainType.Ad:
                    {
                        Debug.Log($"AAA pure hash {visuals.Hash}");

                        _purchaseService.SetNextBuyByRewardedAd(visuals.Hash);
                        YandexGame.RewVideoShow(1);
                        break;
                    }
            }
        }).AddTo(_disposables);

        _tabClickHandler.Subscribe(tabKey =>
        {
            SwitchTab(tabKey);
        }).AddTo(_disposables);

        _currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Subscribe(x =>
        {
            currencyText.SetText($"<sprite name=\"ic_coin\"> {x}");
        }).AddTo(_disposables);

        _selectedItems.Values.Merge()
            .Merge(_currentChosen)
            .Merge(_purchaseService.OnPurchaseComplete.Select(_ => 1))
            .Merge(_currencyService.GetCurrency(CurrencyServiceConstants.GoldCurrency).Select(_ => 1))
            .Subscribe(_ =>
        {
            RefreshButtonsState(_currentSelected.Value);
        }).AddTo(_disposables);

        //podium
        foreach(var tab in _productVisualsContainer.AvailableTabs)
        {
            _selectedItems[tab].SkipLatestValueOnSubscribe().SubscribeWithState(tab, (x, tab) =>
            {
                podiumController.SetPodiumAttachmentIndex(tab, x);
            }).AddTo(_disposables);
        }
    }

    private int GetAttachmentIndexByProductKey(string productKey)
    {
        if (string.IsNullOrEmpty(productKey))
            return -1;

        var values = _currentTabAttachments.Values.ToList();
        var hash = _productVisualsContainer.GetItemByProductKey(productKey).Hash;
        return values.IndexOf(hash);
    }

    private async UniTask PrelaodDependencies()
    {
        IReadOnlyList<AssetReference> GetUniqueReferences(IReadOnlyList<AssetReference> references)
        {
            HashSet<object> uniqueKeys = new();
            List<AssetReference> uniqueReferences = new();

            foreach (var reference in references)
            {
                if (reference.RuntimeKeyIsValid() && uniqueKeys.Add(reference.RuntimeKey))
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
        for (int i = 0; i < _productVisualsContainer.AvailableTabs.Count; i++)
        {
            AttachmentsTab tab = _productVisualsContainer.AvailableTabs[i];
            CustomizationScreenTab newTab = Instantiate(tabPrefab, tabsContainer);
            newTab.SetTab(tab).SetOnClickHandler(tab, _tabClickHandler);
            _selectedItems.Add(tab, new(_customizationIndexes.GetIndex(tab)));

            _tabs.Add(newTab);
        }

        _selectedItems.Add(AttachmentsTab.None, new(-1));
    }

    private void SwitchTab(AttachmentsTab tab)
    {
        if (tab == _currentTab.Value)
            return;

        weaponItemsHandler.ClearTab();

        _currentTabAttachments = podiumController.GetAttachments(tab);
        _currentTab.Value = tab;
        _currentChosen.Value = _customizationIndexes.GetIndex(tab);
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

    private void RefreshButtonsState(int index)
    {
        if (index < 0)
        {
            buyButton.gameObject.SetActive(false);
            adButton.gameObject.SetActive(false);
            return;
        }

        string hash = _currentTabAttachments[index];

        if (_purchaseService.GetIsPurchasedReactiveProperty(hash).Value)
        {
            buyButton.gameObject.SetActive(false);
            adButton.gameObject.SetActive(false);
            chooseButton.gameObject.SetActive(_currentSelected.Value >= 0 && index != _customizationIndexes.GetIndex(_currentTab.Value));
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

    private void SubscribeRawImage()
    {
        uiElementInputProvider.MouseMoveObservable.Subscribe(delta =>
        {
            podiumController.ApplyInput(delta);
        }).AddTo(_disposables);
    }

    public async UniTask DeInit()
    {
        _currentTab.Value = AttachmentsTab.None;
        podiumController.Clear();
        _disposables.Clear();
        _listOfDependencies.ForEach(g => Addressables.Release(g));
        _listOfDependencies.Clear();
        _currentChosen.Value = -1;
        _tabs.ForEach(g => Destroy(g.gameObject));
        _tabs.Clear();
        _selectedItems?.Clear();
    }
}
