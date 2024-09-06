using DG.Tweening;
using UnityEngine;

namespace StickmanSniper.Utilities
{
    public class ScaleOverTime : MonoBehaviour
    {
        [SerializeField] private Vector3 _minScale = Vector3.one;
        [SerializeField] private float _toMinScaleTime = 5f;
        [SerializeField] private Vector3 _maxScale = Vector3.one * 1.2f;
        [SerializeField] private float _toMaxScaleTime = 5f;

        private Tween _sequence;

        void Start()
        {
            transform.localScale = _minScale;

            _sequence = DOTween.Sequence()
                .Append(transform.DOScale(_maxScale, _toMinScaleTime))
                .Append(transform.DOScale(_minScale, _toMaxScaleTime))
                .SetLoops(-1);
            //.SetEase(Ease.OutQuad);
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}