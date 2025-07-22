using UnityEngine;
using Utils;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Finger = UnityEngine.InputSystem.EnhancedTouch.Finger;

namespace Controls
{
    public class Swipe : MonoBehaviour
    {
        [SerializeField] private float minDistance = 1.0f;
        [SerializeField] private float maxSwipeTime = 1.0f;
        [SerializeField] private float angleDirectionTolerance = 45.0f;

        public delegate void SwipeEvent(Vector2 direction);
        public event SwipeEvent OnSwipe = null;

        private void Start() => EnhancedTouch.onFingerUp += HandleFingerUp;

        void HandleFingerUp(Finger finger)
        {
            if (finger.index != 0)
                return;

            if (finger.currentTouch.time - finger.currentTouch.startTime > maxSwipeTime)
                return;

            Vector2 swipeVector = finger.currentTouch.screenPosition - finger.currentTouch.startScreenPosition;
            if (MyUtils.ScaleByDPI(swipeVector).sqrMagnitude < minDistance * minDistance)
                return;

            Vector2 direction;
            float cosineTolerance = Mathf.Cos(Mathf.Deg2Rad * angleDirectionTolerance);

            if (Vector2.Dot(swipeVector.normalized, Vector2.right) >= cosineTolerance)
                direction = Vector2.right;
            else if (Vector2.Dot(swipeVector.normalized, Vector2.left) >= cosineTolerance)
                direction = Vector2.left;
            else if (Vector2.Dot(swipeVector.normalized, Vector2.up) > cosineTolerance)
                direction = Vector2.up;
            else
                direction = Vector2.down;

            OnSwipe?.Invoke(direction);
        }
    }
}
