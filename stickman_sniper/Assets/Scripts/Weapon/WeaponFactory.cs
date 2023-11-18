using DWTools;
using UnityEngine;

public class WeaponFactory
{
    private WeaponsContainerSO _weaponsContainerSO;

    public WeaponFactory(WeaponsContainerSO weaponsContainerSO)
    {
        _weaponsContainerSO = weaponsContainerSO;
    }

    public IWeapon CreateRifle(Vector3 position, Quaternion rotation, Transform parent)
    {
        return Create("sniper_rifle", position, rotation, parent);
    }

    public IWeapon Create(string key, Vector3 position, Quaternion rotation, Transform parent)
    {
        var prefab = _weaponsContainerSO.Get(key);

        return GameObject.Instantiate(prefab.gameObject, position, rotation, parent).GetComponent<IWeapon>();
    }
}
