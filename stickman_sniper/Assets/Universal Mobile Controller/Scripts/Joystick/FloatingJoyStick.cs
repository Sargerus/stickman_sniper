using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;
using DWTools;
using System.Linq;
using YG;
using TMPro;

namespace UniversalMobileController
{
    public class FloatingJoyStick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IFinger
    {
        [Inject(Id = "mobile")]
        private CameraProvider _mobileCameraProvider;

        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joyStick;

        [Range(0, 10f)][SerializeField] private float joystickMovementRange = 1f;
        public float MovementRange => joystickMovementRange;

        private Vector2 joyStickInput = Vector2.zero;
        private bool _wasCalledUp = true;
        private int? uniqueFingerId = null;


        private Vector2 joystickCurrentPosition = new Vector2(0, 0);
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color pressedColor = Color.white;
        [SerializeField] private State joystickState;
        /*List of events*/
        public UnityEvent onGameStart;
        public UnityEvent onPressedJoystick;
        public UnityEvent onStartedDraggingJoystick;
        public UnityEvent onStoppedDraggingJoystick;

        [SerializeField]
        private TMP_Text _log;

        private void Start()
        {
            onGameStart.Invoke();
            SetJoystickColor(normalColor);
        }

        public void OnPointerDown(PointerEventData eventdata)
        {
            if (UniversalMobileController_Manager.editMode || joystickState == State.Un_Interactable) { return; }
            _wasCalledUp = false;
            SetJoystickColor(pressedColor);

            _log.SetText(string.Empty);
            _log.text += " OnPointerDown";

            Vector3 posToConvert = new(eventdata.position.x, eventdata.position.y, GetComponentInParent<Canvas>().planeDistance);

            joystickCurrentPosition = eventdata.position;
            var pos = _mobileCameraProvider.Camera.ScreenToWorldPoint(posToConvert);

            joystickBackground.position = pos;
            joyStick.anchoredPosition = Vector2.zero;
            OnDrag(eventdata);
            onPressedJoystick.Invoke();
        }
        public void OnPointerUp(PointerEventData eventdata)
        {
            if (UniversalMobileController_Manager.editMode || joystickState == State.Un_Interactable) { return; }
            SetJoystickColor(normalColor);

            _log.text += " OnPointerUp";

            joyStickInput = new Vector2(0, 0);
            joyStick.anchoredPosition = new Vector2(0, 0);
            onStoppedDraggingJoystick.Invoke();
            _wasCalledUp = true;
        }
        public void OnDrag(PointerEventData eventdata)
        {
            if (UniversalMobileController_Manager.editMode || joystickState == State.Un_Interactable || uniqueFingerId is null) { return; }

            onStartedDraggingJoystick.Invoke();
            Vector2 direction = eventdata.position - joystickCurrentPosition;

            if (direction.magnitude > joystickBackground.sizeDelta.x / 2f)
            {
                joyStickInput = direction.normalized;
            }
            else
            {
                joyStickInput = direction / (joystickBackground.sizeDelta.x / 2f);
            }

            joyStick.anchoredPosition = (joyStickInput * joystickBackground.sizeDelta.x / 2f) * joystickMovementRange;
        }

        private void SetJoystickColor(Color color)
        {
            joystickBackground.gameObject.GetComponent<Image>().color = color;
            joyStick.gameObject.GetComponent<Image>().color = color;
        }

        public float GetVerticalValue()
        {
            return joyStickInput.y;
        }
        public float GetHorizontalValue()
        {
            return joyStickInput.x;
        }
        public Vector2 GetHorizontalAndVerticalValue()
        {
            return joyStickInput;
        }
        public void SetState(State state)
        {
            joystickState = state;
        }

        public State GetState()
        {
            return joystickState;
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
    public enum State
    {
        Interactable, Un_Interactable
    }
}