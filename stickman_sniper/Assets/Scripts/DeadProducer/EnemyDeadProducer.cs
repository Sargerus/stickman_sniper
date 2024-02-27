using Cinemachine;
using Cysharp.Threading.Tasks;
using DWTools;
using DWTools.Slowmotion;
using Zenject;

namespace stickman_sniper.Producer
{
    public interface IEnemyDeadProducer
    {
        UniTask ShowEnemyDeath(ICinemachineDirector enemyDirector);
    }

    internal class EnemyDeadProducer : IEnemyDeadProducer
    {
        private CameraProvider _cameraProvider;
        private IInputService _inputService;
        private ISlowmotionTimeController _slowmotionTimeController;

        public EnemyDeadProducer([Inject(Id = "slowmotion")] CameraProvider cameraProvider, 
            IInputService inputService, 
            ISlowmotionTimeController slowmotionTimeController)
        {
            _cameraProvider = cameraProvider;
            _inputService = inputService;
            _slowmotionTimeController = slowmotionTimeController;
        }

        public async UniTask ShowEnemyDeath(ICinemachineDirector enemyDirector)
        {
            _slowmotionTimeController.AlignTimeScale(0.1f);
            _inputService.DisableInput();

            enemyDirector.TurnOffAllCameras();
            var vcam = enemyDirector.GetRandomCamera();

            var bufParent = vcam.transform.parent;
            vcam.transform.parent = null;
            vcam.gameObject.SetActive(true);

            _cameraProvider.Camera.gameObject.SetActive(true);

            await UniTask.Delay(enemyDirector.Duration);

            vcam.transform.parent = bufParent;
            enemyDirector.TurnOffAllCameras();
            _cameraProvider.Camera.gameObject.SetActive(false);

            _slowmotionTimeController.AlignWithUnity();
            _inputService.EnableInput();
        }
    }
}