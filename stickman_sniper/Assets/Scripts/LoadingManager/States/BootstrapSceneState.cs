using Cysharp.Threading.Tasks;
using DW.StateMachine;
using stickman_sniper.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public sealed class BootstrapSceneState : IState
{
    public IGlobalBlackboard GlobalBlackboard { get; set; }
    private SceneContext _sceneContext;

    private readonly LoadingManager _loadingManager;
    private readonly WeaponCharacteristicsContainer _weaponCharacteristicsContainer;

    public BootstrapSceneState(LoadingManager loadingManager, WeaponCharacteristicsContainer weaponCharacteristicsContainer)
    {
        _loadingManager = loadingManager;
        _weaponCharacteristicsContainer = weaponCharacteristicsContainer;
    }

    public async UniTask OnEnterState()
    {
        var gameObject = GameObject.FindWithTag(LoadingManager.SceneContextTag);
        if (gameObject == null)
        {
            await OnExitState();
            return;
        }

        _sceneContext = gameObject.GetComponent<SceneContext>();
        _sceneContext.Run();

        var loadingManagerHolder = _sceneContext.Container.Resolve<ILoadingManagerHolder>();
        loadingManagerHolder.LoadingManager = _loadingManager;

        InitializeCurrency();
        
        var saveService = _sceneContext.Container.Resolve<ISaveService>();
        InitializeWeaponIndexes(saveService);

        GlobalBlackboard.Blackboard.SetValue<bool>(BlackboardConstants.BootstrapReadyBool, true);
    }

    private void InitializeCurrency()
    {
        var currencyService = _sceneContext.Container.Resolve<ICurrencyService>();

        currencyService.CreateCurrency("gold");
    }

    private void InitializeWeaponIndexes(ISaveService saveService)
    {
        List<WeaponIndexes> weaponIndexes = saveService.GetWeaponIndexes();

        foreach (var weapon in _weaponCharacteristicsContainer.Config)
        {
            if (weaponIndexes.Any(g => g.WeaponKey.Equals(weapon.WeaponKey)))
            {
                var indexes = weaponIndexes.FirstOrDefault(g => g.WeaponKey.Equals(weapon.WeaponKey));
                weapon.CurrentCustomizationData.CustomizationIndexes = new(indexes.Indexes);
                continue;
            }

            weaponIndexes.Add(new WeaponIndexes(weapon.WeaponKey, weapon.DefaultCustomizationData.CustomizationIndexes));
        }

        saveService.SetWeaponIndexes(weaponIndexes);
        saveService.SaveProgress();
    }

    public async UniTask OnExitState()
    {
    }

    public void Tick(float elapsedTime)
    {
    }

    [Serializable]
    public class WeaponIndexes
    {
        public string WeaponKey;
        public CustomizationIndexes Indexes;

        public WeaponIndexes(string weaponKey, CustomizationIndexes indexes)
        {
            WeaponKey = weaponKey;
            Indexes = new(indexes);
        }
    }
}