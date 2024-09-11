using stickman_sniper.Currency;
using System.Collections.Generic;
using YG;
using static BootstrapSceneState;

public interface ISaveService
{
    void SaveProgress();

    List<WeaponIndexes> GetWeaponIndexes();
    void SetWeaponIndexes(List<WeaponIndexes> weaponIndexes);

    int GetLevelsPassed();
    void SetLevelsPassed(int levelPassed);

    List<CurrencyEntity> GetCurrencies();
    void SetCurrencies(List<CurrencyEntity> currencies);

    List<string> GetPurchases();
    void SetPurchases(List<string> purchases);
}

internal class SaveService : ISaveService
{
    public void SaveProgress()
        => YandexGame.SaveProgress();

    public List<WeaponIndexes> GetWeaponIndexes()
        => YandexGame.savesData.weaponSelectedIndexes;

    public void SetWeaponIndexes(List<WeaponIndexes> weaponIndexes)
        => YandexGame.savesData.weaponSelectedIndexes = weaponIndexes;

    public int GetLevelsPassed()
        => YandexGame.savesData.levelsPassed;

    public void SetLevelsPassed(int levelPassed)
        => YandexGame.savesData.levelsPassed = levelPassed;

    public List<CurrencyEntity> GetCurrencies()
        => YandexGame.savesData.currencies;

    public void SetCurrencies(List<CurrencyEntity> currencies)
        => YandexGame.savesData.currencies = currencies;

    public List<string> GetPurchases()
        => YandexGame.savesData.purchases;

    public void SetPurchases(List<string> purchases)
        => YandexGame.savesData.purchases = purchases;
}
