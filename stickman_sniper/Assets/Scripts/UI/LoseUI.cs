using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _killedText;
    [SerializeField] private Button _button;

    private ILevelLoader _levelLoader;
    private ILevelProgressObserver _levelProgressObserver;
    private IDisposable _clickDisposable;

    [Inject]
    private void Construct(ILevelLoader levelLoader, ILevelProgressObserver levelProgressObserver)
    {
        _levelLoader = levelLoader;
        _levelProgressObserver = levelProgressObserver;
    }

    public void Initialize()
    {
        _killedText.SetText($"{_levelProgressObserver.KilledEnemies}/{_levelProgressObserver.TotalEnemies}");

        _clickDisposable = _button.OnClickAsObservable().Subscribe(_ =>
        {
            _clickDisposable.Dispose();
            _levelLoader.LoadLevel();
        });
    }
}
