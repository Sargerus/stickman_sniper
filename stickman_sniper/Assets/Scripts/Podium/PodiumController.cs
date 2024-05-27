using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumController : MonoBehaviour
{
    [SerializeField] private Transform tr;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _force;
    [SerializeField] private float _zoomForce;

    private Transform _target;
    private Vector3 _prevPos;
    private Vector3 _currentPos;

    private void Awake()
    {
        SetTarget(tr);
        SetCamera(_camera);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetCamera(Camera cam)
    {
        _camera = cam;
    }

    public void ApplyRotation(Vector3 euler)
    {
        _target.Rotate(euler * _force, Space.World);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _prevPos = Input.mousePosition;
            }

            _currentPos = Input.mousePosition;
            Vector3 delta = new(_currentPos.y - _prevPos.y, _prevPos.x - _currentPos.x);
            ApplyRotation(delta);
        }

        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * _zoomForce, 25, 60);
    }    

    private void LateUpdate()
    {
        Vector3 buf = _prevPos;
        _prevPos = _currentPos;
        _currentPos = buf;
    }
}
