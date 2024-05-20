using DWTools;
using System;
using TMPro;
using UniRx;
using UnityEngine;

public class CustomizationScreenShopCell : MonoBehaviour, IPooledItem<CustomizationScreenShopCell>
{
    [SerializeField] private Transform bgImageParent;
    [SerializeField] private Transform itemImageParent;
    [SerializeField] private TMP_Text text;
    [SerializeField] private UnityEngine.UI.Button button;

    private GameObject _bgImage;
    private GameObject _itemImage;
    private CompositeDisposable _disposables = new();

    public CustomizationScreenShopCell Item => this;
    public IPool<CustomizationScreenShopCell> Pool { get; set; }

    public CustomizationScreenShopCell SetBackground(GameObject image)
    {
        _bgImage = image;
        image.transform.SetParent(bgImageParent);
        return this;
    }

    public CustomizationScreenShopCell SetItemImage(GameObject image)
    {
        _itemImage = image;
        image.transform.SetParent(itemImageParent);
        return this;
    }

    public CustomizationScreenShopCell SetText(string text)
    {
        this.text.SetText(text);
        return this;
    }

    public CustomizationScreenShopCell SetOnClickHandler(string key, IObserver<string> onClickHandler)
    {
        button.OnClickAsObservable().SubscribeWithState2(key, onClickHandler, (_, key, onClickHandler) =>
        {
            onClickHandler.OnNext(key);
        }).AddTo(_disposables);

        return this;
    }

    public CustomizationScreenShopCell ContinueWith(Action<CustomizationScreenShopCell> action)
    {
        action?.Invoke(this);
        return this;
    }

    public void ReturnToPool()
    {
        text.SetText(string.Empty);

        if (_bgImage != null)
            Destroy(_bgImage);
        if (_itemImage != null)
            Destroy(_itemImage);

        gameObject.SetActive(false);
        Pool.Add(this);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
