using Cysharp.Threading.Tasks;
using DWTools.Customization;
using DWTools.Windows;
using Sirenix.OdinInspector;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

public class CustomizationScreen : BaseWindow
{
    [SerializeField, BoxGroup("Levels of ui")] private CustomizationScreenAllWeapons weaponsGrid;
    [SerializeField, BoxGroup("Levels of ui")] private CustomizationScreenCertainWeapon certainWeapon;
    
    private Subject<string> _cellClickHandler = new();
    private Subject<string> _backClickHandler = new();
    private Tabs _currentTab = Tabs.None;
    private CompositeDisposable _disposables = new();

    enum Tabs
    {
        None = 0,
        AllWeapons = 1,
        CertainWeapon = 2
    }

    [Inject]
    private void Construct(
        AvailableWeaponConfig availableWeaponConfig,
        CustomiationDataContainerSO customiationDataContainerSO,
        ShopPresentationConfig shopPresentationConfig,
        WeaponCharacteristicsContainer weaponCharacteristicsContainer)
    {
        weaponsGrid.ResolveDependencies(availableWeaponConfig, customiationDataContainerSO, shopPresentationConfig);
        certainWeapon.ResolveDependencies(shopPresentationConfig, weaponCharacteristicsContainer);

        SwitchTabAsync(null, Tabs.AllWeapons).Forget();
    }

    protected override async UniTask BeforeShow(CancellationToken token)
    {        
        await base.BeforeShow(token);
    }

    private async UniTask SwitchTabAsync(string weaponKey, Tabs tab)
    {
        switch (tab)
        {
            case Tabs.None:
            case Tabs.AllWeapons:
                {
                    await weaponsGrid.Init(_cellClickHandler);
                    weaponsGrid.gameObject.SetActive(true);

                    await certainWeapon.DeInit();
                    certainWeapon.gameObject.SetActive(false);

                    break;
                }

            case Tabs.CertainWeapon:
                {
                    await weaponsGrid.DeInit();
                    weaponsGrid.gameObject.SetActive(false);

                    await certainWeapon.Init(weaponKey, _backClickHandler);
                    certainWeapon.gameObject.SetActive(true);

                    break;
                }
        }

        _currentTab = tab;
    }

    protected override async UniTask AfterShow(CancellationToken token)
    {
        _cellClickHandler.Subscribe(async g =>
        {
            await SwitchTabAsync(g, Tabs.CertainWeapon);
        }).AddTo(_disposables);

        _backClickHandler.Subscribe(_ =>
        {
            switch (_currentTab)
            {
                case Tabs.CertainWeapon: SwitchTabAsync(_, Tabs.AllWeapons); return;
            }
        }).AddTo(_disposables);

        await base.AfterShow(token);
    }

    protected override async UniTask BeforeHide(CancellationToken token)
    {
        await base.BeforeHide(token);
    }
}
