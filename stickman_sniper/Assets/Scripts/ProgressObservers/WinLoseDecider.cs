using Cysharp.Threading.Tasks;
using DWTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

public class WinLoseDecider : IInitializable, IDisposable
{
    private readonly IUiManager _uiManager;
    private readonly List<IProgressObserver> _progressObservers;
    private readonly FirstPersonController _firstPersonController;
    private readonly CameraProvider _cameraProvider;

    private CompositeDisposable _disposables = new();

    public WinLoseDecider(IUiManager uiManager, List<IProgressObserver> progressObservers,
        FirstPersonController firstPersonController, [Inject(Id ="mobile")] CameraProvider cameraProvider)
    {
        _uiManager = uiManager;
        _progressObservers = progressObservers;
        _firstPersonController = firstPersonController;
        _cameraProvider = cameraProvider;
    }

    public void Initialize()
    {
        Observable.Merge(_progressObservers.Select(g => g.Win)).Where(h => h == true).Subscribe(_ =>
        {
            HideMobile();
            _firstPersonController.Freeze(true);
            _firstPersonController.ToggleScopeOff();
            _uiManager.ShowWinPopup().Forget();
        }).AddTo(_disposables);

        Observable.Merge(_progressObservers.Select(g => g.Lose)).Where(h => h == true).Subscribe(_ =>
        {
            HideMobile();
            _firstPersonController.Freeze(true);
            _firstPersonController.ToggleScopeOff();
            _uiManager.ShowLosePopup().Forget();
        }).AddTo(_disposables);
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