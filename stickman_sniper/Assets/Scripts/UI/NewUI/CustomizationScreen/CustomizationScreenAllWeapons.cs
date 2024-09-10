using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Customization;
using Sirenix.OdinInspector;
using stickman_sniper.Purchases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class CustomizationScreenAllWeapons : MonoBehaviour
{
    [SerializeField, BoxGroup("Tabs")] private Transform tabsContainer;
    [SerializeField, BoxGroup("Tabs")] private CustomizationScreenAllWeaponsTab tabPrefab;

    [SerializeField, BoxGroup("Cells")] private Transform weaponsContainer;

    [SerializeField] private CustomizationScreemShopCellPool shopCellPool;

    private CustomiationDataContainerSO _customiationDataContainerSO;
    private ShopPresentationConfig _shopPresentationConfig;
    private IPurchaseService _purchaseService;
    private IGameStartWeaponInventoryService _gameStartWeaponInventoryService;

    private List<CustomizationScreenAllWeaponsTab> _tabs = new();
    private List<IPooledItem<CustomizationScreenShopCell>> _shopCells = new();
    private Dictionary<InventoryTab, List<string>> _tagWeaponLink;
    private Subject<InventoryTab> _tabClickHandler = new();
    private IObserver<string> _cellClickHandler;
    private Subject<int> _cellClickHandlerInt = new();
    private InventoryTab _currentSelectedTab;
    private CompositeDisposable _disposables = new();

    public void ResolveDependencies(
        CustomiationDataContainerSO customiationDataContainerSO,
        ShopPresentationConfig shopPresentationConfig,
        IGameStartWeaponInventoryService gameStartWeaponInventoryService,
        IPurchaseService purchaseService)
    {
        _customiationDataContainerSO = customiationDataContainerSO;
        _shopPresentationConfig = shopPresentationConfig;
        _purchaseService = purchaseService;
        _gameStartWeaponInventoryService = gameStartWeaponInventoryService;
    }

    public async UniTask Init(IObserver<string> cellClickHandler)
    {
        _tagWeaponLink = new();
        
        foreach (var item in _shopPresentationConfig.ShopPresentationItems)
        {
            //var shopVisualInfo = _shopPresentationConfig.ShopPresentationItems.FirstOrDefault(g => g.TagName.Equals(item.Tag));
            var availableWeapons = item.Weapons;
        
            _tagWeaponLink.Add(item.Tab, availableWeapons.ToList());
        
            var newTab = Instantiate(tabPrefab, tabsContainer);
            _tabs.Add(newTab);
        
            newTab.SetTabName(item.TabName)
                .SetOnClickHandler(item.Tab, _tabClickHandler);
        }
        
        _cellClickHandler = cellClickHandler;
        
        if (_tagWeaponLink.Count > 0)
        {
            _currentSelectedTab = _tagWeaponLink.First().Key;
            await ShowTab(_currentSelectedTab);
        }
        
        _tabClickHandler.Subscribe(async x =>
        {
            _currentSelectedTab = x;
            _shopCells.ForEach(g => g.ReturnToPool());
            await ShowTab(_currentSelectedTab);
        }).AddTo(_disposables);

        _cellClickHandlerInt.Subscribe(x =>
        {
            _tagWeaponLink.TryGetValue(_currentSelectedTab, out var weapons);
            _cellClickHandler.OnNext(weapons[x]);
        }).AddTo(_disposables);
    }

    public async UniTask DeInit()
    {
        _tabs.ForEach(g => Destroy(g.gameObject));
        _tabs.Clear();

        _shopCells.ForEach(g => g.ReturnToPool());

        _tagWeaponLink.Clear();
        _disposables.Clear();
    }

    private async UniTask ShowTab(InventoryTab tab)
    {
        if (!_tagWeaponLink.TryGetValue(tab, out var weapons))
            return;

        for (int i =0; i< weapons.Count; i++)
        {
            IPooledItem<CustomizationScreenShopCell> cell = shopCellPool.Get();
            _shopCells.Add(cell);
            
            var weaponInventoryVisuals = _shopPresentationConfig.GetConfigByKey(weapons[i]);
            var weaponItem = weaponInventoryVisuals.GetItemByProductKey(weapons[i]);

            if (weaponItem.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponItem.Hash);
            }

            cell.Item.ResolveDependencies();
            cell.Item.Init(weaponItem);
            cell.Item.SetState(GetCellState(cell.Item));

            cell.Item.SetOnClickHandler(i, _cellClickHandlerInt);
            cell.Item.transform.SetParent(weaponsContainer, false);
            cell.Item.gameObject.SetActive(true);
        }
    }

    private CellState GetCellState(CustomizationScreenShopCell cell)
    {
        CellState result = CellState.None;

        var purchasedProprerty = _purchaseService.GetIsPurchasedReactiveProperty(cell.Visual.Hash);
        bool isPurchased = purchasedProprerty.Value;
        bool isSelected = _gameStartWeaponInventoryService.MainWeapon.Equals(cell.Visual.ProductKey);

        if (isSelected)
        {
            result = CellState.Chosen;
        }
        else if (isPurchased)
        {
            result = CellState.Purchased;
        }
        else result = CellState.Available;

        return result;
    }
}
