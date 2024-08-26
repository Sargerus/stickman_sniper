using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISpriteSwaper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image defaultImage;
    [SerializeField] private Image highlightedImage;

    private Image _currentImage;
    private Tween _swapTween;

    public void OnPointerClick(PointerEventData eventData)
    {
        _swapTween?.Kill(true);
        _swapTween = DOTween.Sequence()
            .Append(_currentImage.DOFade(0.8f, 0.1f))
            .Append(_currentImage.DOFade(1f, 0.1f));

        var button = transform.GetComponentInParent<Button>();
        if (button != null)
        {
            ExecuteEvents.Execute<IPointerClickHandler>(button.gameObject, eventData, (x, y) => x.OnPointerClick(eventData));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _swapTween?.Kill();
        _currentImage = highlightedImage;
        _swapTween = highlightedImage.DOFade(1, 0.4f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _swapTween?.Kill();
        _currentImage = defaultImage;
        _swapTween = highlightedImage.DOFade(0, 0.4f);
    }

    private void OnDisable()
    {
        _swapTween?.Kill();
    }
}
