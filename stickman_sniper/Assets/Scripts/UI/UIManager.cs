using Counters;
using Cysharp.Threading.Tasks;
using DWTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using YG;
using Zenject;

public interface IUiManager
{
    UniTask ShowWinPopup();
    UniTask ShowLosePopup();
}

public class UIManager : MonoBehaviour, IUiManager
{
    [Inject] private TouchLocker _touchLocker;

    [SerializeField] private float _fadingSpeed;
    [SerializeField] private WinUI _winUI;
    [SerializeField] private LoseUI _loseUI;
    [SerializeField] private GameObject _restoreAfterAd;
    [SerializeField] private GameObject _levelLabel;
    [SerializeField] private TMP_Text _levelText;

    private FirstPersonController _firstPersonController;
    private CursorLocker _cursorLocker;
    private CameraProvider _mobileCameraProvider;
    private CameraProvider _weaponCameraProvider;
    private ILevelProgressObserver _levelProgressObserver;
    private List<IProgressObserver> _progressObservers;
    private IInputService _inputService;
    private SessionCounter _sessionCounter;

    [Inject]
    private void Construct(FirstPersonController firstPersonController,
        CursorLocker cursorLocker,
        ILevelProgressObserver levelProgressObserver,
        List<IProgressObserver> progressObservers,
        IInputService inputService,
        SessionCounter sessionCounter,
        [Inject(Id = "mobile")] CameraProvider mobileCameraProvider,
        [Inject(Id = "weapon")] CameraProvider weaponCameraProvider)
    {
        _firstPersonController = firstPersonController;
        _cursorLocker = cursorLocker;
        _levelProgressObserver = levelProgressObserver;
        _progressObservers = progressObservers;
        _inputService = inputService;
        _sessionCounter = sessionCounter;
        _mobileCameraProvider = mobileCameraProvider;
        _weaponCameraProvider = weaponCameraProvider;
    }

    private void OnEnable()
    {
        if (!YandexGame.nowAdsShow)
        {
            _cursorLocker.Lock();
        }
        else
        {
            OpenFullAdEvent();
        }

        YandexGame.OpenFullAdEvent += OpenFullAdEvent;
        YandexGame.CloseFullAdEvent += CloseFullAdEvent;
        YandexGame.ErrorFullAdEvent += ErrorFullAdEvent;

        _levelText.SetText($" {YandexGame.savesData.levelsPassed + 1}");
    }

    private void OnDisable()
    {
        YandexGame.OpenFullAdEvent -= OpenFullAdEvent;
        YandexGame.CloseFullAdEvent -= CloseFullAdEvent;
        YandexGame.ErrorFullAdEvent -= ErrorFullAdEvent;
    }

    private void LockTouches(bool isLock)
    {
        if (isLock)
            _touchLocker.Lock();
        else
            _touchLocker.Unlock();
    }

    public async UniTask ShowWinPopup()
    {
        _sessionCounter.LevelsPassed++;

        if (_sessionCounter.LevelsPassed >=3 && YandexGame.EnvironmentData.reviewCanShow)
        {
            YandexGame.ReviewShow(true);
        }

        _levelLabel.gameObject.SetActive(false);
        _cursorLocker.Unlock();
        LockTouches(true);
        _winUI.gameObject.SetActive(true);
        await Show(_winUI.GetComponent<CanvasGroup>(), () => { _winUI.Initialize(); LockTouches(false); });
    }

    public async UniTask ShowLosePopup()
    {
        YandexGame.FullscreenShow();
        _levelLabel.gameObject.SetActive(false);
        _cursorLocker.Unlock();
        LockTouches(true);
        _loseUI.gameObject.SetActive(true);
        await Show(_loseUI.GetComponent<CanvasGroup>(), () => { _loseUI.Initialize(); LockTouches(false); });
    }

    public void RestoreGame()
    {
        if (_levelProgressObserver.Lose.Value == true)
        {
            _restoreAfterAd.SetActive(false);
            return;
        }

        if (YandexGame.Device.ToDevice() == Device.Mobile)
        {
            _mobileCameraProvider.Camera.gameObject.SetActive(true);
        }

        _firstPersonController.Freeze(false);
        _restoreAfterAd.SetActive(false);
        _cursorLocker.Lock();
        _inputService.EnableInput();
    }

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

    //---=Yandex Ad=---
    public void OpenFullAdEvent()
    {
        _firstPersonController.Freeze(true);
        _restoreAfterAd.SetActive(true);
        _cursorLocker.Unlock();
        _inputService.DisableInput(true);
    }

    public void CloseFullAdEvent(string wasShown)
    {
        if (!wasShown.Equals("true"))
            return;

        _mobileCameraProvider.Camera.gameObject.SetActive(false);
        _firstPersonController.Freeze(true);
        _cursorLocker.Unlock();
        _inputService.DisableInput(true);

        if (wasShown.Equals("true") && _progressObservers.Any(g => g.Lose.Value == true))
            return;

        _restoreAfterAd.SetActive(true);
    }

    public void ErrorFullAdEvent(string error)
    {
        //nothing
    }
}