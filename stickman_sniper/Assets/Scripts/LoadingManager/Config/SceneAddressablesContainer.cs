using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "[SCENE] Loading Manager Scenes/Scene container", fileName = "new LoadingManagerSceneContainer")]
public class SceneAddressablesContainer : ScriptableObject
{
    public List<AssetItemReference> SceneContainer;

    public AssetReference Get(string key)
    {
        return SceneContainer.FirstOrDefault(g => g.Key.Equals(key))?.AssetReference;
    }
}

[Serializable]
public class AssetItemReference
{
    public string Key;
    public AssetReference AssetReference;
}