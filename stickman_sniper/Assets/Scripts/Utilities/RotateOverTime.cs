using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public Vector3 RotationAxis;
    public float RotationSpeed;

    void Update()
    {
        transform.Rotate(RotationSpeed * Time.deltaTime * RotationAxis);
    }
}
