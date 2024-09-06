using System.Collections.Generic;

namespace Analytics
{
    public class WatchRewardedCompleteEvent : BaseAnalyticEvent
    {
        public WatchRewardedCompleteEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}