using System;
using TMPro;
using UniRx;
using UnityEngine;
using YG;

public class CustomizationScreenAllWeaponsTab : MonoBehaviour
{
    [SerializeField] private TMP_Text tabName;
    [SerializeField] private UnityEngine.UI.Button button;

    private CompositeDisposable _disposables = new();

    public CustomizationScreenAllWeaponsTab SetTabName(TranslationData trData)
    {
        string name = trData.GetTranslation(YandexGame.lang);

        if (tabName != null)
            tabName.SetText(name);

        return this;
    }

    public CustomizationScreenAllWeaponsTab SetOnClickHandler(InventoryTab tab, IObserver<InventoryTab> onClickHandler)
    {
        button.OnClickAsObservable().SubscribeWithState2(tab, onClickHandler, (_, key, onClickHandler) =>
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
