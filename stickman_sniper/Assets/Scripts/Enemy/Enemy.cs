using DWTools.Slowmotion;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class Enemy : SlowmotionRoot
{
    [SerializeField] private Material _deadMaterial;

    private List<Rigidbody> _rb;
    private SlowmotionAnimator _animator;
    private SkinnedMeshRenderer _smr;

    private ReactiveProperty<bool> _isAlive = new(true);
    public IReadOnlyReactiveProperty<bool> IsAlive => _isAlive;

    private void Awake()
    {
        _rb = GetComponentsInChildren<Rigidbody>().ToList();
        _animator = GetComponent<SlowmotionAnimator>();
        _smr = GetComponentInChildren<SkinnedMeshRenderer>();

        foreach (var rb in _rb)
        {
            rb.isKinematic = true;
        }
    }

    public void PrepareForDeath()
    {
        _animator.enabled = false;
        _animator.AllowToUpdate = false;

        foreach (var rb in _rb)
        {
            rb.isKinematic = false;
        }

        _smr.material = _deadMaterial;
        _isAlive.Value = false;
    }
}
