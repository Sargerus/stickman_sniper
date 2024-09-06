using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DWTools.Slowmotion;
using UnityEngine;
using Zenject;

namespace StickmanSniper.AI
{
    public class SlowmotionWait : Action, ISlowmotionAgent
    {
        public SharedFloat WaitTime;
        public SharedBool IsRandomWait;
        public SharedFloat WaitTimeMin;
        public SharedFloat WaitTimeMax;

        private ISlowmotionService _slowmotionService;

        private float _waitDuration;
        private float _timeWaitLeft;
        private TaskStatus _taskStatus = TaskStatus.Running;

        public bool AllowToUpdate { get; set; }

        [Inject]
        private void Construct(ISlowmotionService slowmotionService)
        {
            _slowmotionService = slowmotionService;
            _slowmotionService.LinkAgent(this);
        }

        public override void OnStart()
        {
            if (IsRandomWait.Value)
            {
                _waitDuration = Random.Range(WaitTimeMin.Value, WaitTimeMax.Value);
            }
            else
            {
                _waitDuration = WaitTime.Value;
            }

            _timeWaitLeft = _waitDuration;
            _taskStatus = TaskStatus.Running;
        }

        public override TaskStatus OnUpdate()
        {
            return _taskStatus;
        }

        public void SlowmotionUpdate(float deltaTime)
        {
            _timeWaitLeft -= deltaTime;
            _taskStatus = _timeWaitLeft > 0 ? TaskStatus.Running : TaskStatus.Success;
        }

        public override void OnBehaviorComplete()
        {
            _slowmotionService.RemoveAgent(this);
        }
    }
}