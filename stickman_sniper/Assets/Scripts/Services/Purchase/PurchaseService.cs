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

    internal partial class PurchaseService : IPurchaseService, IDisposable
    {
        private readonly ISaveService _saveService;

        private HashSet<string> _purchases = new();
        private Dictionary<string, ReactiveProperty<bool>> _cachedProperties = new();
        private HashToProductKeyMapper _hashToProductKeyMapper;

        private Subject<string> _onPurchaseComplete = new();
        public IObservable<string> OnPurchaseComplete => _onPurchaseComplete;

        private string _nextBuy;

        public PurchaseService(ISaveService saveService, HashToProductKeyMapper hashToProductKeyMapper)
        {
            _saveService = saveService;
            _hashToProductKeyMapper = hashToProductKeyMapper;
            _hashToProductKeyMapper.FilLCache();
            _purchases = _saveService.GetPurchases().ToHashSet<string>();
            Subscribe();
        }

        private void Subscribe()
        {
            YandexGame.RewardVideoEvent += OnRewardedVideoWatched;
            YandexGame.OpenFullAdEvent += OpenFullAdEvent;
            YandexGame.CloseFullAdEvent += CloseFullAdEvent;
            YandexGame.ErrorFullAdEvent += ErrorFullAdEvent;
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
                if (_nextBuy != null)
                {
                    Purchase(_nextBuy);
                }
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

        private void OpenFullAdEvent()
        {
            string productKey = _hashToProductKeyMapper.GetProductKeyByHash(_nextBuy);
            AnalyticsEventFactory.GetWatchRewardedStartEvent().AddProductKey(productKey);
        }

        private void CloseFullAdEvent()
        {
            string productKey = _hashToProductKeyMapper.GetProductKeyByHash(_nextBuy);
            AnalyticsEventFactory.GetWatchRewardedCompleteEvent().AddProductKey(productKey);
        }

        private void ErrorFullAdEvent()
        {
            string productKey = _hashToProductKeyMapper.GetProductKeyByHash(_nextBuy);
            AnalyticsEventFactory.GetWatchRewardedFailedEvent().AddProductKey(productKey);
        }

        public void Purchase(string hash)
        {
            string productKey = _hashToProductKeyMapper.GetProductKeyByHash(hash);
            AnalyticsEventFactory.GetPurchaseEvent().AddProductKey(productKey).Send();

            _purchases.Add(hash);
            Save();
            GetIsPurchasedReactivePropertyInternal(hash).Value = true;

            _onPurchaseComplete.OnNext(hash);
        }

        private void Save()
        {
            _saveService.SetPurchases(_purchases.ToList());
            _saveService.SaveProgress();
        }

        public void SetNextBuyByRewardedAd(string hash)
        {
            _nextBuy = hash;
        }

        public void ClearBuyByRewarded()
        {
            _nextBuy = null;
        }

        public void Dispose()
        {
            YandexGame.RewardVideoEvent -= OnRewardedVideoWatched;
            YandexGame.OpenFullAdEvent -= OpenFullAdEvent;
            YandexGame.CloseFullAdEvent -= CloseFullAdEvent;
        }
    }

}