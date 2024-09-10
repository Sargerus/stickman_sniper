using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class CustomizationScreenTab : MonoBehaviour
{
    [SerializeField] private TMP_Text tabName;
    [SerializeField] private UnityEngine.UI.Button button;

    [SerializeField, BoxGroup("Sprite")] private Image image;
    [SerializeField, BoxGroup("Sprite")] private List<TabToSprite> tabToSprites;

    private CompositeDisposable _disposables = new();

    public CustomizationScreenTab SetTab(AttachmentsTab tab)
    {
        image.sprite = tabToSprites.FirstOrDefault(g => g.Tab == tab)?.Sprite;
        return this;
    }

    public CustomizationScreenTab SetTabName(TranslationData trData)
    {
        string name = trData.GetTranslation(YandexGame.lang);

        if (tabName != null)
            tabName.SetText(name);

        return this;
    }

    public CustomizationScreenTab SetOnClickHandler(AttachmentsTab tab, IObserver<AttachmentsTab> onClickHandler)
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

[Serializable]
public class TabToSprite
{
    public AttachmentsTab Tab;
    public Sprite Sprite;
}