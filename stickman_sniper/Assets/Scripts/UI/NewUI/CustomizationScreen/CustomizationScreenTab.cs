using System;
using TMPro;
using UniRx;
using UnityEngine;

public class CustomizationScreenTab : MonoBehaviour
{
    [SerializeField] private TMP_Text tabName;
    [SerializeField] private UnityEngine.UI.Button button;

    private CompositeDisposable _disposables = new();

    public CustomizationScreenTab SetTabName(string name) 
    {
        tabName.SetText(name);
        return this;
    }

    public CustomizationScreenTab SetOnClickHandler(string key, IObserver<string> onClickHandler)
    {
        button.OnClickAsObservable().SubscribeWithState2(key, onClickHandler, (_, key, onClickHandler) =>
        {
            onClickHandler.OnNext(key);
        }).AddTo(_disposables);

        return this;
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
