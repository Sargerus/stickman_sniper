using Cinemachine;
using UnityEngine;

namespace StickmanSniper.Producers
{
    public interface ICinemachineActor
    {
        Transform transform { get; }
    }

    public interface ICinemachineCameraProvider
    {
        CinemachineVirtualCamera GetRandomCamera();
        void TurnOffAllCameras();
    }

    public interface ICinemachineDirector : ICinemachineActor, ICinemachineCameraProvider
    {
        int Duration { get; }
        void SetValue(string key, object value);
        bool TryGetValue(string key, out object value);
        void SetProgress(float progress);
        void Clear();
    }
}