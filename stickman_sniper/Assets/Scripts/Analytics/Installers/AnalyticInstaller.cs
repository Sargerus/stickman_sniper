using System.Collections.Generic;
using Zenject;

namespace Analytics
{
    public class AnalyticInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            YandexMetricaAnalyticSystem yandexMetricaSystem = new();
            AnalyticsEventFactory.SetAnalyticSystems(new List<IAnalyticSystem>() { yandexMetricaSystem });
        }
    }
}
