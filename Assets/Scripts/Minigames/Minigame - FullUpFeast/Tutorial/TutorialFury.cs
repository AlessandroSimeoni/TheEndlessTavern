using UnityEngine;

namespace FullUpFeast
{
    public class TutorialFury : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;

        private const string FURY_TRIGGER = "Fury";

        public delegate void FuryEvent();
        public event FuryEvent OnFuryAnimationCompleted = null;
        public event FuryEvent OnFoodAnimationRequest = null;

        public void HandleFuryAnimationEnding() => OnFuryAnimationCompleted?.Invoke();
        public void TriggerFoodAnimation() => OnFoodAnimationRequest?.Invoke();

        public void StartFuryAnimation() => animator.SetTrigger(FURY_TRIGGER);
    }
}
