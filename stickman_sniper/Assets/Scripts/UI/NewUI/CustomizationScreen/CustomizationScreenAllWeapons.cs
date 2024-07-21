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
    [SerializeField, BoxGroup("Tabs")] private CustomizationScreenTab tabPrefab;

    [SerializeField, BoxGroup("Cells")] private Transform weaponsContainer;

    [SerializeField] private CustomizationScreemShopCellPool shopCellPool;

    private AvailableWeaponConfig _availableWeaponConfig;
    private CustomiationDataContainerSO _customiationDataContainerSO;
    private ShopPresentationConfig _shopPresentationConfig;
    private IPurchaseService _purchaseService;

    private List<CustomizationScreenTab> _tabs = new();
    private List<IPooledItem<CustomizationScreenShopCell>> _shopCells = new();
    private Dictionary<string, List<string>> _tagWeaponLink;
    private Subject<string> _tabClickHandler = new();
    private IObserver<string> _cellClickHandler;
    private string _currentSelectedTab;
    private CompositeDisposable _disposables = new();

    public void ResolveDependencies(
        AvailableWeaponConfig availableWeaponConfig,
        CustomiationDataContainerSO customiationDataContainerSO,
        ShopPresentationConfig shopPresentationConfig,
        IPurchaseService purchaseService)
    {
        _availableWeaponConfig = availableWeaponConfig;
        _customiationDataContainerSO = customiationDataContainerSO;
        _shopPresentationConfig = shopPresentationConfig;
        _purchaseService = purchaseService;
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

            if (weaponInventoryVisuals.IsBoughtByDefault)
            {
                _purchaseService.Purchase(weaponInventoryVisuals.Hash);
            }

            cell.Item.ResolveDependencies(_purchaseService);
            cell.Item.Init(weaponInventoryVisuals);

            cell.Item.SetOnClickHandler(weapon, _cellClickHandler);

            cell.Item.transform.SetParent(weaponsContainer, false);
            cell.Item.gameObject.SetActive(true);
        }
    }
}
