using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

namespace Controls
{
    [DisallowMultipleComponent]
    public class Tap : MonoBehaviour
    {
        [SerializeField, Min(0.0f), Tooltip("The seconds after which the tap will not be evaluated")] 
        private float maxTapTime = 0.5f;

        public delegate void ScreenTap();
        public event ScreenTap OnTap;

        private List<RaycastResult> uiDetectorList = new List<RaycastResult>();

        private void Start() => EnhancedTouch.onFingerUp += HandleFingerUp;

        private void HandleFingerUp(Finger finger)
        {
            if (finger.index != 0)
                return;

            if (finger.currentTouch.time - finger.currentTouch.startTime > maxTapTime)
                return;


            //-------------------------
            //(From the lesson's android project) Prevent touch detection on UI
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = finger.currentTouch.startScreenPosition;
            EventSystem.current.RaycastAll(pointer, uiDetectorList);
            if (uiDetectorList.Count > 0)
                return;
            //-------------------------


            OnTap?.Invoke();
        }

        private void OnDisable() => EnhancedTouch.onFingerUp -= HandleFingerUp;
    }
}
