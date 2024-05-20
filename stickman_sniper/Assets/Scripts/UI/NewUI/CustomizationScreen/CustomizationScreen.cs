using Cysharp.Threading.Tasks;
using DWTools.Customization;
using DWTools.Windows;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

public class CustomizationScreen : BaseWindow
{
    [SerializeField, BoxGroup("Tabs")] private Transform tabsContainer;
    [SerializeField, BoxGroup("Tabs")] private CustomizationScreenTab tabPrefab;

    [SerializeField, BoxGroup("Cells")] private Transform weaponsContainer;

    [SerializeField] private CustomizationScreemShopCellPool shopCellPool;

    private GameWeaponConfig _gameWeaponConfig;
    private AvailableWeaponConfig _availableWeaponConfig;
    private CustomiationDataContainerSO _customiationDataContainerSO;
    private ShopPresentationConfig _shopPresentationConfig;

    private List<CustomizationScreenTab> _tabs = new();
    private List<CustomizationScreenShopCell> _shopCells = new();
    private Dictionary<string, List<string>> _tagWeaponLink = new();
    private Subject<string> _tabClickHandler = new();
    private Subject<string> _cellClickHandler = new();
    private string _currentSelectedTab;
    private CompositeDisposable _disposables = new();

    [Inject]
    private void Construct(GameWeaponConfig gameWeaponConfig,
        AvailableWeaponConfig availableWeaponConfig,
        CustomiationDataContainerSO customiationDataContainerSO,
        ShopPresentationConfig shopPresentationConfig)
    {
        _gameWeaponConfig = gameWeaponConfig;
        _availableWeaponConfig = availableWeaponConfig;
        _customiationDataContainerSO = customiationDataContainerSO;
        _shopPresentationConfig = shopPresentationConfig;
    }

    protected override async UniTask BeforeShow(CancellationToken token)
    {
        foreach (var item in _gameWeaponConfig.WeaponsConfig)
        {
            var shopVisualInfo = _shopPresentationConfig.ShopPresentationItems.FirstOrDefault(g => g.TagName.Equals(item.Tag));
            var availableWeapons = item.Weapons.Where(g => _availableWeaponConfig.AvailableWeapons.Any(h => h.Equals(g.Name)));

            _tagWeaponLink.Add(item.Tag, availableWeapons.Select(g => g.Name).ToList());

            var newTab = Instantiate(tabPrefab, tabsContainer);

            newTab.SetTabName(shopVisualInfo.TabName)
                .SetOnClickHandler(item.Tag, _tabClickHandler);
        }

        _currentSelectedTab = "auto_rifles";
        await ShowTab(_currentSelectedTab);

        await base.BeforeShow(token);
    }

    protected override async UniTask AfterShow(CancellationToken token)
    {
        _tabClickHandler.Subscribe(async x =>
        {
            _currentSelectedTab = x;
            _shopCells.ForEach(g => g.ReturnToPool());
            await ShowTab(_currentSelectedTab);
        }).AddTo(_disposables);

        await base.AfterShow(token);
    }

    protected override async UniTask BeforeHide(CancellationToken token)
    {
        _tabs.ForEach(g => Destroy(g.gameObject));
        _tabs.Clear();

        _tagWeaponLink.Clear();

        await base.BeforeHide(token);
    }

    private async UniTask ShowTab(string tabTag)
    {
        if (!_tagWeaponLink.TryGetValue(tabTag, out var weapons))
            return;

        foreach(var weapon in weapons)
        {
            var cell = shopCellPool.Get();
            var weaponInventoryVisuals = _shopPresentationConfig.ShopProductVisual.FirstOrDefault(g => g.ProductKey.Equals(weapon));
            cell.Item.SetText(weaponInventoryVisuals.ProductName)
                .SetOnClickHandler(weapon, _cellClickHandler)
                .ContinueWith(async g =>
                {
                    if (weaponInventoryVisuals.ProductBackground.IsValid())
                    {
                        g.SetBackground(await weaponInventoryVisuals.ProductBackground.InstantiateAsync());
                    }
                })
                .ContinueWith(async g =>
                {
                    if (weaponInventoryVisuals.ProductImage.IsValid())
                    {
                        g.SetBackground(await weaponInventoryVisuals.ProductImage.InstantiateAsync());
                    }
                });
            cell.Item.transform.SetParent(weaponsContainer);
            cell.Item.gameObject.SetActive(true);
        }
    }
}
