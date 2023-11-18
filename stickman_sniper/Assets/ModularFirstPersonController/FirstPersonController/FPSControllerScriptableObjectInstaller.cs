using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "FPSControllerSOInstaller", fileName = "FPSControllerScriptableObjectInstaller")]
public class FPSControllerScriptableObjectInstaller : ScriptableObjectInstaller
{
    [SerializeField] private WeaponsContainerSO _weaponContainer;

    public override void InstallBindings()
    {
        Container.BindInstances(_weaponContainer);
    }
}
