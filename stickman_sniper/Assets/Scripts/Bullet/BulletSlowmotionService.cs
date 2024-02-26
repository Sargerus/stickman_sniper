using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace DWTools.Slowmotion
{
    internal class BulletSlowmotionPathInfo
    {
        public Vector3 StartPosition;
        public float PathLength;
        public Vector3 PathUnitVector;
        public IBulletProducer Producer;

        public BulletSlowmotionPathInfo(Vector3 startPosition, float pathLength, Vector3 pathUnitVector, IBulletProducer bulletProducer)
        {
            StartPosition = startPosition;
            PathLength = pathLength;
            PathUnitVector = pathUnitVector;
            Producer = bulletProducer;
        }
    }

    public interface IBulletSlowmotionService
    {
        UniTask SendBulletInSlowmotionAsync(Vector3 startPosition, Vector3 endPosition, IBulletProducer producer);
    }

    internal class BulletSlowmotionService : IBulletSlowmotionService, IDisposable
    {
        private CameraProvider _cameraProvider;
        private IInputService _inputService;
        private ISlowmotionTimeController _slowmotionTimeController;

        private CompositeDisposable _disposables = new();

        public BulletSlowmotionService([Inject(Id = "slowmotion")] CameraProvider cameraProvider, 
            IInputService inputService,
            ISlowmotionTimeController slowmotionService)
        {
            _cameraProvider = cameraProvider;
            _inputService = inputService;
            _slowmotionTimeController = slowmotionService;
        }

        public async UniTask SendBulletInSlowmotionAsync(Vector3 startPosition, Vector3 endPosition, IBulletProducer producer)
        {
            float pathLength = Vector3.Distance(startPosition, endPosition);
            Vector3 pathUnitVector = (endPosition - startPosition).normalized;
            producer.gameObject.transform.rotation = Quaternion.LookRotation(pathUnitVector);
            BulletSlowmotionPathInfo pathInfo = new(startPosition, pathLength, pathUnitVector, producer);

            float totalTime = 0;
            _slowmotionTimeController.AlignTimeScale(0.1f);
            _cameraProvider.Camera.gameObject.SetActive(true);
            _inputService.DisableInput();
            while (totalTime < pathInfo.Producer.Duration) 
            {
                totalTime = Time.deltaTime;
                if (pathInfo.Producer.TryGetValue("time_in_air", out object time))
                {
                    float inAirTime = (float)time;
                    totalTime += inAirTime;
                }

                pathInfo.Producer.SetValue("time_in_air", totalTime);

                float lerp = totalTime / pathInfo.Producer.Duration;
                pathInfo.Producer.SetNormalizedPath(lerp);
                pathInfo.Producer.Transform.position = pathInfo.StartPosition + pathInfo.PathUnitVector * Mathf.Lerp(0, pathInfo.PathLength, lerp);

                await UniTask.Yield();
            }

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