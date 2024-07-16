using UnityEngine;

public class TransformRaycastPositionProvider : AbstractRaycastPositionProvider
{
    [SerializeField]
    private Transform target;

    public override Vector3 GetPosition() => target == null ? Vector3.zero : target.position;
}
