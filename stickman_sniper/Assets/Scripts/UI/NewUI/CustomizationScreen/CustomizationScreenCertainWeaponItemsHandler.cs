using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using Sirenix.OdinInspector;
using stickman_sniper.Purchases;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class CustomizationScreenCertainWeaponItemsHandler : MonoBehaviour
{
    [SerializeField, BoxGroup("Context")] private Transform container;
    [SerializeField, BoxGroup("Context")] private CustomizationScreenShopCell cellPrefab;

    private List<CustomizationScreenShopCell> _cells = new();
    private Subject<int> _cellClickHandler = new();
    private CompositeDisposable _disposables = new();
    private Dictionary<int, string> _currentAttachments;
    private int _currentChosen;
    private AttachmentsTab _currentTabLocal = AttachmentsTab.None;
    private IReactiveProperty<int> _currentSelected => _selectedItem[_currentTab.Value];

    private IReadOnlyReactiveProperty<AttachmentsTab> _currentTab;
    private Dictionary<AttachmentsTab, ReactiveProperty<int>> _selectedItem;
    private IReadOnlyReactiveProperty<int> _chosenItem;
    private IAttachmentManager _attachmentManager;
    private ShopProductVisuals _productVisualsContainer;
    private IPurchaseService _purchaseService;
    private CustomizationIndexes _customizationIndexes;

    public async UniTask Initialize(IReadOnlyReactiveProperty<AttachmentsTab> currentTab,
        Dictionary<AttachmentsTab, ReactiveProperty<int>> selectedItem,
        IReadOnlyReactiveProperty<int> chosenItem,
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

        _currentTab.SkipLatestValueOnSubscribe().Subscribe(SwitchTab).AddTo(this);
    }

    public void SwitchTab(AttachmentsTab tab)
    {
        if (tab == _currentTabLocal)
            return;

        _currentChosen = -1;
        _currentTabLocal = tab;
        _currentAttachments = _attachmentManager.GetAttachments(tab);

        int selectedIndex = _customizationIndexes.GetIndex(tab);
        _currentSelected.Value = selectedIndex;

        for (int i = 0; i < _currentAttachments.Count; i++)
        {
            CustomizationScreenShopCell cell = Instantiate(cellPrefab, container);
            _cells.Add(cell);

            ShopProductVisual weaponInventoryVisuals = _productVisualsContainer.GetItemByHash(_currentAttachments[i]);

            if (weaponInventoryVisuals.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponInventoryVisuals.Hash);
            }

            InitCellState(cell, weaponInventoryVisuals, i);
        }

        Subscribe();
    }

    private void InitCellState(CustomizationScreenShopCell cell, ShopProductVisual weaponInventoryVisuals, int index)
    {
        cell.Item.ResolveDependencies();
        cell.Item.Init(weaponInventoryVisuals);
        cell.Item.SetOnClickHandler(index, _cellClickHandler);

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
            var cell = _cells[x];

            if (cell == null)
                return;

            if (_currentSelected.Value >= 0)
            {
                var oldCell = _cells[_currentSelected.Value];
                oldCell.Swaper.SetSelected(false);
            }

            cell.Swaper.SetSelected(true);
            _currentSelected.Value = x;
        }).AddTo(_disposables);

        _chosenItem.SkipLatestValueOnSubscribe().Subscribe(x =>
        {
            if (_currentChosen >= 0)
            {
                if (_currentChosen == x)
                    return;

                DeselectCurrentSelected();
            }

            if(x < 0)
            {
                _currentChosen = -1;
                return;
            }

            var newCell = _cells[x];
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
        bool isChosen = _currentChosen == _cells.IndexOf(cell);

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

    private void DeselectCurrentSelected()
    {
        if (_currentChosen >= 0 && _cells is { Count: > 0})
        {
            var oldCell = _cells[_currentChosen];
            oldCell.SetState(CellState.Purchased);
        }
    }

    public void ClearTab()
    {
        DeselectCurrentSelected();

        _cells.ForEach(g => Destroy(g.gameObject));
        _cells.Clear();
        _currentSelected.Value = _customizationIndexes.GetIndex(_currentTab.Value);
        _disposables?.Clear();
    }
}
