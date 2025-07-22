using Audio;
using UnityEngine;

namespace SceneLoad
{
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] private Animator transitionAnimator = null;
        [SerializeField] private AudioListener _audioListener = null;
        [SerializeField] private AudioClip transitionSFX = null;

        public AudioListener audioListener { get { return _audioListener; } }

        public delegate void TransitionEvent();
        public event TransitionEvent OnFirstAnimationDone = null;
        public event TransitionEvent OnTransitionOver = null;

        public void StartingTransition()
        {
            transitionAnimator.SetBool("Transition", true);
            AudioPlayer.instance.PlaySFX(transitionSFX);
        }
        public void EndingTransition()
        {
            transitionAnimator.SetBool("Transition", false);
            AudioPlayer.instance.PlaySFX(transitionSFX);
        }

        public void FirstAnimationOver() => OnFirstAnimationDone?.Invoke();
        public void TransitionOver() => OnTransitionOver?.Invoke();
    }
}
