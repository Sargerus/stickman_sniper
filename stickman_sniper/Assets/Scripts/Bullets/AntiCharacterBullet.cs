using DWTools.Slowmotion;
using stickman_sniper.Producer;
using UnityEngine;

public class AntiCharacterBullet : SlowmotionRoot
{
    private Vector3 _startPosition;
    private Vector3 _direction;
    private float _distance;
    private float _damage;
    private float _speed;

    private Vector3 _targetCache;

    public void Setup(Vector3 startPosition, Vector3 direction, float distance = 100, float damage = 5, float speed = 3)
    {
        _startPosition = startPosition;
        _direction = direction.normalized;
        _distance = distance;
        _damage = damage;
        _speed = speed;

        _targetCache = _startPosition + _direction * _distance;
        transform.rotation = Quaternion.LookRotation(_direction);
        AllowToUpdate = true;
    }

    protected override void SelfUpdate()
    {
        if (Vector3.Distance(transform.position, _targetCache) <= 1)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetCache, _direction.magnitude * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<CharacterComponent>(out var character))
            {
                character.Character.Damage(_damage);
            }
        }

        Destroy(gameObject);
    }
}
