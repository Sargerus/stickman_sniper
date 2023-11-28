using DWTools;
using UnityEngine;
using Zenject;

public class RestoreStateAfterAdLock : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _gameStart;
    [SerializeField] private GameObject _restoreAfterAd;

    private ILevelLoader _levelLoader;

    [Inject]
    public void Construct(ILevelLoader levelLoader)
    {
        _levelLoader = levelLoader;
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
        StaticCursorLocker.Lock();
    }

    public void StartGame()
    {
        _camera.gameObject.SetActive(false);
        _gameStart.SetActive(false);
        StaticCursorLocker.Lock();
        _levelLoader.LoadLevel();
    }
}
