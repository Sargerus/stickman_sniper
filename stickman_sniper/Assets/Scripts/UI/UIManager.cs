using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public interface IUiManager
{
    UniTask ShowWinPopup();
    UniTask ShowLosePopup();
}

public class UIManager : MonoBehaviour, IUiManager
{
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private float _fadingSpeed;

    private void LockTouches(bool isLock)
    {
        _eventSystem.gameObject.SetActive(!isLock);
    }

    public async UniTask ShowWinPopup()
    {
        throw new NotImplementedException();
    }

    public async UniTask ShowLosePopup()
    {
        throw new NotImplementedException();
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