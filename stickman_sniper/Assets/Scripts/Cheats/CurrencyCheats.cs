using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace stickman_sniper.Currency
{
    public class CurrencyCheats : MonoBehaviour
    {
        private ICurrencyService _currencyService;

        private void ResolveCurrencyService()
        {
            if (_currencyService == null)
            {
                var contexts = FindObjectsOfType<SceneContext>();
                foreach (var context in contexts)
                {
                    if (!context.Container.HasBinding<ICurrencyService>())
                        continue;

                    _currencyService = context.Container.Resolve<ICurrencyService>();
                }
            }
        }

        [Button]
        private void AddCurrency(string currency, float value)
        {
            ResolveCurrencyService();
            _currencyService.AddCurrency(currency, value);
        }

        [Button]
        private void AddGold(float value)
        {
            AddCurrency(CurrencyServiceConstants.GoldCurrency, value);
        }
    }
}