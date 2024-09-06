using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using System;
using UnityEngine;

namespace Customization
{
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
}