using DWTools.Windows;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using DWTools;

public class GameOverUI : BaseWindow
{
    [SerializeField] private TMP_Text resultTextWin;
    [SerializeField] private TMP_Text resultTextLose;
    [SerializeField] private TMP_Text _killedText;
    [SerializeField] private GameObject _star1;
    [SerializeField] private TMP_Text _bulletsText;
    [SerializeField] private GameObject _star2;
    [SerializeField] private TMP_Text _restartsText;
    [SerializeField] private GameObject _star3;
    [SerializeField] private GameObject earnCoins;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartLevelButton;

    private ILevelProgressObserver _levelProgressObserver;
    private ILoadingManagerHolder _loadingManagerHolder;
    private Character _character;
    private ILevelLoader _levelLoader;
    private CursorLocker _cursorLocker;

    private CompositeDisposable _disposables = new();

    [Inject]
    public void Construct(ILevelProgressObserver levelProgressObserver,
        Character character, ILoadingManagerHolder loadingManagerHolder, ILevelLoader levelLoader,
        CursorLocker cursorLocker)
    {
        _levelProgressObserver = levelProgressObserver;
        _character = character;
        _loadingManagerHolder = loadingManagerHolder;
        _levelLoader = levelLoader;
        _cursorLocker = cursorLocker;
    }

    protected override async UniTask BeforeShow(CancellationToken token)
    {
        if (_levelProgressObserver.Win.Value)
        {
            ShowWinUI();
        }
        else
        {
            ShowLoseUI();
        }
    }

    protected override async UniTask AfterShow(CancellationToken token)
    {
        _cursorLocker.Unlock();
    }

    private void ShowWinUI()
    {
        var equippedWeapon = _character.GetInventory().GetEquipped();
        int currentAmmunition = equippedWeapon.GetAmmunitionCurrent();

        resultTextWin.gameObject.SetActive(true);
        earnCoins.SetActive(true);
        resultTextLose.gameObject.SetActive(false);

        _killedText.SetText($"{_levelProgressObserver.KilledEnemies}/{_levelProgressObserver.TotalEnemies}");
        StartCoroutine(ShowStar(_star1.GetComponent<Image>()));

        _bulletsText.SetText($"{currentAmmunition}");
        StartCoroutine(ShowStar(_star2.GetComponent<Image>(), 0.4f));
        
        _restartsText.SetText($"{50}");
        StartCoroutine(ShowStar(_star3.GetComponent<Image>(), 0.8f));

        _nextLevelButton.gameObject.SetActive(true);
        _restartLevelButton.gameObject.SetActive(false);

        _nextLevelButton.OnClickAsObservable().Subscribe(_ =>
        {
            CloseManaged(true);
            _loadingManagerHolder.LoadingManager.LoadMainMenuState();
        }).AddTo(_disposables);
    }

    private void ShowLoseUI()
    {
        var equippedWeapon = _character.GetInventory().GetEquipped();
        int currentAmmunition = equippedWeapon.GetAmmunitionCurrent();

        resultTextWin.gameObject.SetActive(false);
        earnCoins.SetActive(false);
        resultTextLose.gameObject.SetActive(true);

        _killedText.SetText($"{_levelProgressObserver.KilledEnemies}/{_levelProgressObserver.TotalEnemies}");
        _bulletsText.SetText($"{currentAmmunition}");

        _nextLevelButton.gameObject.SetActive(false);
        _restartLevelButton.gameObject.SetActive(true);

        _restartLevelButton.OnClickAsObservable().Subscribe(_ =>
        {
            CloseManaged(true);
            _loadingManagerHolder.LoadingManager.LoadMainMenuState();
        }).AddTo(_disposables);
    }

    private IEnumerator ShowStar(Image image, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        image.gameObject.transform.localScale *= 1.5f;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

        float limit = 0.8f;
        float counter = 0;
        image.gameObject.SetActive(true);

        while (counter < limit)
        {
            float coef = (Time.deltaTime / limit);
            image.gameObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f) * coef;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + coef);

            counter += Time.deltaTime;
            yield return null;
        }
    }

    protected override async UniTask BeforeHide(CancellationToken token)
    {
        StopAllCoroutines();
        _disposables.Clear();
    }
}
