using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace DWTools.Slowmotion
{
    public interface IBulletProducer
    {
        GameObject gameObject { get; }
        Transform Transform { get; }
        float Duration { get; }

        void SetValue(string key, object value);
        bool TryGetValue(string key, out object value);
        void SetNormalizedPath(float path);
        void Clear();
    }

    public class BulletProducer : MonoBehaviour, IBulletProducer
    {
        [SerializeField]
        private CinemachineDollyCart _cart;

        [field: SerializeField]
        public Transform Transform { get; private set; }

        [field: SerializeField]
        public float Duration { get; private set; }

        private Dictionary<string, object> _cinemaData = new();

        public void SetValue(string key, object value)
        {
            _cinemaData[key] = value;
        }

        public bool TryGetValue(string key, out object value)
        {
            _cinemaData.TryGetValue(key, out value);
            return value != null;
        }

        public void SetNormalizedPath(float path)
        {
            _cart.m_Position = path;
        }

        private void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            _cinemaData?.Clear();
        }
    }
}