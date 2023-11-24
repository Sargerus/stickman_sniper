using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;

namespace DWTools
{
    public interface IAnimationInterface
    {
        RuntimeAnimatorController Animator { get; }
        void SetAnimator(RuntimeAnimatorController animatorController);
    }

    public interface IWeapon
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
        GameObject View { get; }

        IReadOnlyReactiveProperty<bool> CanShoot { get; }
        IReadOnlyReactiveProperty<bool> IsGrabing { get; }
        IReadOnlyReactiveProperty<bool> IsReloading { get; }
        IReadOnlyReactiveProperty<bool> IsShooting { get; }

        void Shoot();
        void Reload();
        void Grab();
        void Initialize(WeaponModel model, WeaponState weaponState);
    }

    public interface IHands
    {
    }
}