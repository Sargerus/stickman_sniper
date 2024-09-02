public static class AnalyticsEventFactory
{
    public static LevelLoadedEvent GetLevelLoadedEvent()
    {
        return new(YandexMetricaConstants.Events.level_loaded);
    }

    public static LevelCompletedEvent GetLevelCompletedEvent()
    {
        return new(YandexMetricaConstants.Events.level_completed);
    }

    public static LevelFailedEvent GetLevelFailedEvent()
    {
        return new(YandexMetricaConstants.Events.level_failed);
    }

    public static PurchaseEvent GetPurchaseEvent()
    {
        return new(YandexMetricaConstants.Events.purchase);
    }

    public static WatchRewardedStartEvent GetWatchRewardedStartEvent()
    {
        return new(YandexMetricaConstants.Events.watch_rewarded_start);
    }

    public static WatchRewardedCompleteEvent GetWatchRewardedCompleteEvent()
    {
        return new(YandexMetricaConstants.Events.watch_rewarded_complete);
    }

    public static WatchRewardedFailedEvent GetWatchRewardedFailedEvent()
    {
        return new(YandexMetricaConstants.Events.watch_rewarded_failed);
    }
}