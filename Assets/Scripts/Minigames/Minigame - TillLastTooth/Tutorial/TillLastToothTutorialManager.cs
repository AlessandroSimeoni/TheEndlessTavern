using Minigame;
using System;
using UnityEngine;

namespace TillLastTooth
{
    public class TillLastToothTutorialManager : TillLastToothManager
    {
        [SerializeField] private Animator tutorialAnimator = null;
        [Serializable] public sealed class TutorialEvent : UnityEngine.Events.UnityEvent<MinigameSceneIndex> { }
        public TutorialEvent OnTutorialCompleted = new TutorialEvent();

        private const string TUTORIAL_TRIGGER = "TillLastToothTutorial";
        private const string LEFT_SWIPE_BOOL = "SwipeLeft";
        private const string RIGHT_SWIPE_BOOL = "SwipeRight";

        public override void FirstEnemyEntrance()
        {
            tutorialAnimator.SetTrigger(TUTORIAL_TRIGGER);
            base.FirstEnemyEntrance();
            ((EnemyTutorial)enemyInstance).OnEnemyExit += HandleTutorialEnd;
            uiManager.OnPopUpDone += HandleTutorialEnemyAttack;
        }

        public override void MovePlayer(Vector2 direction)
        {
            if (Time.timeScale == 0 &&
                ((direction == Vector2.right && ((EnemyTutorial)enemyInstance).attackCount == 0) ||
                (direction == Vector2.left && ((EnemyTutorial)enemyInstance).attackCount == 1)))
            {
                Time.timeScale = 1;
                playerInstance.Dodge(direction);
                tutorialAnimator.SetBool(LEFT_SWIPE_BOOL, false);
                tutorialAnimator.SetBool(RIGHT_SWIPE_BOOL, false);
            }
        }

        private void HandleTutorialEnd() => OnTutorialCompleted?.Invoke(MinigameSceneIndex.TillLastTooth);

        private void HandleTutorialEnemyAttack()
        {
            Time.timeScale = 0;
            tutorialAnimator.SetBool( ((EnemyTutorial)enemyInstance).attackCount == 0 ? RIGHT_SWIPE_BOOL : LEFT_SWIPE_BOOL, true);
        }
    }
}
