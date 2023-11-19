using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace DWTools
{
    [Serializable]
    public class WeaponModel
    {
        public string Key;
        public int BulletType;
        public float Damage;
        public float ReloadingTime;
        public int MaxBulletsCount;
        public int MagazineCapacity;
        public int TimeBetweenShots;
    }

    public interface IWeaponAnimationInterface
    {
        void SetAnimator(RuntimeAnimatorController animatorController);

        UniTask Shoot(CancellationToken cancellationToken);
        UniTask Reload(CancellationToken cancellationToken);
        UniTask Grab(CancellationToken cancellationToken);
    }

    public interface IWeapon
    {
        GameObject gameObject { get; }
        IReadOnlyReactiveProperty<int> CurrentBulletsCount { get; }

        string Key { get; }
        int BulletType { get; }
        float Damage { get; }
        float ReloadingTime { get; }
        int MaxBulletsCount { get; }
        int MagazineCapacity { get; }
        int TimeBetweenShots { get; }

        IReadOnlyReactiveProperty<bool> CanShoot { get; }
        IReadOnlyReactiveProperty<bool> IsGrabing { get; }
        IReadOnlyReactiveProperty<bool> IsReloading { get; }
        IReadOnlyReactiveProperty<bool> IsShooting { get; }

        void Shoot();
        void Reload();
        void Grab();
        void Initialize(WeaponModel model);
    }

    public interface IWeaponStateController
    {
        void SetIsGrabing(bool isGrabing);
        void SetIsReloading(bool isReloading);
        void SetIsShooting(bool isShooting);
    }

    public interface IHands
    {
    }
}