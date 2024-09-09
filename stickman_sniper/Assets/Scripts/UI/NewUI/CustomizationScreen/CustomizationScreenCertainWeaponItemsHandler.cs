using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using Sirenix.OdinInspector;
using stickman_sniper.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using YG;

public class CustomizationScreenCertainWeaponItemsHandler : MonoBehaviour
{
    [SerializeField, BoxGroup("Context")] private Transform container;
    [SerializeField, BoxGroup("Context")] private CustomizationScreenShopCell cellPrefab;

    private List<CustomizationScreenShopCell> _cells = new();
    private Subject<string> _cellClickHandler = new();
    private CompositeDisposable _disposables = new();
    private Dictionary<int, string> _currentAttachments;
    private string _currentChosen;
    private AttachmentsTab _currentTabLocal = AttachmentsTab.None;

    private IReadOnlyReactiveProperty<AttachmentsTab> _currentTab;
    private IReactiveProperty<string> _selectedItem;
    private IReadOnlyReactiveProperty<string> _chosenItem;
    private IAttachmentManager _attachmentManager;
    private ShopProductVisuals _productVisualsContainer;
    private IPurchaseService _purchaseService;
    private CustomizationIndexes _customizationIndexes;

    public async UniTask Initialize(IReadOnlyReactiveProperty<AttachmentsTab> currentTab,
        IReactiveProperty<string> selectedItem,
        IReadOnlyReactiveProperty<string> chosenItem,
        IAttachmentManager attachmentManager,
        ShopProductVisuals productVisualsContainer,
        IPurchaseService purchaseService,
        CustomizationIndexes customizationIndexes)
    {
        _currentTab = currentTab;
        _selectedItem = selectedItem;
        _chosenItem = chosenItem;
        _attachmentManager = attachmentManager;
        _productVisualsContainer = productVisualsContainer;
        _purchaseService = purchaseService;
        _customizationIndexes = customizationIndexes;

        _currentTab.Subscribe(SwitchTab).AddTo(this);
    }

    public void SwitchTab(AttachmentsTab tab)
    {
        if (tab == _currentTabLocal)
            return;

        Clear();
        _currentChosen = string.Empty;
        _currentTabLocal = tab;

        _currentAttachments = _attachmentManager.GetAttachments(tab);

        int selectedIndex = _customizationIndexes.GetIndex(tab);
        if (selectedIndex >= 0)
            _selectedItem.Value = _productVisualsContainer.GetItemByHash(_currentAttachments[selectedIndex]).ProductKey;

        for (int i = 0; i < _currentAttachments.Count; i++)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            ShopProductVisual weaponInventoryVisuals = _productVisualsContainer.GetItemByHash(_currentAttachments[i]);

            if (weaponInventoryVisuals.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponInventoryVisuals.Hash);
            }

            InitCellState(cell, weaponInventoryVisuals);
        }

        Subscribe();
    }

    private void InitCellState(CustomizationScreenShopCell cell, ShopProductVisual weaponInventoryVisuals)
    {
        cell.Item.ResolveDependencies();
        cell.Item.Init(weaponInventoryVisuals);
        cell.Item.SetOnClickHandler(weaponInventoryVisuals.ProductKey, _cellClickHandler);

        CellState state = GetCellState(cell);
        cell.SetState(state);

        var purchasedProprerty = _purchaseService.GetIsPurchasedReactiveProperty(weaponInventoryVisuals.Hash);
        if (!purchasedProprerty.Value)
        {
            purchasedProprerty.Where(x => x).SubscribeWithState(cell, (_, cell) => cell.Item.SetState(GetCellState(cell))).AddTo(_disposables);
        }
    }

    private void Subscribe()
    {
        _cellClickHandler.Subscribe(x =>
        {
            var cell = _cells.FirstOrDefault(g => g.Visual.ProductKey.Equals(x));

            if (cell == null)
                return;

            if (!string.IsNullOrEmpty(_selectedItem.Value))
            {
                var oldCell = _cells.FirstOrDefault(g => g.Visual.ProductKey.Equals(_selectedItem.Value));
                oldCell.Swaper.SetSelected(false);
            }

            cell.Swaper.SetSelected(true);
            _selectedItem.Value = x;
        }).AddTo(_disposables);

        _chosenItem.SkipLatestValueOnSubscribe().Subscribe(x =>
        {
            if (!string.IsNullOrEmpty(_currentChosen))
            {
                if (_currentChosen.Equals(x))
                    return;

                var oldCell = _cells.FirstOrDefault(g => g.Visual.ProductKey.Equals(_currentChosen));
                oldCell.SetState(CellState.Purchased);
            }

            var newCell = _cells.FirstOrDefault(g => g.Visual.ProductKey.Equals(x));
            if (newCell == null)
                return;

            newCell.SetState(CellState.Chosen);
            _currentChosen = x;
        }).AddTo(_disposables);
    }

    private CellState GetCellState(CustomizationScreenShopCell cell)
    {
        CellState result = CellState.Available;

        var purchasedProprerty = _purchaseService.GetIsPurchasedReactiveProperty(cell.Visual.Hash);
        bool isPurchased = purchasedProprerty.Value;
        bool isChosen = _chosenItem.Value == cell.Visual.ProductKey;

        if (isChosen)
        {
            result = CellState.Chosen;
        }
        else if (isPurchased)
        {
            result = CellState.Purchased;
        }

        return result;
    }

    private void Clear()
    {
        _cells.ForEach(g => Destroy(g.gameObject));
        _cells.Clear();

        _attachmentManager.SetAttachmentIndex(_currentTabLocal, _customizationIndexes.GetIndex(_currentTabLocal));
        //_selectedItem.Value = string.Empty;
        _disposables?.Clear();
    }
}
