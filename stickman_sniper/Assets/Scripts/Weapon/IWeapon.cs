using Cysharp.Threading.Tasks;
using DWTools.Customization;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace DWTools
{
    public interface IAnimationInterface
    {
        RuntimeAnimatorController Animator { get; }
        void SetAnimator(RuntimeAnimatorController animatorController);
    }

    [Serializable]
    public class SwaySettings
    {
        public float Speed;
        public float SensitivityMultiplier;
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
        GameObject SlowmotionBulletPrefab { get; }
        GameObject View { get; set; }
        SwaySettings SwaySettings { get; }

        IReadOnlyReactiveProperty<bool> CanShoot { get; }
        IReadOnlyReactiveProperty<bool> IsGrabing { get; }
        IReadOnlyReactiveProperty<bool> IsReloading { get; }
        IReadOnlyReactiveProperty<bool> IsShooting { get; }
        IReadOnlyReactiveProperty<bool> IsAiming { get; }

        void Shoot();
        void Reload();
        void Grab();
        void SetAim(bool aim);
        void Initialize(WeaponModel model, WeaponState weaponState, ICustomizationDataProvider customizationContainer);
        UniTask Customize(CustomizableEntityProvider customizeItems);
    }

    public interface IHands
    {
    }
}