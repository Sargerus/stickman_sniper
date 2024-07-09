using UnityEngine;
using Zenject;

public class UIManagerInstaller : MonoInstaller
{
    [SerializeField] private DWTools.Windows.UIManager uiManager;
    [SerializeField] private Camera _uiCamera;

    public override void InstallBindings()
    {
        uiManager.SetCamera(_uiCamera);
        Container.BindInterfacesTo<DWTools.Windows.UIManager>().FromInstance(uiManager).AsSingle().NonLazy();
        Container.QueueForInject(uiManager);
    }
}
