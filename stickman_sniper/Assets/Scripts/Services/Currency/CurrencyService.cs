using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using YG;

namespace stickman_sniper.Currency
{
    public interface ICurrencyService
    {
        IReadOnlyReactiveProperty<float> CreateCurrency(string key);
        IReadOnlyReactiveProperty<float> GetCurrency(string key);
        void AddCurrency(string key, float value);
    }

    [Serializable]
    public class CurrencyEntity
    {
        public string Key;
        public float Value;

        public CurrencyEntity(string key, float value)
        {
            Key = key;
            Value = value;
        }
    }

    internal class CurrencyService : ICurrencyService, IInitializable, IDisposable
    {
        private Dictionary<string, ReactiveProperty<float>> _currencies = new();

        public void Initialize()
        {
            foreach (var c in YandexGame.savesData.currencies)
            {
                _currencies.Add(c.Key, new(c.Value));
            }
        }

        public void AddCurrency(string key, float value)
        {
            if (!_currencies.TryGetValue(key, out var property))
                return;

            property.Value += value;
            Save();
        }

        public IReadOnlyReactiveProperty<float> CreateCurrency(string key)
        {
            if (!_currencies.TryGetValue(key, out var property))
            {
                property = new(0);
                _currencies.Add(key, property);
            }

            Save();

            return property;
        }

        public IReadOnlyReactiveProperty<float> GetCurrency(string key)
        {
            ReactiveProperty<float> result = null;
            _currencies.TryGetValue(key, out result);
            return result;
        }

        private void Save()
        {
            YandexGame.savesData.currencies = _currencies.Select(g => new CurrencyEntity(g.Key, g.Value.Value)).ToList();
            YandexGame.SaveProgress();
        }

        public void Dispose()
        {
            
        }
    }
}