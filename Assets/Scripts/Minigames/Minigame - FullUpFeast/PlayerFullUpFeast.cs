using UnityEngine;

namespace FullUpFeast
{
    public class PlayerFullUpFeast : MonoBehaviour
    {
        public delegate void EatAnimationEvent();
        public event EatAnimationEvent OnEatAnimationEnd = null;

        private void EndEatAnimation() => OnEatAnimationEnd?.Invoke();
    }
}
