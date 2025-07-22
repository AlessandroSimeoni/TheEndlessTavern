using Audio;
using System.Collections;

namespace FullUpFeast
{
    public class TutorialPlate : Plate
    {
        private bool lastMovement = false;

        public delegate void TutorialPlateEvent();
        public event TutorialPlateEvent OnTutorialPlateStop = null;
        public event TutorialPlateEvent OnTutorialPlateEmpty = null;

        protected override void HandleMovementStop()
        {
            plateServed = true;
            if (!lastMovement)
                AudioPlayer.instance.PlaySFX(plateInSFX);
            OnTutorialPlateStop?.Invoke();
        }

        protected override IEnumerator PlateEmptyWaitingCoroutine()
        {
            OnTutorialPlateEmpty?.Invoke();
            return base.PlateEmptyWaitingCoroutine();
        }

        public void SetLastMovement(float stopDistance)
        {
            lastMovement = true;
            linearMovement.Stop();

            linearMovement.OnWaiting -= SetTimeSlider;

            linearMovement.startingPosition = transform.position;
            linearMovement.stopOffset = stopDistance;
            linearMovement.movementDistance = 4.0f;
            linearMovement.stopTime = 0.01f;
            linearMovement.Move();
        }

        public void TriggerFoodAnimation()
        {
            if (currentFoodToConsume >= foodOnPlate.Length)
                return;

            ((TutorialFood)foodOnPlate[currentFoodToConsume]).ConsumeFoodTutorialFury();
        }
    }
}
