using DWTools.Extensions;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Level : MonoBehaviour
{
    [SerializeField] private List<Transform> _enemiesSpawns;
    [SerializeField] private int _enemyCountMin;
    [SerializeField] private int _enemyCountMax;
    [SerializeField] private List<Transform> _playerSpawns;
    [SerializeField] private List<Enemy> _enemyPrefabs;
    [SerializeField] private List<NavMeshData> navMeshData;

    private List<Enemy> _enemies = new();
    private Character _player;
    private List<NavMeshDataInstance> _navMeshInstances = new();

    [Inject] private Character.Factory _characterFactory;
    //[Inject] private ILevelProgressObserver _levelProgressObserver;
    [Inject] private DiContainer _container;

    public async void Start()
    {
        foreach (var nmData in navMeshData)
            _navMeshInstances.Add(NavMesh.AddNavMeshData(nmData));

        int max = Math.Min(_enemiesSpawns.Count, _enemyCountMax);
        int enemiesCount = UnityEngine.Random.Range(_enemyCountMin, max);

        List<Transform> copyList = new(_enemiesSpawns);
        for (int i = 0; i < enemiesCount; i++)
        {
            Transform place = copyList.Random();
            copyList.Remove(place);

            var enemy = _container.InstantiatePrefabForComponent<Enemy>(_enemyPrefabs.Random(), place.position, place.rotation, transform);
            _enemies.Add(enemy);
        }

        var playerPlace = _playerSpawns.Random();
        _player = _characterFactory.Create();
        _player.transform.position = playerPlace.position;
        _player.transform.rotation = playerPlace.rotation;
        await _player.Initialize();
        //_levelProgressObserver.Observe(_enemies);
    }

    private void OnDestroy()
    {
        foreach (var instance in _navMeshInstances)
            NavMesh.RemoveNavMeshData(instance);
    }
}
