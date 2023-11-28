using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WinUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _killedText;
    [SerializeField] private GameObject _star1;
    [SerializeField] private TMP_Text _bulletsText;
    [SerializeField] private GameObject _star2;
    [SerializeField] private TMP_Text _restartsText;
    [SerializeField] private GameObject _star3;

    private IPlayerProgressObserver _playerProgressObserver;
    private ILevelProgressObserver _levelProgressObserver;
    private IWeaponService _weaponService;

    [Inject]
    public void Construct(IPlayerProgressObserver playerProgressObserver,
        ILevelProgressObserver levelProgressObserver,
        IWeaponService weaponService)
    {
        _playerProgressObserver = playerProgressObserver;
        _levelProgressObserver = levelProgressObserver;
        _weaponService = weaponService;
    }

    public void Initialize()
    {
        _killedText.SetText($"{_levelProgressObserver.KilledEnemies}/{_levelProgressObserver.TotalEnemies}");

        StartCoroutine(ShowStar(_star1.GetComponent<Image>()));

        _bulletsText.SetText($"{5 - _weaponService.CurrentWeapon.Value.CurrentBulletsCount.Value}/{5}");
        if (_weaponService.CurrentWeapon.Value.CurrentBulletsCount.Value > 0)
        {
            StartCoroutine(ShowStar(_star2.GetComponent<Image>(), 0.4f));
        }

        _restartsText.SetText($"{0}");
        if (true)
        {
            StartCoroutine(ShowStar(_star3.GetComponent<Image>(), 0.8f));
        }
    }

    private IEnumerator ShowStar(Image image, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        image.gameObject.transform.localScale *= 1.5f;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

        float limit = 0.8f;
        float counter = 0;
        image.gameObject.SetActive(true);

        while (counter < limit)
        {
            float coef = (Time.deltaTime / limit);
            image.gameObject.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f) * coef;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + coef);

            counter += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
