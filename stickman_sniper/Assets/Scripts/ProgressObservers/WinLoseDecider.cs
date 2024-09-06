using Cysharp.Threading.Tasks;
using DWTools;
using InfimaGames.LowPolyShooterPack;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using YG;
using Zenject;

public class WinLoseDecider : IInitializable, IDisposable
{
    private readonly DWTools.Windows.IUIManager _uiManager;
    private readonly List<IProgressObserver> _progressObservers;
    private readonly CameraProvider _cameraProvider;
    private readonly IInputService _inputService;
    private readonly DiContainer _diContainer;
    private readonly Character _character;

    private CompositeDisposable _disposables = new();

    public WinLoseDecider(DWTools.Windows.IUIManager uiManager,
        List<IProgressObserver> progressObservers,
        [Inject(Id = "mobile")] CameraProvider cameraProvider,
        IInputService inputService, DiContainer diContainer,
        Character character)
    {
        _uiManager = uiManager;
        _progressObservers = progressObservers;
        _cameraProvider = cameraProvider;
        _inputService = inputService;
        _diContainer = diContainer;
        _character = character;
    }

    public void Initialize()
    {
        Observable.Merge(_progressObservers.SelectMany(g => new List<IReadOnlyReactiveProperty<bool>>() { g.Win, g.Lose }))
            .ToReactiveProperty().SkipLatestValueOnSubscribe().Subscribe(async x =>
        {
            HideMobile();
            _character.freeze = true;
            _inputService.DisableInput(true);
            var handler = await _uiManager.CreateWindow("game_over_ui", null, _diContainer);
            await handler.Show(false);

            if (YandexGame.savesData.levelsPassed >= 5 && YandexGame.EnvironmentData.reviewCanShow)
            {
                YandexGame.ReviewShow(true);
            }
            //_firstPersonController.Freeze(true);
            //_firstPersonController.ToggleScopeOff();
            //_uiManager.ShowWinPopup().Forget();
        }).AddTo(_disposables);

        //Observable.Merge(_progressObservers.Select(g => g.Win)).Where(h => h == true).Subscribe(_ =>
        //{
        //    HideMobile();
        //    _inputService.DisableInput();
        //    //_firstPersonController.Freeze(true);
        //    //_firstPersonController.ToggleScopeOff();
        //    //_uiManager.ShowWinPopup().Forget();
        //}).AddTo(_disposables);
        //
        //Observable.Merge(_progressObservers.Select(g => g.Lose)).Where(h => h == true).Subscribe(_ =>
        //{
        //    HideMobile();
        //    _inputService.DisableInput();
        //    //_firstPersonController.Freeze(true);
        //    //_firstPersonController.ToggleScopeOff();
        //    //_uiManager.ShowLosePopup().Forget();
        //}).AddTo(_disposables);
    }

    private void HideMobile()
    {
        _cameraProvider.Camera.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}