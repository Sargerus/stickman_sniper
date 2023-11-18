using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace DWTools
{
    public interface IHandsController
    {
        UniTask SwitchWeapon(IWeapon weapon);
    }

    public class HandsController : MonoBehaviour, IHandsController, IWeapon
    {
        private IWeapon _weapon;
        private CompositeDisposable _weaponDisposables = new();
        private CancellationTokenSource _shootCancellationToken;
        private CancellationTokenSource _switchWeaponCancellationToken;
        private CancellationTokenSource _reloadWeaponCancellationToken;

        #region Weapon
        public int BulletType => _weapon.BulletType;

        public ReactiveProperty<int> CurrentBulletsCount => _weapon.CurrentBulletsCount;

        public int MaxBulletsCount => _weapon.MaxBulletsCount;

        public ReactiveProperty<bool> CanShoot => _weapon.CanShoot;

        public ReactiveProperty<bool> IsReloading => _weapon.IsReloading;

        public async UniTask Reload()
        {
            if (_weapon.CurrentBulletsCount.Value == _weapon.MaxBulletsCount ||
                IsReloading.Value)
                return;

            await UpdateToken(_reloadWeaponCancellationToken);
            await PlayReloadAnimation(_reloadWeaponCancellationToken.Token);

            _weapon.Reload();
        }

        public async UniTask Shoot()
        {
            if (IsReloading.Value)
                return;

            await UpdateToken(_shootCancellationToken);

            _weapon.Shoot();
            await PlayShootAnimation(_shootCancellationToken.Token);
        }

        private async UniTask UpdateToken(CancellationTokenSource cts)
        {
            cts?.Cancel();
            await UniTask.Yield();
            cts = new();
        }
        #endregion

        public async UniTask SwitchWeapon(IWeapon weapon)
        {
            if (_weapon == weapon)
                return;

            _switchWeaponCancellationToken?.Cancel();
            await UniTask.Yield();
            _switchWeaponCancellationToken = new();

            _weapon = weapon;
            _weaponDisposables?.Clear();

            await PlayGrabAnimation(_switchWeaponCancellationToken.Token);

            if (_weapon.CurrentBulletsCount.Value == 0)
            {
                Reload();
            }
        }

        private async UniTask PlayGrabAnimation(CancellationToken cancellationToken)
        {

        }

        private async UniTask PlayReloadAnimation(CancellationToken cancellationToken)
        {

        }

        private async UniTask PlayShootAnimation(CancellationToken cancellationToken)
        {

        }
    }
}