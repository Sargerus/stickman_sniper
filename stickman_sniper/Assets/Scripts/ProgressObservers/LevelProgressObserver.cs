using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using YG;

public interface IProgressObserver
{
    IReadOnlyReactiveProperty<bool> Lose { get; }
    IReadOnlyReactiveProperty<bool> Win { get; }
}

public interface ILevelProgressObserver : IProgressObserver
{
    int TotalEnemies { get; }
    IReadOnlyReactiveProperty<int> KilledEnemies { get; }

    void Observe(IReadOnlyList<Enemy> enemyList);
}

public class LevelProgressObserver : ILevelProgressObserver, IDisposable
{
    private List<IReadOnlyReactiveProperty<bool>> _enemies = new();
    private CompositeDisposable _disposables = new();

    public int TotalEnemies { get; private set; }

    private ReactiveProperty<int> _killedEnemies = new(0);
    public IReadOnlyReactiveProperty<int> KilledEnemies => _killedEnemies;

    private ReactiveProperty<bool> _lose = new(false);
    public IReadOnlyReactiveProperty<bool> Lose => _lose;

    private ReactiveProperty<bool> _win = new(false);
    public IReadOnlyReactiveProperty<bool> Win => _win;

    public void Observe(IReadOnlyList<Enemy> enemyList)
    {
        if (enemyList is null || enemyList.Count < 0)
            return;

        TotalEnemies = enemyList.Count;

        IReadOnlyReactiveProperty<bool> mergedObs = Observable.Merge(enemyList.Select(g => g.IsAlive)).ToReactiveProperty();

        foreach (var enemy in enemyList)
        {
            _enemies.Add(enemy.IsAlive);
        }

        mergedObs.Subscribe(_ =>
        {
            _killedEnemies.Value = _enemies.Count(g => g.Value == false);
        }).AddTo(_disposables);

        _killedEnemies.Subscribe(killed =>
        {
            if (_killedEnemies.Value == TotalEnemies)
            {
                YandexGame.savesData.levelsPassed++;
                YandexGame.SaveProgress();

                _win.Value = true;
            }
        }).AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}