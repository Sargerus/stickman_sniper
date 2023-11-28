using DWTools;
using UnityEngine;
using Zenject;

public class RestoreStateAfterAdLock : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _gameStart;
    [SerializeField] private GameObject _restoreAfterAd;

    private ILevelLoader _levelLoader;
    private CursorLocker _cursorLocker;

    [Inject]
    public void Construct(ILevelLoader levelLoader, CursorLocker cursorLocker)
    {
        _levelLoader = levelLoader;
        _cursorLocker = cursorLocker;
    }

    public void ShowRestoreUI()
    {
        _camera.gameObject.SetActive(true);
        _restoreAfterAd.SetActive(true);
    }

    public void Restore()
    {
        _camera.gameObject.SetActive(false);
        _restoreAfterAd.SetActive(false);
        _cursorLocker.Lock();
    }

    public void StartGame()
    {
        _camera.gameObject.SetActive(false);
        _gameStart.SetActive(false);
        _cursorLocker.Lock();
        _levelLoader.LoadLevel();
    }
}
