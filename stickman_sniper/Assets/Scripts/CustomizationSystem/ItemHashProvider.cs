using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using UnityEngine;

public class ItemHashProvider : MonoBehaviour
{
    [ReadOnly]
    public string Hash;

#if UNITY_EDITOR
    [Button]
    private void GenerateHash()
    {
        if (!string.IsNullOrEmpty(Hash))
            return;

        Hash = Guid.NewGuid().GetHashCode().ToString();
    }

    [Button]
    private void ClearHash()
    {
        Hash = null;
    }

    [Button]
    private void Copy()
    {
        Clipboard.Copy(Hash);
    }

    private void Reset()
    {
        GenerateHash();
    }
#endif
}
