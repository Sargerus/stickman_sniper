using Cysharp.Threading.Tasks;
using System;
using System.Numerics;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace stickman_sniper.Producer
{
    public interface ICoreProducer
    {
        UniTask KillEnemyWeaponSlowmotion(Enemy enemy, UnityEngine.Vector3 hitPoint, Action onHitCallback);
    }

    internal class CoreProducer : ICoreProducer
    {
        private readonly IWeaponService _weaponService;
        private readonly IBulletFlyProducer _bulletSlowmotionService;
        private readonly IEnemyDeadProducer _enemyDeadProducer;

        public CoreProducer(IWeaponService weaponService,
            IBulletFlyProducer bulletSlowmotionService,
            IEnemyDeadProducer enemyDeadProducer)
        {
            _weaponService = weaponService;
            _bulletSlowmotionService = bulletSlowmotionService;
            _enemyDeadProducer = enemyDeadProducer;
        }

        public async UniTask KillEnemyWeaponSlowmotion(Enemy enemy, UnityEngine.Vector3 hitPoint, Action onHitCallback)
        {
            var bulletSlowmotion = GameObject.Instantiate(_weaponService.CurrentWeapon.Value.SlowmotionBulletPrefab);
            var bulletDirector = bulletSlowmotion.GetComponent<ICinemachineDirector>();

            await _bulletSlowmotionService.SendBulletInSlowmotionAsync(_weaponService.CurrentWeapon.Value.View.transform.position,
                hitPoint, bulletDirector);

            bulletSlowmotion.gameObject.SetActive(false);
            GameObject.Destroy(bulletSlowmotion, 0.1f);

            onHitCallback?.Invoke();
            await _enemyDeadProducer.ShowEnemyDeath(enemy);
        }
    }
}