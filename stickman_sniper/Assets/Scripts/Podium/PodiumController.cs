using Cysharp.Threading.Tasks;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PodiumController : MonoBehaviour
{
    private Vector3 _defaultRotation = new(-45, 90, 0);

    [SerializeField] private Transform _container;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _force;
    [SerializeField] private float _zoomForce;

    private ShopProductVisual _weaponVisuals;
    private GameObject _prefabInstance;
    private Vector3 _prevPos;
    private Vector3 _currentPos;

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
       // _prefabInstance.transform.position = Vector3.zero;
        _prefabInstance.transform.rotation = Quaternion.Euler(_defaultRotation);

        var attachmentManager = _prefabInstance.GetComponent<WeaponAttachmentManager>();
        attachmentManager.SetIndexes(customizationIndexes);
        attachmentManager.Initialize();
        AttachmentManager = attachmentManager;
    }

    public void ApplyInput(Vector2 delta)
    {
        Vector2 rot = new(0, delta.x);
        _prefabInstance.transform.Rotate(rot * _force, Space.World);
    }

    public void ApplyZoom(float zoom)
    {
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - zoom * _zoomForce, 25, 60);
    }

    public void Clear()
    {
        _weaponVisuals = null;
        _prevPos = Vector3.zero;
        _currentPos = Vector3.zero;
        AttachmentManager = null;
        Addressables.ReleaseInstance(_prefabInstance);
    }
}
