using DWTools;
using UnityEngine;
using YG;
using Zenject;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivityMultiplier;

    private IInputService _inputService;

    private bool _isInitialized = false;
    private Quaternion refRotation;

    private float xRotation;
    private float yRotation;

    [Inject]
    private void Construct(IInputService inputService)
    {
        _inputService = inputService;

        _isInitialized = YandexGame.Device.ToDevice() != Device.Mobile;
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        // get mouse input
        float mouseX = /*Input.GetAxisRaw("Mouse X")*/ _inputService.MouseX * sensitivityMultiplier;
        float mouseY = /*Input.GetAxisRaw("Mouse Y")*/ _inputService.MouseY * sensitivityMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
    }
}