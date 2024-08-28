using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISpriteSwaper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image defaultImage;
    [SerializeField] private Image highlightedImage;
    [SerializeField] private Image selectedImage;

    private Image _currentImage;
    private Tween _swapTween;
    private bool _isSelected;

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
        if (_isSelected)
            return;

        _swapTween?.Kill(true);
        _currentImage = highlightedImage;
        _swapTween = highlightedImage.DOFade(1, 0.4f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected)
            return;

        _swapTween?.Kill(true);
        _currentImage = defaultImage;
        _swapTween = highlightedImage.DOFade(0, 0.4f);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;

        if (isSelected)
        {
            _swapTween?.Kill(true);
            _currentImage = selectedImage;
            _swapTween = DOTween.Sequence().Append(highlightedImage.DOFade(0,0.4f)).Join(selectedImage.DOFade(1, 0.4f));
        }
        else
        {
            _swapTween?.Kill();
            _currentImage = defaultImage;
            _swapTween = selectedImage.DOFade(0, 0.4f);
        }        
    }

    private void OnDisable()
    {
        _isSelected = false;
        _swapTween?.Kill();
    }
}
