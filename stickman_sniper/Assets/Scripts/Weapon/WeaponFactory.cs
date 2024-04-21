using DWTools;
using DWTools.Customization;
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

    public IWeapon CreateRifle()
    {
        return Create("sniper_rifle");
    }

    public IWeapon Create(string key)
    {
        var so = _weaponsContainerSO.Get(key);

        IWeapon result = so.GetWeapon();
        _diContainer.Inject(result);

        return result;
    }
}
