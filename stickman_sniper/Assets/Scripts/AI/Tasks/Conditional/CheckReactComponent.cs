using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;
using UnityEngine;

public class CheckReactComponent : Conditional
{
    public SharedFloat Radius;
    public SharedLayerMask LayerMask;
    public SharedLayerMask VisibleItemsLayerMask;
    public SharedNavMeshAgent NavMeshAgent;
    public SharedVector3 LastCharacterSeenPosition;
    public SharedVector3 LastCharacterShootPosition;

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
            if (_colliders[0].TryGetComponent<RaycastPointsProvider>(out var raycastPointsProvider))
            {
                Ray ray = new(_raycastPointsProvider.RaycastOrigin.position,
                    (raycastPointsProvider.RaycastTarget.position - _raycastPointsProvider.RaycastOrigin.position).normalized);

                //Debug.DrawRay(_raycastPointsProvider.RaycastOrigin.position, 5 * (raycastPointsProvider.RaycastTarget.position - _raycastPointsProvider.RaycastOrigin.position).normalized);

                if (Physics.Raycast(ray, out RaycastHit info, Radius.Value, VisibleItemsLayerMask.Value, QueryTriggerInteraction.Ignore))
                {
                    if (info.colliderInstanceID.Equals(_colliders[0].GetInstanceID()))
                    {
                        //Debug.DrawLine(_raycastPointsProvider.RaycastOrigin.position, raycastPointsProvider.RaycastTarget.position);
                        if (_colliders[0].TryGetComponent<TransformRaycastPositionProvider>(out var raycastPositionProvider))
                        {
                            LastCharacterSeenPosition.Value = raycastPositionProvider.GetPosition();
                        }

                        Vector3 end = new(raycastPointsProvider.RaycastOrigin.position.x,
                            _raycastPointsProvider.RaycastOrigin.position.y,
                            raycastPointsProvider.RaycastOrigin.position.z);

                        Ray straightRay = new(_raycastPointsProvider.RaycastOrigin.position, (end - _raycastPointsProvider.RaycastOrigin.position).normalized);

                        //Debug.DrawRay(_raycastPointsProvider.RaycastOrigin.position, (end - _raycastPointsProvider.RaycastOrigin.position).normalized);

                        if (Physics.Raycast(straightRay, out info, Radius.Value, VisibleItemsLayerMask.Value, QueryTriggerInteraction.Ignore))
                        {
                            LastCharacterShootPosition.Value = info.point;
                            Debug.DrawRay(_raycastPointsProvider.RaycastOrigin.position, (LastCharacterShootPosition.Value - _raycastPointsProvider.RaycastOrigin.position).normalized);
                        }
                        else if (_colliders[0].TryGetComponent<TransformShootPositionProvider>(out var shootPositionProvider))
                        {
                            LastCharacterShootPosition.Value = shootPositionProvider.GetPosition();
                        }

                        result = TaskStatus.Success;
                    }
                }
            }
        }

        return result;
    }
}
