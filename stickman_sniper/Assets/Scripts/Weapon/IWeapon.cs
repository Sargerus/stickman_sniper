using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine;

namespace DWTools
{
    public interface IWeaponAnimationInterface
    {
        void SetAnimator(RuntimeAnimatorController animatorController);

        UniTask Shoot(CancellationToken cancellationToken);
        UniTask Reload(CancellationToken cancellationToken);
        UniTask Grab(CancellationToken cancellationToken);
    }

    public interface IWeapon
    {
        int BulletType { get; }
        IReadOnlyReactiveProperty<int> CurrentBulletsCount { get; }
        int MaxBulletsCount { get; }

        IReadOnlyReactiveProperty<bool> CanShoot { get; }
        IReadOnlyReactiveProperty<bool> IsGrabing { get; }
        IReadOnlyReactiveProperty<bool> IsReloading { get; }

        void Shoot();
        void Reload();
        void Grab();
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