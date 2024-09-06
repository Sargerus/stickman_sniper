using System.Collections.Generic;

namespace Analytics
{
    public class PurchaseEvent : BaseAnalyticEvent
    {
        public PurchaseEvent(string eventName, IReadOnlyList<IAnalyticSystem> systems)
            : base(eventName, systems)
        {
        }
    }
}