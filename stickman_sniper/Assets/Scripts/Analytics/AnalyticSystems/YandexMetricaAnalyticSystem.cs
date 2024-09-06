using Analytics;
using System.Collections.Generic;
using YG;

namespace Analytics
{
    public class YandexMetricaAnalyticSystem : IAnalyticSystem
    {
        public void Send(string eventName, Dictionary<string, string> param)
        {
            YandexMetrica.Send(eventName, param);
        }
    }
}