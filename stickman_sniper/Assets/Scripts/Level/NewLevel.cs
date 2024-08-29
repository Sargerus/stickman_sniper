using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Extensions;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class NewLevel : MonoBehaviour
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
        Debug.Log($"Placer place {playerPlace.position}");
        _player = _characterFactory.Create();
        Debug.Log($"Player position before {_player.transform.position}");
        _player.transform.position = playerPlace.position;
        Debug.Log($"Player position after {_player.transform.position}");
        _player.transform.rotation = playerPlace.rotation;
        _player.Initialize();

        _levelProgressObserver.Observe(enemies);
    }

    private void Update()
    {
        if(_player!= null )
        Debug.Log($"Player position {_player.transform.position}");
    }

    private void OnDestroy()
    {
        foreach (var instance in _navMeshInstances)
            NavMesh.RemoveNavMeshData(instance);
    }
}
