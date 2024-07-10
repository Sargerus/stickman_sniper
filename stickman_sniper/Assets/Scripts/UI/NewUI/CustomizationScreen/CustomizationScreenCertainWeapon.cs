using Cysharp.Threading.Tasks;
using DWTools;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationScreenCertainWeapon : MonoBehaviour
{
    private enum AttachmentsTab
    {
        None = 0,
        Scope = 1
    }

    [SerializeField] private PodiumController podiumController;
    [SerializeField] private UIElementInputProvider uiElementInputProvider;
    [SerializeField] private Button backButton;

    [SerializeField, BoxGroup("Context")] private Transform container;
    [SerializeField, BoxGroup("Context")] private CustomizationScreenShopCell cellPrefab;

    private ShopPresentationConfig _shopProductConfig;
    private WeaponCharacteristicsContainer _weaponCharacteristicsContainer;
    private IObserver<string> _backClickHandler;
    private string _key;

    private ShopProductVisual _productVisuals;
    private List<CustomizationScreenShopCell> _cells = new();
    private CompositeDisposable _disposables = new();
    private AttachmentsTab _currentTab = AttachmentsTab.None;

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
        CustomizationIndexes customizationIndexes = null;

        if (charContainer != null)
        {
            customizationIndexes = charContainer.CurrentCustomizationData.CustomizationIndexes;
        }

        backButton.OnClickAsObservable().Subscribe(_ => { _backClickHandler.OnNext(_key); }).AddTo(_disposables);
        await podiumController.Initialize(_productVisuals, customizationIndexes);
        SubscribeRawImage();
        SwitchTab(AttachmentsTab.Scope);
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
        }

        foreach (var contentItem in content)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            var weaponInventoryVisuals = _shopProductConfig.ShopProductVisual.FirstOrDefault(g => g.Hash.Equals(contentItem.Value));
            cell.Item.SetText(weaponInventoryVisuals.ProductName)
                //.SetOnClickHandler(weapon, _cellClickHandler)
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
