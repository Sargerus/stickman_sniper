using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "[PRELOAD]/Container", fileName = "new PreloadContainer")]
public class PreloadScriptableObjectContainer : ScriptableObject
{
    public List<PreloadScriptableObject> Assets;
}
