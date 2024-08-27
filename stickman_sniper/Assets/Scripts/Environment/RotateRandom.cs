using UnityEngine;

public class RotateRandom : MonoBehaviour
{
    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 180), 0);
    }
}
