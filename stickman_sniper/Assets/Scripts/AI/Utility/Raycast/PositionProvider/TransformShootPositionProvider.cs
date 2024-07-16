using UnityEngine;

public class TransformShootPositionProvider : AbstractPositionProvider
{
    [SerializeField] private Transform target;

    public override Vector3 GetPosition() => target == null ? Vector3.zero : target.position;
}
