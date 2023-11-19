using DWTools;
using UnityEngine;
using Zenject;

public class WeaponFactory
{
    private readonly WeaponsContainerSO _weaponsContainerSO;
    private readonly DiContainer _diContainer;

    public WeaponFactory(WeaponsContainerSO weaponsContainerSO, DiContainer diContainer)
    {
        _weaponsContainerSO = weaponsContainerSO;
        _diContainer = diContainer;
    }

    public IWeapon CreateRifle(Vector3 position, Quaternion rotation, Transform parent, out IWeaponStateController weaponStateController)
    {
        return Create("sniper_rifle", position, rotation, parent, out weaponStateController);
    }

    public IWeapon Create(string key, Vector3 position, Quaternion rotation, Transform parent, out IWeaponStateController weaponStateController)
    {
        var so = _weaponsContainerSO.Get(key);
        var weapon = GameObject.Instantiate(so.Value.gameObject, position, rotation, parent);

        weaponStateController = weapon.GetComponent<IWeaponStateController>();
        _diContainer.InjectGameObject(weapon);

        return weapon.GetComponent<IWeapon>();
    }
}
