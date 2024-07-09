using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;

public class CustomizationScreenCertainWeapon : MonoBehaviour
{
    [SerializeField] private PodiumController podiumController;

    private ShopPresentationConfig _shopProductConfig;
    private ShopProductVisual _productVisuals;

    public void ResolveDependencies(ShopPresentationConfig shopProductConfig)
    {
        _shopProductConfig = shopProductConfig;
    }

    public async UniTask Init(string key, IObserver<string> backClickHandler)
    {
        _productVisuals = _shopProductConfig.GetConfig(key);
        podiumController.Initialize(_productVisuals, null);
    }

    public async UniTask DeInit()
    {
    }
}
