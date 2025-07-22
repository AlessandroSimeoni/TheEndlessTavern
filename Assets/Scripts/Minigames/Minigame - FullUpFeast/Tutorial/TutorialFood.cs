using UnityEngine;

namespace FullUpFeast
{
    public class TutorialFood : Food
    {
        [SerializeField] private Animator[] foodAnimator = new Animator[1];

        private const string ANIMATION_TRIGGER = "Tutorial";

        public void ConsumeFoodTutorialFury()
        {
            if (nextFoodPiece == foodAnimator.Length - 1)
                InvokeFoodConsumed();

            foodAnimator[nextFoodPiece++].SetTrigger(ANIMATION_TRIGGER);
        }
    }
}
