using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "[PRELOAD]/File", fileName = "new PreloadFile")]
public class PreloadScriptableObject : ScriptableObject
{
    public string Key;
    public List<AssetItemReference> Assets;
}
