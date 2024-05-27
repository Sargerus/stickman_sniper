public interface IGameStartWeaponInventoryService
{
    string MainWeapon { get; set; }
    string SecondaryWeapon { get; set; }
}

public class GameStartWeaponInventoryService : IGameStartWeaponInventoryService
{
    public string MainWeapon { get; set; }
    public string SecondaryWeapon { get; set; }

    public GameStartWeaponInventoryService(string mainWeaponByDefault, string secondaryWeaponByDefault)
    {
        MainWeapon = mainWeaponByDefault;
        SecondaryWeapon = secondaryWeaponByDefault;
    }
}
