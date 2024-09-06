using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[SHOP] ShopSOInstaller", fileName = "ShopScriptableObjectInstaller")]
public class ShopScriptableInstaller : ScriptableObjectInstaller
{
    [SerializeField] private AvailableWeaponConfig _availableWeaponConfig;
    [SerializeField] private ShopPresentationConfig shopPresentationConfig;

    public override void InstallBindings()
    {
        Container.BindInstances(shopPresentationConfig, _availableWeaponConfig);
    }
}
