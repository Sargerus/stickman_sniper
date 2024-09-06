using System.Collections.Generic;

namespace Analytics
{
    public class WatchRewardedStartEvent : BaseAnalyticEvent
    {
        public WatchRewardedStartEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}