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
}

public class UIManager : MonoBehaviour, IUiManager
{
    [Inject] private TouchLocker _touchLocker;

    [SerializeField] private float _fadingSpeed;
    [SerializeField] private WinUI _winUI;
    [SerializeField] private LoseUI _loseUI;

    private void LockTouches(bool isLock)
    {
        if (isLock)
            _touchLocker.Lock();
        else
            _touchLocker.Unlock();
    }

    public async UniTask ShowWinPopup()
    {
        StaticCursorLocker.Unlock();
        LockTouches(true);
        await Show(_winUI.GetComponent<CanvasGroup>(), () => { _winUI.Initialize(); LockTouches(false); });
    }

    public async UniTask ShowLosePopup()
    {
        StaticCursorLocker.Unlock();
        LockTouches(true);
        await Show(_loseUI.GetComponent<CanvasGroup>(), () => { _loseUI.Initialize(); LockTouches(false); });
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