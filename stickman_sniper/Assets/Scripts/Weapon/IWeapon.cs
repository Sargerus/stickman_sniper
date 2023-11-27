using System;
using UniRx;
using UnityEngine;

namespace DWTools
{
    public interface IAnimationInterface
    {
        RuntimeAnimatorController Animator { get; }
        void SetAnimator(RuntimeAnimatorController animatorController);
    }

    public interface IWeapon : IDisposable
    {
        IReadOnlyReactiveProperty<int> CurrentBulletsCount { get; }
        IReadOnlyReactiveProperty<int> StashedBulletsCount { get; }

        string Key { get; }
        int BulletType { get; }
        float Damage { get; }
        float ReloadingTime { get; }
        int MaxBulletsCount { get; }
        int MagazineCapacity { get; }
        int TimeBetweenShots { get; }
        GameObject Prefab { get; }
        GameObject View { get; set; }

        IReadOnlyReactiveProperty<bool> CanShoot { get; }
        IReadOnlyReactiveProperty<bool> IsGrabing { get; }
        IReadOnlyReactiveProperty<bool> IsReloading { get; }
        IReadOnlyReactiveProperty<bool> IsShooting { get; }
        IReadOnlyReactiveProperty<bool> IsAiming { get; }

        void Shoot();
        void Reload();
        void Grab();
        void Aim();
        void Initialize(WeaponModel model, WeaponState weaponState);
    }

    public interface IHands
    {
    }
}