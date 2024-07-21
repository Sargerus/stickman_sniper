using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using stickman_sniper.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static CustomizationIndexes;

public class CustomizationScreenCertainWeapon : MonoBehaviour
{
    [SerializeField] private PodiumController podiumController;
    [SerializeField] private UIElementInputProvider uiElementInputProvider;

    [SerializeField, BoxGroup("Buttons")] private Button backButton;
    [SerializeField, BoxGroup("Buttons")] private Button chooseButton;

    [SerializeField, BoxGroup("Context")] private Transform container;
    [SerializeField, BoxGroup("Context")] private CustomizationScreenShopCell cellPrefab;

    [SerializeField, BoxGroup("Tabs")] private List<CustomizationScreenTab> tabs;

    private IPurchaseService _purchaseService;
    private ShopPresentationConfig _shopProductConfig;
    private WeaponCharacteristicsContainer _weaponCharacteristicsContainer;
    private IObserver<string> _backClickHandler;
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
        IPurchaseService purchaseService)
    {
        _shopProductConfig = shopProductConfig;
        _weaponCharacteristicsContainer = weaponCharacteristicsContainer;
        _purchaseService = purchaseService;
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

        Observable.EveryUpdate().Subscribe(_ =>
        {
            chooseButton.interactable = _currentIndexSelectedReactive.Value >= 0 && _currentIndexSelectedReactive.Value != _customizationIndexes.GetIndex(_currentTab);
        }).AddTo(_disposables);

        chooseButton.OnClickAsObservable().Subscribe(_ =>
        {
            _customizationIndexes.SetIndex(_currentTab, _currentIndexSelectedReactive.Value);
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
        Dictionary<int, string> content = new(); //<index, hash>

        switch (tab)
        {
            case AttachmentsTab.Scope: content = podiumController.AttachmentManager.GetScopes(); break;
            case AttachmentsTab.Muzzle: content = podiumController.AttachmentManager.GetMuzzles(); break;
            case AttachmentsTab.Laser: content = podiumController.AttachmentManager.GetLasers(); break;
            case AttachmentsTab.Grip: content = podiumController.AttachmentManager.GetGrips(); break;
            case AttachmentsTab.Magazine: content = podiumController.AttachmentManager.GetMagazines(); break;
        }

        for (int i = 0; i < content.Count; i++)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            ShopProductVisual weaponInventoryVisuals = _shopProductConfig.GetConfigByHash(content[i]);

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

            _currentIndexSelectedReactive.Value = index;
            SetPodiumAttachmentIndex(_currentTab, index);
        }).AddTo(_disposables);
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
