using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Material _deadMaterial;

    private List<Rigidbody> _rb;
    private Animator _animator;
    private SkinnedMeshRenderer _smr;
    private IEnemyCounter _enemyCounter;

    private void Awake()
    {
        _rb = GetComponentsInChildren<Rigidbody>().ToList();
        _animator = GetComponent<Animator>();
        _smr = GetComponentInChildren<SkinnedMeshRenderer>();

        foreach (var rb in _rb)
        {
            rb.isKinematic = true;
        }
    }

    public void PrepareForDeath()
    {
        _animator.enabled = false;

        foreach (var rb in _rb)
        {
            rb.isKinematic = false;
        }

        _smr.material = _deadMaterial;

        _enemyCounter.EnemyKilled();
    }

    public void Link(IEnemyCounter enemyCounter) 
        => _enemyCounter = enemyCounter;
}
