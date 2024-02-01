using Cysharp.Threading.Tasks;
using DWTools;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using YG;
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
    [SerializeField] private GameObject _levelLabel;
    [SerializeField] private TMP_Text _levelText;

    private FirstPersonController _firstPersonController;
    private CursorLocker _cursorLocker;
    private CameraProvider _mobileCameraProvider;
    private CameraProvider _weaponCameraProvider;
    private ILevelProgressObserver _levelProgressObserver;

    [Inject]
    private void Construct(FirstPersonController firstPersonController,
        CursorLocker cursorLocker,
        ILevelProgressObserver levelProgressObserver,
        [Inject(Id = "mobile")] CameraProvider mobileCameraProvider,
        [Inject(Id = "weapon")] CameraProvider weaponCameraProvider)
    {
        _firstPersonController = firstPersonController;
        _cursorLocker = cursorLocker;
        _levelProgressObserver = levelProgressObserver;
        _mobileCameraProvider = mobileCameraProvider;
        _weaponCameraProvider = weaponCameraProvider;
    }

    private void OnEnable()
    {
        if (!YandexGame.nowAdsShow)
        {
            _cursorLocker.Lock();
        }

        YG.YandexGame.CloseFullAdEvent += asd;
        _levelText.SetText($" {YandexGame.savesData.levelsPassed + 1}");
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
    }
    public async UniTask ShowRestoreAfterAd()
    {
        _mobileCameraProvider.Camera.gameObject.SetActive(false);
        _firstPersonController.Freeze(true);
        _cursorLocker.Unlock();

        _restoreAfterAd.SetActive(true);
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
}