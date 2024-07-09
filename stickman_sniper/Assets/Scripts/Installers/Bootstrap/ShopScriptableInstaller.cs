using DWTools.Customization;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "[SHOP] ShopSOInstaller", fileName = "ShopScriptableObjectInstaller")]
public class ShopScriptableInstaller : ScriptableObjectInstaller
{
    //[SerializeField] private GameWeaponConfig _allWeaponConfig;
    [SerializeField] private AvailableWeaponConfig _availableWeaponConfig;
    [SerializeField] private CustomiationDataContainerSO _customiationDataContainerSO;
    [SerializeField] private ShopPresentationConfig shopPresentationConfig;

    public override void InstallBindings()
    {
        Container.BindInstances(_customiationDataContainerSO, shopPresentationConfig, _availableWeaponConfig);
    }
}
