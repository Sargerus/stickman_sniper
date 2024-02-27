using Cinemachine;
using DWTools.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace stickman_sniper.Producer
{
    public class BulletDirector : MonoBehaviour, ICinemachineDirector
    {
        [SerializeField] private List<CinemachinePathBase> _path;
        [SerializeField] private CinemachineDollyCart _cart;
        [SerializeField] private CinemachineVirtualCamera _camera;

        [field: SerializeField]
        public int Duration { get; private set; }

        private Dictionary<string, object> _cinemaData = new();

        private void Awake()
        {
            _camera.gameObject.SetActive(false);
        }

        public void SetValue(string key, object value)
        {
            _cinemaData[key] = value;
        }

        public bool TryGetValue(string key, out object value)
        {
            _cinemaData.TryGetValue(key, out value);
            return value != null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            _cinemaData?.Clear();
        }

        public CinemachineVirtualCamera GetRandomCamera()
        {
            _cart.m_Path = _path.Random();
            return _camera;
        }

        public void TurnOffAllCameras()
        {
            _camera.gameObject.SetActive(false);
        }

        public void SetProgress(float progress)
        {
            _cart.m_Position = progress;
        }
    }
}