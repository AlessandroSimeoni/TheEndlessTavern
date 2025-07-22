using System.Collections;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

namespace Controls
{
    public class Hold : MonoBehaviour
    {
        [SerializeField, Min(0.0f), Tooltip("The delay time in seconds of the input. The input will not be evaluated before this amount of time")] 
        protected float holdDelayTime = 0.0f;

        public delegate void ScreenHold(bool isHolding);
        public event ScreenHold OnHold = null;

        protected Coroutine delayCoroutine = null;

        protected void Start()
        {
            EnhancedTouch.onFingerDown += HandleFingerDown;
            EnhancedTouch.onFingerUp += HandleFingerUp;
        }
        protected virtual void HandleFingerDown(Finger finger)
        {
            if (finger.index != 0)
                return;

            delayCoroutine = StartCoroutine(Delay());
        }

        protected IEnumerator Delay()
        {
            yield return new WaitForSeconds(holdDelayTime); // wait for delay if any
            InvokeEvent(true);
            delayCoroutine = null;
        }

        protected virtual void HandleFingerUp(Finger finger)
        {
            if (finger.index != 0)
                return;

            if (delayCoroutine != null)
                StopCoroutine(delayCoroutine);
            else
                InvokeEvent(false);
        }

        protected virtual void InvokeEvent(bool isHolding)
        {
            OnHold?.Invoke(isHolding);
        }

        private void OnDestroy()
        {
            EnhancedTouch.onFingerDown -= HandleFingerDown;
            EnhancedTouch.onFingerUp -= HandleFingerUp;
        }
    }
}
