using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Extensions;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using YG;
using Zenject;

public class Level : MonoBehaviour
{
    [SerializeField] private List<Transform> _enemiesSpawns;
    [SerializeField] private int _enemyCountMin;
    [SerializeField] private int _enemyCountMax;
    [SerializeField] private List<Transform> _playerSpawns;
    [SerializeField] private List<Enemy> _enemyPrefabs;
    [SerializeField] private List<NavMeshData> navMeshData;

    private Character _player;
    private List<NavMeshDataInstance> _navMeshInstances = new();

    [Inject] private Character.Factory _characterFactory;
    [Inject] private ILevelProgressObserver _levelProgressObserver;
    [Inject] private DiContainer _container;
    [Inject] private CursorLocker _cursorLocker;

    public void Start()
    {
        foreach (var nmData in navMeshData)
            _navMeshInstances.Add(NavMesh.AddNavMeshData(nmData));

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        var playerPlace = _playerSpawns.Random();
        _player = _characterFactory.Create();
        _player.transform.position = playerPlace.position;
        _player.transform.rotation = playerPlace.rotation;
        _player.Initialize();

        _levelProgressObserver.Observe(enemies);
        YandexGame.GameplayStart();
    }

    private void OnDestroy()
    {
        foreach (var instance in _navMeshInstances)
            NavMesh.RemoveNavMeshData(instance);
    }
}
