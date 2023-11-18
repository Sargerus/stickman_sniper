using DWTools;
using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private SceneContext _sceneContext;
    [SerializeField] private TMP_Text _text;

    private bool _isInitialize = false;

    private void OnEnable() => YandexGame.GetDataEvent += OnYGInitialized;
    private void OnDisable() => YandexGame.GetDataEvent -= OnYGInitialized;

    private void Start()
    {
        if (!YandexGame.SDKEnabled)
            return;

        OnYGInitialized();   
    }

    private void OnYGInitialized()
    {
        if(_isInitialize) 
            return;

        _isInitialize = true;

        _sceneContext.Run();
        var inputService = _sceneContext.Container.Resolve<IInputService>();
        _text.SetText(inputService.Device.ToString());
    }
}
