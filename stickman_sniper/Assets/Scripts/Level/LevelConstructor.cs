using Zenject;

public class LevelConstructor : IInitializable
{
    private readonly FirstPersonController.Factory _fpsFactory;

    private FirstPersonController _player;

    public LevelConstructor(FirstPersonController.Factory fpsFactory)
    {
        _fpsFactory = fpsFactory;
    }

    public void Initialize()
    {
        _player = _fpsFactory.Create();
    }
}
