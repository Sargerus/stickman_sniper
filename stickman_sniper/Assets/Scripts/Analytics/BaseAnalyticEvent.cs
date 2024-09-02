using System.Collections.Generic;
using YG;

public abstract class BaseAnalyticEvent
{
    protected string _eventName;
    protected Dictionary<string, string> _param = new();

    public BaseAnalyticEvent(string eventName)
    {
        _eventName = eventName;
    }

    public BaseAnalyticEvent AddLevelNumber(int levelNumber)
    {
        _param.Add(YandexMetricaConstants.Parameters.level_number, levelNumber.ToString());
        return this;
    }

    public BaseAnalyticEvent AddProductKey(string productKey)
    {
        _param.Add(YandexMetricaConstants.Parameters.product_key, productKey);
        return this;
    }

    public virtual void Send()
    {
        YandexMetrica.Send(_eventName, _param);
    }
}