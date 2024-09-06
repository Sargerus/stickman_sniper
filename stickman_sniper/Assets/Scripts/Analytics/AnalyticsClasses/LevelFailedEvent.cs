using System.Collections.Generic;

namespace Analytics
{
    public class LevelFailedEvent : BaseAnalyticEvent
    {
        public LevelFailedEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}