using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[GAME] GameSOInstaller", fileName = "GameScriptableObjectInstaller")]
public class GameScriptableInstaller : ScriptableObjectInstaller
{
    [SerializeField] private GameWeaponConfig _allWeaponConfig;

    public override void InstallBindings()
    {
        Container.BindInstances(_allWeaponConfig);
    }
}
