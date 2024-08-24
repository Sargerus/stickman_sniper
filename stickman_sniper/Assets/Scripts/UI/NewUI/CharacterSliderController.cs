using DWTools.RPG;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[RequireComponent(typeof(Slider))]
public class CharacterSliderController : MonoBehaviour
{
    [SerializeField] private CharacterStat statToObserve;

    private Character _character;
    private Slider _slider;
    private IDisposable _disposable;

    public void Init(Character character)
    {
        _character = character;
        _slider = GetComponent<Slider>();

        if (_character.TryGetReactiveStat(statToObserve, out var currentStat) &&
            _character.TryGetBaseStat(statToObserve, out var baseStatEntity))
        {
            _disposable = currentStat.Subscribe(x =>
            {
                _slider.value = x / baseStatEntity.Value;
            });
        }
    }

    private void OnDestroy()
    {
        _disposable?.Dispose();
    }
}
