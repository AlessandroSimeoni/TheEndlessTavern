using System;
using UnityEngine;
using EnhancedTouchSupport = UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport;

namespace Controls
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tap))]
    [RequireComponent(typeof(EndlessTavernHold))]
    [RequireComponent(typeof(Swipe))]
    public class InputManager : MonoBehaviour
    {
        [Serializable] public sealed class TapEvent : UnityEngine.Events.UnityEvent { }
        [Serializable] public sealed class HoldEvent : UnityEngine.Events.UnityEvent<bool, GameObject> { }
        [Serializable] public sealed class SwipeEvent : UnityEngine.Events.UnityEvent<Vector2> { }
        
        public TapEvent OnTap = new TapEvent();
        public HoldEvent OnHold = new HoldEvent();
        public SwipeEvent OnSwipe = new SwipeEvent();

        private Tap tap = null;
        private EndlessTavernHold hold = null;
        private Swipe swipe = null;

        private void Awake() => EnhancedTouchSupport.Enable();

        private void Start()
        {
            tap = GetComponent<Tap>();
            tap.OnTap += HandleTap;

            hold = GetComponent<EndlessTavernHold>();
            hold.OnHoldGameObject += HandleHold;

            swipe = GetComponent<Swipe>();
            swipe.OnSwipe += HandleSwipe;
        }

        private void HandleTap() => OnTap?.Invoke();
        private void HandleHold(bool isHolding, GameObject go) => OnHold?.Invoke(isHolding, go);
        private void HandleSwipe(Vector2 direction) => OnSwipe?.Invoke(direction);
    }
}
