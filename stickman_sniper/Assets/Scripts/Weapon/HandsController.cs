using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace DWTools
{
    public interface IHandsController : IWeapon
    {
        UniTask SwitchWeapon(IWeapon weapon);
        Transform WeaponContainer { get; }
    }

    public class HandsController : MonoBehaviour, IHandsController
    {
        [SerializeField] private Transform _weaponContainer;
        public Transform WeaponContainer => _weaponContainer;

        private IWeaponAnimationInterface _weaponAnimation;
        private IWeaponAnimationInterface _handsAnimation;

        private IWeapon _weapon;
        private IWeaponStateController _weaponStateController;
        private IHands _hands;

        private CompositeDisposable _weaponDisposables = new();
        private CancellationTokenSource _shootCancellationToken;
        private CancellationTokenSource _switchWeaponCancellationToken;
        private CancellationTokenSource _reloadWeaponCancellationToken;
        private CancellationTokenSource _grabWeaponCancellationToken;

        #region Weapon
        public int BulletType => _weapon.BulletType;
        public IReadOnlyReactiveProperty<int> CurrentBulletsCount => _weapon.CurrentBulletsCount;
        public int MaxBulletsCount => _weapon.MaxBulletsCount;
        public IReadOnlyReactiveProperty<bool> CanShoot => _weapon.CanShoot;
        public IReadOnlyReactiveProperty<bool> IsReloading => _weapon.IsReloading;
        public IReadOnlyReactiveProperty<bool> IsGrabing => _weapon.IsGrabing;        

        public void Reload()
        {
            if (_weapon.CurrentBulletsCount.Value == _weapon.MaxBulletsCount ||
                IsReloading.Value)
                return;

            _weaponStateController.SetIsReloading(true);
            _weapon.Reload();
            ReloadInternal().Forget();
        }

        private async UniTask ReloadInternal()
        {
            await UpdateToken(_reloadWeaponCancellationToken);
            _reloadWeaponCancellationToken = new();

            await UniTask.WhenAll(
                _weaponAnimation != null ? _weaponAnimation.Reload(_reloadWeaponCancellationToken.Token) : UniTask.CompletedTask,
                _handsAnimation != null ? _handsAnimation.Reload(_reloadWeaponCancellationToken.Token) : UniTask.CompletedTask);

            _weaponStateController.SetIsReloading(false);
        }

        public void Shoot()
        {
            if (IsReloading.Value)
                return;

            _weaponStateController.SetIsShooting(true);
            _weapon.Shoot();
            ShootInternal().Forget();
        }

        private async UniTask ShootInternal()
        {
            await UpdateToken(_shootCancellationToken);
            _shootCancellationToken = new();

            await UniTask.WhenAll(
                _weaponAnimation != null ? _weaponAnimation.Shoot(_shootCancellationToken.Token) : UniTask.CompletedTask,
                _handsAnimation != null ? _handsAnimation.Shoot(_shootCancellationToken.Token) : UniTask.CompletedTask);

            _weaponStateController.SetIsShooting(false);
        }

        public void Grab()
        {
            if (IsGrabing.Value)
                return;

            _weaponStateController.SetIsGrabing(true);
            _weapon.Grab();
            GrabInternal().Forget();
        }

        private async UniTask GrabInternal()
        {
            await UpdateToken(_grabWeaponCancellationToken);
            _grabWeaponCancellationToken = new();
            
            await UniTask.WhenAll(
                _weaponAnimation != null ? _weaponAnimation.Grab(_grabWeaponCancellationToken.Token) : UniTask.CompletedTask,
                _handsAnimation != null ? _handsAnimation.Grab(_grabWeaponCancellationToken.Token) : UniTask.CompletedTask );

            _weaponStateController.SetIsGrabing(false);
        }

        private async UniTask UpdateToken(CancellationTokenSource cts)
        {
            cts?.Cancel();
            await UniTask.Yield();
        }
        #endregion

        public async UniTask SwitchWeapon(IWeapon weapon)
        {
            if (_weapon == weapon)
                return;

            await UpdateToken(_switchWeaponCancellationToken);
            _switchWeaponCancellationToken = new();

            _weapon = weapon;
            _weaponDisposables?.Clear();

            Grab();

            if (_weapon.CurrentBulletsCount.Value == 0)
            {
                Reload();
            }
        }
    }
}