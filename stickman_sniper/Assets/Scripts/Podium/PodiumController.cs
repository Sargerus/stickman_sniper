using Customization;
using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PodiumController : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _force;
    [SerializeField] private float _zoomForce;

    private ShopProductVisual _weaponVisuals;
    private GameObject _prefabInstance;
    private Transform _target;
    private Vector3 pos = new(0.2f, -0.6f, 0.65f);

    public IAttachmentManager AttachmentManager { get; private set; }

    public async UniTask Initialize(ShopProductVisual weaponVisuals, CustomizationIndexes customizationIndexes)
    {
        Clear();
        _weaponVisuals = weaponVisuals;
        await InitializeAsync(customizationIndexes);
    }

    private async UniTask InitializeAsync(CustomizationIndexes customizationIndexes)
    {
        _prefabInstance = (GameObject)(await _weaponVisuals.Product3DModel.InstantiateAsync(_container));
        _prefabInstance.transform.localPosition = pos;

        var attachmentManager = _prefabInstance.GetComponentInChildren<WeaponAttachmentManager>();
        _target = _prefabInstance.transform.GetChild(0);
        attachmentManager.SetIndexes(customizationIndexes);
        attachmentManager.Initialize();
        AttachmentManager = attachmentManager;
    }

    public void ApplyInput(Vector2 delta)
    {
        Vector2 rot = new(0, delta.x);
        _target.Rotate(rot * _force, Space.World);
    }

    public void ApplyZoom(float zoom)
    {
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - zoom * _zoomForce, 25, 60);
    }

    public void Clear()
    {
        _weaponVisuals = null;
        AttachmentManager = null;

        if (_prefabInstance != null)
            Addressables.ReleaseInstance(_prefabInstance);
    }
}
