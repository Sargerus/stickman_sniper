using DW.RandomExtensions;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class Level : MonoBehaviour
{
    [SerializeField] private List<Transform> _enemiesSpawns;
    [SerializeField] private int _enemyCountMin;
    [SerializeField] private int _enemyCountMax;
    [SerializeField] private List<Transform> _playerSpawns;
    [SerializeField] private List<Enemy> _enemyPrefabs;

    private List<Enemy> _enemies = new();
    private FirstPersonController _player;

    [Inject] private FirstPersonController.Factory _fpsFactory;
    [Inject] private ILevelProgressObserver _levelProgressObserver;

    public void Start()
    {
        int max = Math.Min(_enemiesSpawns.Count, _enemyCountMax);
        int enemiesCount = UnityEngine.Random.Range(_enemyCountMin, max);

        List<Transform> copyList = new(_enemiesSpawns);
        for (int i = 0; i < enemiesCount; i++)
        {
            Transform place = copyList.Random();
            copyList.Remove(place);

            var enemy = Instantiate(_enemyPrefabs.Random(), place.position, place.rotation, transform);
            _enemies.Add(enemy);
        }

        var playerPlace = _playerSpawns.Random();
        _player = _fpsFactory.Create();
        _player.transform.position = playerPlace.position;
        _player.transform.rotation = playerPlace.rotation;

        //_player.Freeze(true);
        //Observable.Timer(TimeSpan.FromSeconds(0.8f)).Subscribe(_ => _player.Freeze(false)).AddTo(this);

        _levelProgressObserver.Observe(_enemies);
    }
}
