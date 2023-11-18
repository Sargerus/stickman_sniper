using DWTools;
using UnityEngine;

public abstract class BaseInputHandlerSO : ScriptableObject
{
    public abstract IInputHandler GetHandler();
}
