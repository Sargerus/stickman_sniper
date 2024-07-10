using DW.StateMachine;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public interface ILoadingManagerHolder
{
    LoadingManager LoadingManager { get; set; }
}

public class LoadingManagerHolder : ILoadingManagerHolder
{
    public LoadingManager LoadingManager { get; set; }
}

public class LoadingManager : IDisposable
{
    public const string SceneContextTag = "SceneContext";
    public const string MainMenu = "MainMenuScene";
    public const string GameScene = "GameScene";

    private readonly SceneAddressablesContainer _sceneAddressablesContainer;

    private StateMachine _gameStateMachine;
    private IDisposable _updateDisposable;

    public LoadingManager(SceneAddressablesContainer sceneAddressablesContainer)
    {
        _sceneAddressablesContainer = sceneAddressablesContainer;

        _updateDisposable = Observable.EveryUpdate().Subscribe(x =>
        {
            _gameStateMachine.Tick(Time.deltaTime);
        });
    }

    public void StartGameMachine()
    {
        _gameStateMachine = new();

        IState bootstrapState = new BootstrapSceneState(this);
        IState mainMenuState = new MainMenuSceneState(_sceneAddressablesContainer.Get(MainMenu));
        //IState gameState = new SceneState(_sceneAddressablesContainer.Get(GameScene));

        List<IConnection> connections = new() { new BootstrapMainMenuConnection(bootstrapState, mainMenuState) };
        _gameStateMachine.AddState(bootstrapState, connections);

        connections.Clear();
        //connections.AddRange(new List<IConnection>() { new MainMenuGameConnection(mainMenuState, gameState) });

        _gameStateMachine.AddState(mainMenuState, connections);

        //connections.Clear();
        //connections.AddRange(new List<IConnection>() { new GameMainMenuConnection(gameState, mainMenuState) });
        //
        //_gameStateMachine.AddState(gameState, connections);

        _gameStateMachine.Start(bootstrapState);
    }

    public void Dispose()
    {
        _updateDisposable?.Dispose();
        _gameStateMachine.Stop(true);
    }
}