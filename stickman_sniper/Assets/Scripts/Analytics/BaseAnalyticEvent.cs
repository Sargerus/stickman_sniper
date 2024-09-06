using Sirenix.Utilities;
using System.Collections.Generic;

namespace Analytics
{
    public abstract class BaseAnalyticEvent
    {
        protected string _eventName;
        protected Dictionary<string, string> _param = new();

        private readonly IReadOnlyList<IAnalyticSystem> _systems;

        public BaseAnalyticEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
        {
            _eventName = eventName;
            _systems = systems;
        }

        public BaseAnalyticEvent AddLevelNumber(int levelNumber)
        {
            _param.Add(YandexMetricaConstants.Parameters.level_number, levelNumber.ToString());
            return this;
        }

        public BaseAnalyticEvent AddProductKey(string productKey)
        {
            if (string.IsNullOrEmpty(productKey))
                productKey = string.Empty;

            _param.Add(YandexMetricaConstants.Parameters.product_key, productKey);
            return this;
        }

        public virtual void Send()
        {
            _systems.ForEach(g => g.Send(_eventName, _param));
        }
    }
}