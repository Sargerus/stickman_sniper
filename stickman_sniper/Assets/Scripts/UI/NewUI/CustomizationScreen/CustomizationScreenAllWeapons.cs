using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Customization;
using Purchase;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class CustomizationScreenAllWeapons : MonoBehaviour
{
    [SerializeField, BoxGroup("Tabs")] private Transform tabsContainer;
    [SerializeField, BoxGroup("Tabs")] private CustomizationScreenTab tabPrefab;

    [SerializeField, BoxGroup("Cells")] private Transform weaponsContainer;

    [SerializeField] private CustomizationScreemShopCellPool shopCellPool;

    private AvailableWeaponConfig _availableWeaponConfig;
    private ShopPresentationConfig _shopPresentationConfig;
    private IPurchaseService _purchaseService;
    private IGameStartWeaponInventoryService _gameStartWeaponInventoryService;

    private List<CustomizationScreenTab> _tabs = new();
    private List<IPooledItem<CustomizationScreenShopCell>> _shopCells = new();
    private Dictionary<string, List<string>> _tagWeaponLink;
    private Subject<string> _tabClickHandler = new();
    private IObserver<string> _cellClickHandler;
    private string _currentSelectedTab;
    private CompositeDisposable _disposables = new();

    public void ResolveDependencies(
        AvailableWeaponConfig availableWeaponConfig,
        ShopPresentationConfig shopPresentationConfig,
        IGameStartWeaponInventoryService gameStartWeaponInventoryService,
        IPurchaseService purchaseService)
    {
        _availableWeaponConfig = availableWeaponConfig;
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
            var availableWeapons = item.Weapons.Where(g => _availableWeaponConfig.AvailableWeapons.Any(h => h.Equals(g)));
        
            _tagWeaponLink.Add(item.TagName, availableWeapons.ToList());
        
            var newTab = Instantiate(tabPrefab, tabsContainer);
            _tabs.Add(newTab);
        
            newTab.SetTabName(item.TabName)
                .SetOnClickHandler(item.TagName, _tabClickHandler);
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
    }

    public async UniTask DeInit()
    {
        _tabs.ForEach(g => Destroy(g.gameObject));
        _tabs.Clear();

        _shopCells.ForEach(g => g.ReturnToPool());

        _tagWeaponLink.Clear();
        _disposables.Clear();
    }

    private async UniTask ShowTab(string tabTag)
    {
        if (!_tagWeaponLink.TryGetValue(tabTag, out var weapons))
            return;

        foreach (var weapon in weapons)
        {
            IPooledItem<CustomizationScreenShopCell> cell = shopCellPool.Get();
            _shopCells.Add(cell);
            
            var weaponInventoryVisuals = _shopPresentationConfig.GetConfigByKey(weapon);
            var weaponItem = weaponInventoryVisuals.GetItemByProductKey(weapon);

            if (weaponItem.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponItem.Hash);
            }

            cell.Item.ResolveDependencies();
            cell.Item.Init(weaponItem, null);
            cell.Item.SetState(GetCellState(cell.Item));

            cell.Item.SetOnClickHandler(weapon, _cellClickHandler);
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
            result = CellState.Selected;
        }
        else if (isPurchased)
        {
            result = CellState.Purchased;
        }
        else result = CellState.Available;

        return result;
    }
}
