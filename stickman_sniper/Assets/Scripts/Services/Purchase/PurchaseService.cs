using System.Collections.Generic;
using System.Linq;
using UniRx;
using YG;
using Zenject;

namespace stickman_sniper.Purchases
{
    public interface IPurchaseService
    {
        IReadOnlyReactiveProperty<bool> GetIsPurchasedReactiveProperty(string hash);
        void Purchase(string hash);
    }

    internal class PurchaseService : IPurchaseService
    {
        private HashSet<string> _purchases = new();
        private Dictionary<string, ReactiveProperty<bool>> _cachedProperties = new();

        public PurchaseService()
        {
            _purchases = YandexGame.savesData.purchases.ToHashSet<string>();
        }

        public IReadOnlyReactiveProperty<bool> GetIsPurchasedReactiveProperty(string hash) => GetIsPurchasedReactivePropertyInternal(hash);

        private ReactiveProperty<bool> GetIsPurchasedReactivePropertyInternal(string hash)
        {
            if (!_cachedProperties.TryGetValue(hash, out var property))
            {
                property = new(_purchases.Contains(hash));
                _cachedProperties.Add(hash, property);
            }

            return property;
        }

        public void Purchase(string hash)
        {
            _purchases.Add(hash);
            Save();
            GetIsPurchasedReactivePropertyInternal(hash).Value = true;
        }

        private void Save()
        {
            YandexGame.savesData.purchases = _purchases.ToList();
            YandexGame.SaveProgress();
        }
    }

}