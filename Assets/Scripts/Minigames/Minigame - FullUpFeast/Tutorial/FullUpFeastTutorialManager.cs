using Audio;
using Minigame;
using System;
using UnityEngine;

namespace FullUpFeast
{
    public class FullUpFeastTutorialManager : FullUpFeastManager
    {
        [SerializeField] private Animator tutorialAnimator = null;
        [SerializeField] private TutorialFury furyAnimation = null;
        [SerializeField] private GameObject inputGameobject = null;
        [Serializable] public sealed class TutorialEvent : UnityEngine.Events.UnityEvent<MinigameSceneIndex> { }
        public TutorialEvent OnTutorialCompleted = new TutorialEvent();

        private int[] tapPerPlate = { 1,2,3,6};
        private int currentPlate = -1;

        private const string TUTORIAL_TRIGGER = "FullUpFeastTutorial";
        private const string SINGLE_TAP_BOOL = "SingleTap";
        private const string DOUBLE_TAP_BOOL = "DoubleTap";
        private const string TRIPLE_TAP_BOOL = "TripleTap";
       

        public override void SetUpFirstPlate()
        {
            base.SetUpFirstPlate();
            tutorialAnimator.SetTrigger(TUTORIAL_TRIGGER);
            ((TutorialPlate)plate).OnTutorialPlateStop += HandleTutorialPlateStop;
            ((TutorialPlate)plate).OnTutorialPlateEmpty += HandleTutorialPlateEmpty;
            furyAnimation.OnFuryAnimationCompleted += HandleEmptyPlateTap;
            furyAnimation.OnFoodAnimationRequest += ((TutorialPlate)plate).TriggerFoodAnimation;
        }


        protected override void PreparePlate()
        {
            if (++currentPlate >= 4)
            {
                OnTutorialCompleted?.Invoke(MinigameSceneIndex.FullUpFeast);
                return;
            }

            plate.SetEscapeSpeed(1.4f);
            plate.SetUpFood(foodManager.PrepareFood(tapPerPlate[currentPlate], plate.maxPossibleFoods));
        }

        private void HandleTutorialPlateStop()
        {
            Time.timeScale = 0;
            switch (currentPlate)
            {
                case 0: case 3: // handle first and fourth plate
                    tutorialAnimator.SetBool(SINGLE_TAP_BOOL, true);
                break;
                case 1: // handle second plate
                    tutorialAnimator.SetBool(DOUBLE_TAP_BOOL, true);
                break;
                case 2: // handle third plate
                    tutorialAnimator.SetBool(TRIPLE_TAP_BOOL, true);
                break;
                case 4: // handle fourth plate escape
                    furyAnimation.StartFuryAnimation();
                break;
            }
        }
        protected override void CheckLeftovers(int leftovers) { } //do nothing

        private void HandleTutorialPlateEmpty()
        {
            Time.timeScale = 1;
            tutorialAnimator.SetBool(SINGLE_TAP_BOOL, false);
            tutorialAnimator.SetBool(DOUBLE_TAP_BOOL, false);
            tutorialAnimator.SetBool(TRIPLE_TAP_BOOL, false);
        }

        public override void HandleEmptyPlateTap()
        {
            Time.timeScale = 1;
            if (currentPlate < 4)
                return;
            else
                AudioPlayer.instance.PlaySFX(maxFurySFX);

            base.HandleEmptyPlateTap();
        }

        protected override void HandleFoodConsumed()
        {
            base.HandleFoodConsumed();

            if (currentPlate == 3)
            {
                inputGameobject.SetActive(false);
                currentPlate++;
                // do as if the plate is empty
                HandleTutorialPlateEmpty();
                plate.SetTimeSlider(0.0f);
                ((TutorialPlate)plate).SetLastMovement(0.25f);
            }
        }
    }
}
