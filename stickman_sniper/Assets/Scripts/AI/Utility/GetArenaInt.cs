using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class GetArenaInt : MonoBehaviour
{
    [Button]
    private void LogAreaInt(string areaName)
    {
        Debug.Log(NavMesh.GetAreaFromName(areaName));
    }
}
