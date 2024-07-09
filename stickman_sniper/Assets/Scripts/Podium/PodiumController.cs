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

    public void Initialize(ShopProductVisual weaponVisuals, CustomizationIndexes customizationIndexes)
    {
        Clear();
        _weaponVisuals = weaponVisuals;
        InitializeAsync(customizationIndexes).Forget();
    }

    private async UniTask InitializeAsync(CustomizationIndexes customizationIndexes)
    {
        _prefabInstance = (GameObject)(await _weaponVisuals.Product3DModel.InstantiateAsync(Vector3.zero, Quaternion.Euler(_defaultRotation), _container));

        var attachmentManager = _prefabInstance.GetComponent<WeaponAttachmentManager>();
        attachmentManager.SetIndexes(customizationIndexes);
        attachmentManager.Initialize();
        AttachmentManager = attachmentManager;
    }

    public void ApplyInput(Vector2 mousePosition, float zoom)
    {
        _currentPos = mousePosition;
        ApplyRotation();
        //Input.GetAxis("Mouse ScrollWheel")
        ApplyZoom(zoom);
        CacheInputData();
    }

    private void ApplyZoom(float zoom)
    {
        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - zoom * _zoomForce, 25, 60);
    }

    private void ApplyRotation()
    {
        Vector3 delta = new(_currentPos.y - _prevPos.y, _prevPos.x - _currentPos.x);
        _prefabInstance.transform.Rotate(delta * _force, Space.World);
    }

    private void CacheInputData()
    {
        Vector3 buf = _prevPos;
        _prevPos = _currentPos;
        _currentPos = buf;
    }

    private void Clear()
    {
        _weaponVisuals = null;
        _prevPos = Vector3.zero;
        _currentPos = Vector3.zero;
        AttachmentManager = null;
        Addressables.ReleaseInstance(_prefabInstance);
    }
}
