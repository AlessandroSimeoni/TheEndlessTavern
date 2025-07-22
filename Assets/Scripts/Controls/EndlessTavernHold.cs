using UnityEngine;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

namespace Controls
{
    public class EndlessTavernHold : Hold
    {
        // same as Hold class, but this uses raycast to detect the tap area


        [SerializeField] private float rayDistance = 20.0f;

        public delegate void HoldIDEvent(bool isHolding, GameObject go);
        public event HoldIDEvent OnHoldGameObject = null;

        private const string TAP_AREA_TAG = "TapArea";

        private Ray ray;
        private RaycastHit hit;

        protected override void HandleFingerDown(Finger finger)
        {
            if (finger.index != 0)
                return;

            ray = Camera.main.ScreenPointToRay(finger.screenPosition);
            Physics.Raycast(ray, out hit, rayDistance);
            if (hit.collider != null && hit.collider.CompareTag(TAP_AREA_TAG))
                delayCoroutine = StartCoroutine(Delay());
        }

        protected override void HandleFingerUp(Finger finger)
        {
            if (hit.collider == null || !hit.collider.CompareTag(TAP_AREA_TAG))
                return;

            base.HandleFingerUp(finger);
        }

        protected override void InvokeEvent(bool isHolding) => OnHoldGameObject?.Invoke(isHolding, hit.collider.gameObject);
    }
}
