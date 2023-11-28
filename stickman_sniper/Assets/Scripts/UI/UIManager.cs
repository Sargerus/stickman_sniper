using Cysharp.Threading.Tasks;
using DWTools;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface IUiManager
{
    UniTask ShowWinPopup();
    UniTask ShowLosePopup();
    UniTask ShowRestoreAfterAd();
}

public class UIManager : MonoBehaviour, IUiManager
{
    [Inject] private TouchLocker _touchLocker;

    [SerializeField] private float _fadingSpeed;
    [SerializeField] private WinUI _winUI;
    [SerializeField] private LoseUI _loseUI;
    [SerializeField] private GameObject _restoreAfterAd;

    private FirstPersonController _firstPersonController;
    private CursorLocker _cursorLocker;
    private CameraProvider _mobileCameraProvider;

    [Inject]
    private void Constaruct(FirstPersonController firstPersonController, 
        CursorLocker cursorLocker,
        [Inject(Id ="mobile")] CameraProvider mobileCameraProvider)
    {
        _firstPersonController = firstPersonController;
        _cursorLocker = cursorLocker;
        _mobileCameraProvider = mobileCameraProvider;
    }

    private void OnEnable()
    {
        YG.YandexGame.CloseFullAdEvent += asd;
    }

    private void OnDisable()
    {
        YG.YandexGame.CloseFullAdEvent -= asd;
    }

    private void asd()
        => ShowRestoreAfterAd();

    private void LockTouches(bool isLock)
    {
        if (isLock)
            _touchLocker.Lock();
        else
            _touchLocker.Unlock();
    }

    public async UniTask ShowWinPopup()
    {
        _cursorLocker.Unlock();
        LockTouches(true);
        await Show(_winUI.GetComponent<CanvasGroup>(), () => { _winUI.Initialize(); LockTouches(false); });
    }

    public async UniTask ShowLosePopup()
    {
        _cursorLocker.Unlock();
        LockTouches(true);
        await Show(_loseUI.GetComponent<CanvasGroup>(), () => { _loseUI.Initialize(); LockTouches(false); });
    }
    
    public void RestoreGame()
    {
        _mobileCameraProvider.Camera.gameObject.SetActive(true);
        _firstPersonController.Freeze(false);
        _restoreAfterAd.SetActive(false);
        _cursorLocker.Lock();
    }
    public async UniTask ShowRestoreAfterAd()
    {
        _mobileCameraProvider.Camera.gameObject.SetActive(false);
        _firstPersonController.Freeze(true);
        _cursorLocker.Unlock();

        _restoreAfterAd.SetActive(true);
    }

    //public void ShowRestartUI()
    //{
    //    LockTouches(true);
    //
    //    _restartUI.SetScore(_scoreService.Score, 1000f);
    //    StartCoroutine(Show(_restartUI.GetComponent<CanvasGroup>(), () => LockTouches(false)));
    //}
    //
    //public void ShowTutorialUI(HappyFarmer.LevelValues _levelValues)
    //{
    //    LockTouches(true);
    //
    //    _tutorialUI.SetSize(_levelValues);
    //    StartCoroutine(Show(_tutorialUI.GetComponent<CanvasGroup>()));
    //    LockTouches(false);
    //}
    //
    //public void ShowScoreUI()
    //{
    //    StartCoroutine(Show(_scoreUI.GetComponent<CanvasGroup>()));
    //}
    //
    //public void HideScoreUI()
    //{
    //    StartCoroutine(Hide(_scoreUI.GetComponent<CanvasGroup>()));
    //}
    //
    //public void HideTutorialUI()
    //{
    //    StartCoroutine(Hide(_tutorialUI.GetComponent<CanvasGroup>()));
    //}

    private IEnumerator Hide(CanvasGroup cg, Action action = null)
    {
        while (cg.alpha > 0)
        {
            cg.alpha -= _fadingSpeed * Time.deltaTime;
            yield return null;
        }

        cg.gameObject.SetActive(false);
        action?.Invoke();
    }

    private IEnumerator Show(CanvasGroup cg, Action action = null)
    {
        cg.gameObject.SetActive(true);

        while (cg.alpha < 1)
        {
            cg.alpha += _fadingSpeed * Time.deltaTime;
            yield return null;
        }

        action?.Invoke();
    }
}