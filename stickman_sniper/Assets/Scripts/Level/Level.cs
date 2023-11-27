using DW.RandomExtensions;
using DWTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public interface IEnemyCounter
{
    void EnemyKilled();
}

public class Level : MonoBehaviour, IEnemyCounter
{
    [SerializeField] private bool _isEverythingSpawnedAtStart;
    [SerializeField] private List<Transform> _enemiesSpawns;
    [SerializeField] private int _enemyCountMin;
    [SerializeField] private int _enemyCountMax;
    [SerializeField] private List<Transform> _playerSpawns;
    [SerializeField] private List<Enemy> _enemyPrefabs;

    private List<Enemy> _enemies = new();
    private FirstPersonController _player;

    public Action AimComplete;

    private int _enemiesCount;

    [Inject] private FirstPersonController.Factory _fpsFactory;

    public void EnemyKilled()
    {
        _enemiesCount--;

        if (_enemiesCount <= 0)
        {
            AimComplete?.Invoke();
        }
    }

    public void Start()
    {
        List<Enemy> enemies = new();

        if (_isEverythingSpawnedAtStart)
        {
            enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None).ToList();
            enemies.ForEach(g => g.Link(this));
        }
        else
        {
            int max = Math.Min(_enemiesSpawns.Count, _enemyCountMax);
            int enemiesCount = UnityEngine.Random.Range(_enemyCountMin, max);

            List<Transform> copyList = new(_enemiesSpawns);
            for (int i = 0; i < enemiesCount; i++)
            {
                Transform place = copyList.Random();
                copyList.Remove(place);

                var enemy = Instantiate(_enemyPrefabs.Random(), place.position, Quaternion.identity, transform);
                _enemies.Add(enemy);
                enemy.Link(this);
            }
        }

        var playerPlace = _playerSpawns.Random();
        _player = _fpsFactory.Create();
        _player.transform.position = playerPlace.position;

        _player.Freeze(false);
        Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => _player.Freeze(true)).AddTo(this);

        _enemiesCount = enemies.Count;
    }
}
