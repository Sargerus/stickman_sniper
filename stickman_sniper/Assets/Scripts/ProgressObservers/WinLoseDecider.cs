using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;

public class WinLoseDecider : IInitializable, IDisposable
{
    private readonly IUiManager _uiManager;
    private readonly List<IProgressObserver> _progressObservers;
    private readonly FirstPersonController _firstPersonController;

    private CompositeDisposable _disposables = new();

    public WinLoseDecider(IUiManager uiManager, List<IProgressObserver> progressObservers,
        FirstPersonController firstPersonController)
    {
        _uiManager = uiManager;
        _progressObservers = progressObservers;
        _firstPersonController = firstPersonController;
    }

    public void Initialize()
    {
        Observable.Merge(_progressObservers.Select(g => g.Win)).Subscribe(_ =>
        {
            _firstPersonController.Freeze(true);
            _uiManager.ShowWinPopup().Forget();
        }).AddTo(_disposables);

        Observable.Merge(_progressObservers.Select(g => g.Lose)).Subscribe(_ =>
        {
            _firstPersonController.Freeze(true);
            _uiManager.ShowLosePopup().Forget();
        }).AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}