using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _bulletCount;
    [SerializeField] private Image _bulletImage;

    [Inject] private IWeaponService _weaponService;

    private CompositeDisposable _compositeDisposable = new();

    [Inject]
    private void Construct()
    {
        _weaponService.CurrentWeapon.Subscribe(w =>
        {
            _compositeDisposable?.Clear();

            if (w == null)
                return;

            w.CurrentBulletsCount.Subscribe(c =>
            {
                _bulletCount.SetText(c.ToString());
            }).AddTo(_compositeDisposable);
        }).AddTo(this);
    }
}
