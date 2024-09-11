using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

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

    internal class CurrencyService : ICurrencyService, IDisposable
    {
        private readonly ISaveService _saveService;

        private Dictionary<string, ReactiveProperty<float>> _currencies = new();

        public CurrencyService(ISaveService saveService)
        {
            _saveService = saveService;

            foreach (var c in _saveService.GetCurrencies())
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
            _saveService.SetCurrencies(_currencies.Select(g => new CurrencyEntity(g.Key, g.Value.Value)).ToList());
            _saveService.SaveProgress();
        }

        public void Dispose()
        {
            
        }
    }
}