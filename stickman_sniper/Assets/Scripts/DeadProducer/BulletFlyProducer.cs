using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Slowmotion;
using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace stickman_sniper.Producer
{
    public interface IBulletFlyProducer
    {
        UniTask SendBulletInSlowmotionAsync(Vector3 startPosition, Vector3 endPosition, ICinemachineDirector bulletDirector);
    }

    internal class BulletFlyProducer : IBulletFlyProducer, IDisposable
    {
        private CameraProvider _cameraProvider;
        private IInputService _inputService;
        private ISlowmotionTimeController _slowmotionTimeController;

        private CompositeDisposable _disposables = new();

        public BulletFlyProducer([Inject(Id = "slowmotion")] CameraProvider cameraProvider, 
            IInputService inputService,
            ISlowmotionTimeController slowmotionService)
        {
            _cameraProvider = cameraProvider;
            _inputService = inputService;
            _slowmotionTimeController = slowmotionService;
        }

        public async UniTask SendBulletInSlowmotionAsync(Vector3 startPosition, Vector3 endPosition, ICinemachineDirector bulletDirector)
        {
            float pathLength = Vector3.Distance(startPosition, endPosition);
            Vector3 pathUnitVector = (endPosition - startPosition).normalized;
            bulletDirector.transform.rotation = Quaternion.LookRotation(pathUnitVector);
            bulletDirector.GetRandomCamera().gameObject.SetActive(true);

            float totalTime = 0;
            _slowmotionTimeController.AlignTimeScale(0.1f);
            _cameraProvider.Camera.gameObject.SetActive(true);
            _inputService.DisableInput();
            while (totalTime < (bulletDirector.Duration / 1000)) 
            {
                totalTime = Time.deltaTime;
                if (bulletDirector.TryGetValue("time_in_air", out object time))
                {
                    float inAirTime = (float)time;
                    totalTime += inAirTime;
                }

                bulletDirector.SetValue("time_in_air", totalTime);

                float lerp = totalTime / (bulletDirector.Duration / 1000);
                bulletDirector.SetProgress(lerp);
                bulletDirector.transform.position = startPosition + pathUnitVector * Mathf.Lerp(0, pathLength, lerp);

                await UniTask.Yield();
            }

            bulletDirector.TurnOffAllCameras();
            _slowmotionTimeController.AlignWithUnity();
            _cameraProvider.Camera.gameObject.SetActive(false);
            _inputService.EnableInput();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}