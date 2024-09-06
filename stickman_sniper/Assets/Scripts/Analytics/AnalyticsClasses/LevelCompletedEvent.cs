using System.Collections.Generic;

namespace Analytics
{
    public class LevelCompletedEvent : BaseAnalyticEvent
    {
        public LevelCompletedEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}