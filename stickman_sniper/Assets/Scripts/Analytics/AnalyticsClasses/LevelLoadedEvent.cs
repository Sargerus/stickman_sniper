using System.Collections.Generic;

namespace Analytics
{
    public class LevelLoadedEvent : BaseAnalyticEvent
    {
        public LevelLoadedEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}