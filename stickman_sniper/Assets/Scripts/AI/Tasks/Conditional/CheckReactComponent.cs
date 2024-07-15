using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;
using UnityEngine;

public class CheckReactComponent : Conditional
{
    public SharedFloat Radius;
    public SharedLayerMask LayerMask;
    public SharedNavMeshAgent NavMeshAgent;

    private Collider[] _colliders = new Collider[1];
    private RaycastPointsProvider _raycastPointsProvider;

    public override void OnAwake()
    {
        _raycastPointsProvider = transform.GetComponentInChildren<RaycastPointsProvider>();
    }

    public override TaskStatus OnUpdate()
    {
        Array.Clear(_colliders, 0, _colliders.Length);
        TaskStatus result = TaskStatus.Failure;
        float halfExtent = Radius.Value;

        if (Physics.OverlapBoxNonAlloc(NavMeshAgent.Value.transform.position,
            new Vector3(halfExtent, halfExtent, halfExtent), _colliders,
            Quaternion.identity, LayerMask.Value, QueryTriggerInteraction.Ignore) > 0)
        {
            if (_colliders[0].transform.root.TryGetComponent<RaycastPointsProvider>(out var raycastPointsProvider))
            {
                Ray ray = new(_raycastPointsProvider.RaycastOrigin.position,
                    (raycastPointsProvider.RaycastTarget.position - _raycastPointsProvider.RaycastOrigin.position).normalized);

                Debug.DrawRay(_raycastPointsProvider.RaycastOrigin.position, (raycastPointsProvider.RaycastTarget.position - _raycastPointsProvider.RaycastOrigin.position).normalized);
                if (Physics.Raycast(ray, out RaycastHit info, Radius.Value))
                {
                    if (info.colliderInstanceID.Equals(_colliders[0].GetInstanceID()))
                    {
                        result = TaskStatus.Success;
                    }
                }
            }
        }

        return result;
    }
}
