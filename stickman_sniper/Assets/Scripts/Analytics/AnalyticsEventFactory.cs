using System.Collections.Generic;

namespace Analytics
{
    public static class AnalyticsEventFactory
    {
        private static IReadOnlyList<IAnalyticSystem> _analyticSystems;

        public static void SetAnalyticSystems(IReadOnlyList<IAnalyticSystem> analyticSystems)
            => _analyticSystems = analyticSystems;

        public static LevelLoadedEvent GetLevelLoadedEvent()
        {
            return new(YandexMetricaConstants.Events.level_loaded, _analyticSystems);
        }

        public static LevelCompletedEvent GetLevelCompletedEvent()
        {
            return new(YandexMetricaConstants.Events.level_completed, _analyticSystems);
        }

        public static LevelFailedEvent GetLevelFailedEvent()
        {
            return new(YandexMetricaConstants.Events.level_failed, _analyticSystems);
        }

        public static PurchaseEvent GetPurchaseEvent()
        {
            return new(YandexMetricaConstants.Events.purchase, _analyticSystems);
        }

        public static WatchRewardedStartEvent GetWatchRewardedStartEvent()
        {
            return new(YandexMetricaConstants.Events.watch_rewarded_start, _analyticSystems);
        }

        public static WatchRewardedCompleteEvent GetWatchRewardedCompleteEvent()
        {
            return new(YandexMetricaConstants.Events.watch_rewarded_complete, _analyticSystems);
        }

        public static WatchRewardedFailedEvent GetWatchRewardedFailedEvent()
        {
            return new(YandexMetricaConstants.Events.watch_rewarded_failed, _analyticSystems);
        }
    }
}