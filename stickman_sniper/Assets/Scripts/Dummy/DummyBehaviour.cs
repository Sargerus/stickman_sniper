using System.Linq;
using UnityEngine;

public class DummyBehaviour : MonoBehaviour
{
    private void Awake()
    {
        var rbs = GetComponentsInChildren<Rigidbody>().ToList();

        foreach (var rb in rbs)
        {
            rb.isKinematic = true;
        }
    }
}
