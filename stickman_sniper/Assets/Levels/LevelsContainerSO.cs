using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="[LEVEL]Level/LevelContainer", fileName = "LevelsContainer")]
public class LevelsContainerSO : ScriptableObject
{
    public List<Level> Levels;
}
