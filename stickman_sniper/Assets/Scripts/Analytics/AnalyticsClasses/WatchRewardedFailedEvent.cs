using System.Collections.Generic;

namespace Analytics
{
    public class WatchRewardedFailedEvent : BaseAnalyticEvent
    {
        public WatchRewardedFailedEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}