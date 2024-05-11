using UnityEngine;
using Zenject;

public class UIManagerInstaller : MonoInstaller
{
    [SerializeField] private DWTools.Windows.UIManager uiManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<DWTools.Windows.UIManager>().FromInstance(uiManager).AsSingle().NonLazy();
        Container.QueueForInject(uiManager);
    }
}
