using Minigame;
using System;
using UnityEngine;

namespace BeerNBooze
{
    public class BeerNBoozeTutorialManager : BeerNBoozeManager
    {
        public enum BarCounterPosition
        {
            Left,
            Center,
            Right
        }

        private int beerCounter = 0;

        private const string TUTORIAL_TRIGGER = "BeerNBoozeTutorial";
        private const string LEFT_HOLD_BOOL = "HoldLeft";
        private const string CENTER_HOLD_BOOL = "HoldCenter";
        private const string RIGHT_HOLD_BOOL = "HoldRight";

        [SerializeField] private Animator tutorialAnimator = null;

        [Serializable] public sealed class TutorialEvent : UnityEngine.Events.UnityEvent<MinigameSceneIndex> { }
        public TutorialEvent OnTutorialCompleted = new TutorialEvent();


        protected override void SpawnBarCounters()
        {
            tutorialAnimator.SetTrigger(TUTORIAL_TRIGGER);

            base.SpawnBarCounters();

            foreach (BarCounterTutorial bc in barCounter)
            {
                bc.OnBeerOutOfRange += HandleBeerOutOfRange;
                bc.OnBeerInRange += HandleBeerInRange;
            }
        }


        public override void SpawnBeers()
        {
            int beerSizeIndex = UnityEngine.Random.Range(0, settings.beerSize.Length);
            int barCounterIndex;

            switch (beerCounter)
            {
                case 0:
                    barCounterIndex = (int)BarCounterPosition.Right;
                    break;
                case 1:
                    barCounterIndex = (int)BarCounterPosition.Left;
                    break;
                case 2:
                    barCounterIndex = (int)BarCounterPosition.Center;
                    break;
                default:
                    OnTutorialCompleted?.Invoke(MinigameSceneIndex.BeerNBooze);
                    return;
            }

            barCounter[barCounterIndex].SpawnBeer(settings.beerSize[beerSizeIndex], settings.beerDestructionHeight);
        }

        protected override void DrinkBeer(int barCounterID)
        {
            base.DrinkBeer(barCounterID);

            tutorialAnimator.SetBool(RIGHT_HOLD_BOOL, false);
            tutorialAnimator.SetBool(LEFT_HOLD_BOOL, false);
            tutorialAnimator.SetBool(CENTER_HOLD_BOOL, false);
        }


        private void HandleBeerInRange()
        {
            tutorialAnimator.SetBool(RIGHT_HOLD_BOOL, beerCounter == 0);
            tutorialAnimator.SetBool(LEFT_HOLD_BOOL, beerCounter == 1);
            tutorialAnimator.SetBool(CENTER_HOLD_BOOL, beerCounter == 2);
        }

        private void HandleBeerOutOfRange(bool empty)
        {
            if ( (beerCounter == 0 && empty) || beerCounter > 0)
                beerCounter++;

            SpawnBeers();
        }
    }
}
