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

    internal class PurchaseService : IInitializable, IPurchaseService
    {
        private HashSet<string> _purchases = new();
        private Dictionary<string, ReactiveProperty<bool>> _cachedProperties = new();

        public void Initialize()
        {
            _purchases = YandexGame.savesData.purchases.ToHashSet<string>();
        }

        public IReadOnlyReactiveProperty<bool> GetIsPurchasedReactiveProperty(string hash) => GetIsPurchasedReactivePropertyInternal(hash);

        private ReactiveProperty<bool> GetIsPurchasedReactivePropertyInternal(string hash)
        {
            if (!_cachedProperties.TryGetValue(hash, out var property))
            {
                property = new(false);
                _cachedProperties.Add(hash, property);
            }

            return property;
        }

        public void Purchase(string hash)
        {
            _purchases.Add(hash);
            GetIsPurchasedReactivePropertyInternal(hash).Value = true;
        }

        private void Save()
        {
            YandexGame.savesData.purchases = _purchases.ToList();
            YandexGame.SaveProgress();
        }
    }

}