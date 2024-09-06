using System.Collections.Generic;

namespace Analytics
{
    public interface IAnalyticSystem
    {
        void Send(string eventName, Dictionary<string, string> param);
    }
}
