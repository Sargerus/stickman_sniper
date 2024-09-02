public static class YandexMetricaConstants
{
    public static class Events
    {
        public static string level_loaded = nameof(level_loaded);
        public static string level_completed = nameof(level_completed);
        public static string level_failed = nameof(level_failed);
        public static string purchase = nameof(purchase);
        public static string watch_rewarded_start = nameof(watch_rewarded_start);
        public static string watch_rewarded_complete = nameof(watch_rewarded_complete);
        public static string watch_rewarded_failed = nameof(watch_rewarded_failed);
    }

    public static class Parameters
    {
        public static string level_number = nameof(level_number);
        public static string product_key = nameof(product_key);
    }
}