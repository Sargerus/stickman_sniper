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

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void Init(Character character)
    {
        _character = character;

        if (character.TryGetReactiveStat(statToObserve, out var currentStat) &&
            character.TryGetBaseStat(statToObserve, out var baseStatEntity))
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
