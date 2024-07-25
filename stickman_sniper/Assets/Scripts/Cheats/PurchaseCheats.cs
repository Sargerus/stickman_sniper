using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace stickman_sniper.Purchases
{
    public class PurchaseCheats : MonoBehaviour
    {
        private IPurchaseService _purchaseService;

        private void ResolvePurchaseService()
        {
            if (_purchaseService == null)
            {
                var contexts = FindObjectsOfType<SceneContext>();
                foreach(var context in contexts)
                {
                    if (!context.Container.HasBinding<IPurchaseService>())
                        continue;

                    _purchaseService = context.Container.Resolve<IPurchaseService>();
                }
            }
        }

        [Button]
        private void Purchase(string hash)
        {
            ResolvePurchaseService();
            _purchaseService.Purchase(hash);
        }
    }
}