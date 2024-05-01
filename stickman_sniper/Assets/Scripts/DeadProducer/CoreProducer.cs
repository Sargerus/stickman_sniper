using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace stickman_sniper.Producer
{
    public interface ICoreProducer
    {
        UniTask KillEnemyWeaponSlowmotion(Enemy enemy, Vector3 startPos, Vector3 hitPoint, GameObject prefab, Action onHitCallback);
    }

    internal class CoreProducer : ICoreProducer
    {
        private readonly IBulletFlyProducer _bulletSlowmotionService;
        private readonly IEnemyDeadProducer _enemyDeadProducer;

        public CoreProducer(IBulletFlyProducer bulletSlowmotionService,
            IEnemyDeadProducer enemyDeadProducer)
        {
            _bulletSlowmotionService = bulletSlowmotionService;
            _enemyDeadProducer = enemyDeadProducer;
        }

        public async UniTask KillEnemyWeaponSlowmotion(Enemy enemy, Vector3 startPos, Vector3 hitPoint, GameObject prefab, Action onHitCallback)
        {
            var bulletSlowmotion = GameObject.Instantiate(prefab);
            var bulletDirector = bulletSlowmotion.GetComponent<ICinemachineDirector>();

            await _bulletSlowmotionService.SendBulletInSlowmotionAsync(startPos, hitPoint, bulletDirector);

            bulletSlowmotion.gameObject.SetActive(false);
            GameObject.Destroy(bulletSlowmotion, 0.1f);

            onHitCallback?.Invoke();
            await _enemyDeadProducer.ShowEnemyDeath(enemy);
        }
    }
}