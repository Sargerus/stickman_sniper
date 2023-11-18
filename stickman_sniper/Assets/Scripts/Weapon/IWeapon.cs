using Cysharp.Threading.Tasks;
using UniRx;

namespace DWTools
{
    public interface IWeapon
    {
        public int BulletType { get; }
        public ReactiveProperty<int> CurrentBulletsCount { get; }
        public int MaxBulletsCount { get; }

        public ReactiveProperty<bool> CanShoot { get; }
        public ReactiveProperty<bool> IsReloading { get; }

        UniTask Shoot();
        UniTask Reload();
    }
}