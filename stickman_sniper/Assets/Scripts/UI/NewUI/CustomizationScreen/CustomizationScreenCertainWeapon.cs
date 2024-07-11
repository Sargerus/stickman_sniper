using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
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
        WeaponCharacteristicsContainer weaponCharacteristicsContainer)
    {
        _shopProductConfig = shopProductConfig;
        _weaponCharacteristicsContainer = weaponCharacteristicsContainer;
    }

    public async UniTask Init(string key, IObserver<string> backClickHandler)
    {
        _key = key;
        _backClickHandler = backClickHandler;
        _productVisuals = _shopProductConfig.GetConfig(key);
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

        ClearCells();
        Dictionary<int, string> content = new(); //<index, hash>

        switch (tab)
        {
            case AttachmentsTab.Scope: content = podiumController.AttachmentManager.GetScopes(); break;
            case AttachmentsTab.Muzzle: content = podiumController.AttachmentManager.GetMuzzles(); break;
        }

        for (int i = 0; i < content.Count; i++)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            var weaponInventoryVisuals = _shopProductConfig.ShopProductVisual.FirstOrDefault(g => g.Hash.Equals(content[i]));
            cell.Item.SetText(weaponInventoryVisuals.ProductName)
                .SetOnClickHandler(i.ToString(), _cellClickHandler)
                .ContinueWith(async g =>
                {
                    if (weaponInventoryVisuals.ProductBackground.RuntimeKeyIsValid())
                    {
                        g.SetBackground(await weaponInventoryVisuals.ProductBackground.InstantiateAsync(), false);
                    }
                })
                .ContinueWith(async g =>
                {
                    if (weaponInventoryVisuals.ProductImage.RuntimeKeyIsValid())
                    {
                        g.SetItemImage(await weaponInventoryVisuals.ProductImage.InstantiateAsync(), false);
                    }
                });
        }

        _currentTab = tab;
        _currentIndexSelectedReactive.Value = _customizationIndexes.GetIndex(_currentTab);

        _cellClickHandler.Subscribe(indexString =>
        {
            if (!int.TryParse(indexString, out int index))
                return;

            _currentIndexSelectedReactive.Value = index;

            switch (_currentTab)
            {
                case AttachmentsTab.Scope: podiumController.AttachmentManager.SetScopeIndex(index); return;
                case AttachmentsTab.Muzzle: podiumController.AttachmentManager.SetMuzzleIndex(index); return;
            }
        }).AddTo(_disposables);
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
