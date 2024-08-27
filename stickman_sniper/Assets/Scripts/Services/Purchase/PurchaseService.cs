using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using YG;
using Zenject;

namespace stickman_sniper.Purchases
{
    public interface IPurchaseService
    {
        IObservable<string> OnPurchaseComplete { get; }
        IReadOnlyReactiveProperty<bool> GetIsPurchasedReactiveProperty(string hash);
        void Purchase(string hash);

        //workaround SendMessage(int parameter gets corrupted)
        void SetNextBuyByRewardedAd(string hash);
    }

    internal class PurchaseService : IPurchaseService
    {
        private HashSet<string> _purchases = new();
        private Dictionary<string, ReactiveProperty<bool>> _cachedProperties = new();

        private Subject<string> _onPurchaseComplete = new();
        public IObservable<string> OnPurchaseComplete => _onPurchaseComplete;

        private string _nextBuy;

        public PurchaseService()
        {
            _purchases = YandexGame.savesData.purchases.ToHashSet<string>();

            YandexGame.RewardVideoEvent += OnRewardedVideoWatched;
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

        private void OnRewardedVideoWatched(int _)
        {
            try
            {
                //Debug.Log($"AAA video watched 1 {guid}");

                //string guidString = guid.ToString();

                Debug.Log($"AAA video watched guid parsed {_nextBuy}");

                if (_nextBuy != null)
                {
                    Purchase(_nextBuy);
                }

                Debug.Log("Bought throug AD");
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                ClearBuyByRewarded();
            }
        }

        public void Purchase(string hash)
        {
            _purchases.Add(hash);
            Save();
            GetIsPurchasedReactivePropertyInternal(hash).Value = true;

            _onPurchaseComplete.OnNext(hash);
        }

        private void Save()
        {
            YandexGame.savesData.purchases = _purchases.ToList();
            YandexGame.SaveProgress();
        }

        public void SetNextBuyByRewardedAd(string hash)
        {
            _nextBuy = hash;
        }

        public void ClearBuyByRewarded()
        {
            _nextBuy = null;
        }
    }

}