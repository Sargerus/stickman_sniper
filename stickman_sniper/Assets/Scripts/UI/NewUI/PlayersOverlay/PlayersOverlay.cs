using DWTools.RPG;
using DWTools.Windows;
using UnityEngine;

public class PlayersOverlayWindowModel : WindowModel
{
    public readonly Character Character;

    public PlayersOverlayWindowModel(string key, Character character) : base(key)
    {
        Character = character;
    }
}

public class PlayersOverlay : BaseWindow
{
    [SerializeField] public CharacterSliderController hpSlider;

    private PlayersOverlayWindowModel _model;

    public override void SetModel(WindowModel model)
    {
        base.SetModel(model);
        _model = (PlayersOverlayWindowModel)model;

        hpSlider.Init(_model.Character);
    }
}
