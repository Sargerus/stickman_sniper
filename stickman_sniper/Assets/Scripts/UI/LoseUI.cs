using TMPro;
using UnityEngine;
using Zenject;

public class LoseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _killedText;

    private ILevelLoader _levelLoader;
    private ILevelProgressObserver _levelProgressObserver;

    [Inject]
    private void Construct(ILevelLoader levelLoader, ILevelProgressObserver levelProgressObserver)
    {
        _levelLoader = levelLoader;
        _levelProgressObserver = levelProgressObserver;
    }

    public void Initialize()
    {
        _killedText.SetText($"{_levelProgressObserver.KilledEnemies}/{_levelProgressObserver.TotalEnemies}");
    }

    public void LoadLevel()
    {
        _levelLoader.LoadLevel();
    }
}
