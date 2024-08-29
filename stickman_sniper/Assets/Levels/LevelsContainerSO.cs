using DWTools.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="[LEVEL]Level/LevelContainer", fileName = "LevelsContainer")]
public class LevelsContainerSO : ScriptableObject
{
    public Level TestLevel;
    public List<Level> Levels;

#if UNITY_EDITOR
    [Button]
    private void Shuffle()
    {
        Levels.Shuffle();
    }
#endif
}
