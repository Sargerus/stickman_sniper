using Cysharp.Threading.Tasks;
using DWTools.Customization;
using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using Zenject;

namespace DWTools
{
    public interface IHandsController
    {
        Transform WeaponContainer { get; }
    }

    public class HandsController : MonoBehaviour, IHandsController, IInitializable
    {
        [SerializeField] private Transform _weaponContainer;
        public Transform WeaponContainer => _weaponContainer;

        [Inject] private IWeaponService _weaponService;
        [Inject] private DiContainer _diContainer;

        private GameObject _currentWeaponView;

        private IAnimationInterface _weaponAnimation;
        private IAnimationInterface _handsAnimation;

        private CompositeDisposable _weaponDisposables = new();
        private CancellationTokenSource _shootCancellationToken;
        private CancellationTokenSource _switchWeaponCancellationToken;
        private CancellationTokenSource _reloadWeaponCancellationToken;
        private CancellationTokenSource _grabWeaponCancellationToken;

        public void Initialize()
        {
            _weaponService.CurrentWeapon.Subscribe(async x =>
            {
                x.View = _currentWeaponView = Instantiate(x.Prefab, _weaponContainer);

                //customize
                _currentWeaponView.gameObject.SetActive(false);
                await x.Customize(x.View.GetComponentInChildren<CustomizableEntityProvider>());
                _currentWeaponView.gameObject.SetActive(true);

                _diContainer.InjectGameObject(x.View);
                _weaponAnimation = _currentWeaponView.GetComponent<IAnimationInterface>();
            }).AddTo(_weaponDisposables);
        }

        #region Weapon
        public void Reload()
        {
           //if (_weapon.CurrentBulletsCount.Value == _weapon.MaxBulletsCount ||
           //    IsReloading.Value)
           //    return;
           //
           //_weaponStateController.SetIsReloading(true);
           //_weapon.Reload();
           //ReloadInternal().Forget();
        }

        private async UniTask ReloadInternal()
        {
           //await UpdateToken(_reloadWeaponCancellationToken);
           //_reloadWeaponCancellationToken = new();
           //
           //await UniTask.WhenAll(
           //    _weaponAnimation != null ? _weaponAnimation.Reload(_reloadWeaponCancellationToken.Token) : UniTask.CompletedTask,
           //    _handsAnimation != null ? _handsAnimation.Reload(_reloadWeaponCancellationToken.Token) : UniTask.CompletedTask);
           //
           //_weaponStateController.SetIsReloading(false);
        }

        public void Shoot()
        {
           //if (CanShoot.Value)
           //    return;
           //
           //_weaponStateController.SetIsShooting(true);
           //_weapon.Shoot();
           //ShootInternal().Forget();
        }

        private async UniTask ShootInternal()
        {
           // await UpdateToken(_shootCancellationToken);
           // _shootCancellationToken = new();
           //
           // await UniTask.WhenAny(
           //     UniTask.WhenAll(_weaponAnimation != null ? _weaponAnimation.Shoot(_shootCancellationToken.Token) : UniTask.CompletedTask,
           //                     _handsAnimation != null ? _handsAnimation.Shoot(_shootCancellationToken.Token) : UniTask.CompletedTask),
           //     UniTask.Delay(_weapon.TimeBetweenShots));
           //
           // _weaponStateController.SetIsShooting(false);
        }

        public void Grab()
        {
            //if (IsGrabing.Value)
            //    return;
            //
            //_weaponStateController.SetIsGrabing(true);
            //_weapon.Grab();
            //GrabInternal().Forget();
        }

        private async UniTask GrabInternal()
        {
            //await UpdateToken(_grabWeaponCancellationToken);
            //_grabWeaponCancellationToken = new();
            //
            //await UniTask.WhenAll(
            //    _weaponAnimation != null ? _weaponAnimation.Grab(_grabWeaponCancellationToken.Token) : UniTask.CompletedTask,
            //    _handsAnimation != null ? _handsAnimation.Grab(_grabWeaponCancellationToken.Token) : UniTask.CompletedTask );
            //
            //_weaponStateController.SetIsGrabing(false);
        }

        private async UniTask UpdateToken(CancellationTokenSource cts)
        {
            cts?.Cancel();
            await UniTask.Yield();
        }
        #endregion

        //public async UniTask SwitchWeapon(IWeapon weapon, IWeaponStateController weaponStateController)
        //{
        //    if (_weapon == weapon)
        //        return;
        //
        //    await UpdateToken(_switchWeaponCancellationToken);
        //    _switchWeaponCancellationToken = new();
        //
        //    _weapon = weapon;
        //    _weaponStateController = weaponStateController;
        //    _weaponDisposables?.Clear();
        //
        //    Grab();
        //
        //    if (_weapon.CurrentBulletsCount.Value == 0)
        //    {
        //        Reload();
        //    }
        //}
    }
}