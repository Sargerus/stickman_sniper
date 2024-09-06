using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public interface IEnemyBattleController
{
    UniTask Shoot(Vector3 target);
    UniTask Shoot(Transform target);
}

public class EnemyBattleController : MonoBehaviour, IEnemyBattleController
{
    [SerializeField, BoxGroup("Bullet")] private AntiCharacterBullet bulletPrefab;
    [SerializeField, BoxGroup("Bullet")] private Transform bulletStartPosition;
    [SerializeField, BoxGroup("Bullet")] private float distance;
    [SerializeField, BoxGroup("Bullet")] private float damage;
    [SerializeField, BoxGroup("Bullet")] private float speed;

    private DiContainer _diContainer;

    private bool _isInitialized;

    [Inject]
    private void Construct(DiContainer diContainer)
    {
        _diContainer = diContainer;
        _isInitialized = true;
    }

    public async UniTask Shoot(Transform target) => await Shoot(target.position);

    public async UniTask Shoot(Vector3 target)
    {
        if (!_isInitialized)
            return;

        var bullet = _diContainer.InstantiatePrefab(bulletPrefab, bulletStartPosition.position, Quaternion.identity, null);
        if (!bullet.TryGetComponent<AntiCharacterBullet>(out var acb))
        {
            Destroy(bullet);
            return;
        }

        acb.Setup(bulletStartPosition.position, (target - bulletStartPosition.position).normalized, distance, damage, speed);
    }
}
