using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using YG;
using DWTools;
using TMPro;
using System.Linq;

namespace UniversalMobileController
{
    public class SpecialTouchPad : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IFinger
    {
        private Vector2 distanceBetweenTouch;
        private Vector2 PointerOld;
        private int eventPointerID;
        private bool pressingTouchPad;
        private Vector2 touchPadInput;
        [Range(0.2f, 20)]
        public float senstivityX = 3f;
        [Range(0.2f, 20)]
        public float senstivityY = 3f;
        public State touchPadState;
        public UnityEvent onGameStart;
        public UnityEvent onPressedTouchPad;
        public UnityEvent onStartedDraggingTouchPad;
        public UnityEvent onStoppedDraggingTouchPad;
        private int? uniqueFingerId = null;
        private bool _wasCalledUp = true;

        private Device device;

        [SerializeField]
        private TMP_Text _log;

        private void OnEnable()
        {
            device = YandexGame.Device.ToDevice();
        }

        void EraseInput()
        {
            touchPadInput = Vector2.zero;
        }

        void Update()
        {
            if (UniversalMobileController_Manager.editMode|| touchPadState == State.Un_Interactable || uniqueFingerId == null) { EraseInput(); return; }

            _log.text += "2";
            if (pressingTouchPad)
            {
                _log.text += "3";
                if (eventPointerID < Input.touches.Length && eventPointerID >= 0)
                {
                    distanceBetweenTouch = Input.touches[eventPointerID].position - PointerOld;
                    PointerOld = Input.touches[eventPointerID].position;
                }
                else
                {
                    distanceBetweenTouch = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                    PointerOld = Input.mousePosition;
                }
            }
            else
            {
                distanceBetweenTouch = Vector2.zero;
            }

            touchPadInput.x = distanceBetweenTouch.x * Time.deltaTime * senstivityX;
            touchPadInput.y = distanceBetweenTouch.y * Time.deltaTime * senstivityY;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _wasCalledUp = false;
            pressingTouchPad = true;
            eventPointerID = eventData.pointerId;
            PointerOld = eventData.position;

            _log.SetText(string.Empty);
            _log.text += " OnPointerDown";
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressingTouchPad = false;
            uniqueFingerId = null;
            _log.text +=" OnPointerUp";
            _wasCalledUp = true;
            EraseInput();
        }
        public float GetVerticalValue()
        {
            return touchPadInput.y;
        }
        public float GetHorizontalValue()
        {
            return touchPadInput.x;
        }
        public Vector2 GetHorizontalAndVerticalValue()
        {
            return touchPadInput;
        }
        public void SetState(State state)
        {
            touchPadState = state;
        }

        public State GetState()
        {
            return touchPadState;
        }

        public void SetFingerId(int? fingerId)
        {
            uniqueFingerId = fingerId;

            if (uniqueFingerId == null && _wasCalledUp == false)
            {
                OnPointerUp(null);
                _log.text += "Uniq=null";
            }
        }
    }
}