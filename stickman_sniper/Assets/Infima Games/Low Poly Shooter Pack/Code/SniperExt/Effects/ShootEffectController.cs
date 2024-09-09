using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class ShootEffectController : MonoBehaviour
    {
        [SerializeField] private InfimaWeapon weapon;
        [SerializeField] private GameObject muzzleShootGO;

        private CompositeDisposable _disposables = new();

        private void Start()
        {
            muzzleShootGO.SetActive(false);

            weapon.OnFire.Subscribe(async _ =>
            {
                muzzleShootGO.SetActive(false);
                muzzleShootGO.SetActive(true);
                await UniTask.Delay(50);
                muzzleShootGO.SetActive(false);
            }).AddTo(_disposables);
        }
    }
}