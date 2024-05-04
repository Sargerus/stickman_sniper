using Cinemachine;
using DWTools.Extensions;
using DWTools.RPG;
using DWTools.Slowmotion;
using Sirenix.OdinInspector;
using stickman_sniper.Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class Enemy : SlowmotionRoot, ICinemachineDirector
{
    [SerializeField] private Material _deadMaterial;
    [BoxGroup("Cinemachine"), SerializeField] private List<CinemachineVirtualCamera> _deadCams;
    [field: SerializeField, BoxGroup("Cinemachine")] public int Duration { get; private set; }

    [BoxGroup("Canvas"), SerializeField] private EnemyHPCanvas enemyHPCanvas;

    [SerializeField] private Character _character;

    private List<Rigidbody> _rb;
    private SlowmotionAnimator _animator;
    private SkinnedMeshRenderer _smr;

    private ReactiveProperty<bool> _isAlive = new(true);
    public IReadOnlyReactiveProperty<bool> IsAlive => _isAlive;

    private Dictionary<string, object> _cinemaData = new();

    private void Awake()
    {
        _rb = GetComponentsInChildren<Rigidbody>().ToList();
        _animator = GetComponent<SlowmotionAnimator>();
        _smr = GetComponentInChildren<SkinnedMeshRenderer>();
        _character.CalculateStats();

        foreach (var rb in _rb)
        {
            rb.isKinematic = true;
        }

        _deadCams.ForEach(g => g.gameObject.SetActive(false));

        if (_character.TryGetReactiveStat(CharacterStat.Health, out var property) &&
            _character.TryGetBaseStat(CharacterStat.Health, out var baseStat))
        {
            float plusStatValue = 0;
            if (_character.TryGetPlusStat(CharacterStat.Health, out var plusStat))
                plusStatValue = plusStat.Value;

            property.SubscribeWithState2(baseStat, plusStat, (val, baseStat, plusStat) =>
            {
                enemyHPCanvas.SetHp(val / (baseStat.Value + plusStatValue));
            }).AddTo(this);
        }
    }

    public void PrepareForDeath()
    {
        //_animator.enabled = false;
        //_animator.AllowToUpdate = false;

        foreach (var rb in _rb)
        {
            rb.isKinematic = false;
        }

        enemyHPCanvas.SetActiveCanvas(false);
        _smr.material = _deadMaterial;
        _isAlive.Value = false;
    }

    public CinemachineVirtualCamera GetRandomCamera() => _deadCams.Random();

    public void TurnOffAllCameras()
    {
        _deadCams.ForEach(g => g.gameObject.SetActive(false));
    }

    public void SetValue(string key, object value)
    {
        _cinemaData[key] = value;
    }

    public bool TryGetValue(string key, out object value)
    {
        _cinemaData.TryGetValue(key, out value);
        return value != null;
    }

    public void SetProgress(float progress)
    {
        //
    }

    public void Damage(float damage)
    {
        _character.Damage(damage);
    }

    public bool TryGetStat(CharacterStat characterStat, out StatEntity stat)
    {
        return _character.TryCurrentGetStat(characterStat, out stat);
    }

    public void Clear()
    {
        _cinemaData?.Clear();
    }
}
