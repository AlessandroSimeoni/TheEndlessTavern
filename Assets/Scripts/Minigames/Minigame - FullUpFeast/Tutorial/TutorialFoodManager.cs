using System.Linq;

namespace FullUpFeast
{
    public class TutorialFoodManager : FoodManager
    {
        public override void InstantiateFood(int maxFoodNumber, int maxTapRequired)
        {
            foodAvailable = foodPrefab.ToList();
        }

        public override Food[] PrepareFood(int tapRequired, int maxFoodNumber)
        {
            ResetFoodStorage();

            switch (tapRequired)
            {
                case 1:
                    foodPrefab[0].gameObject.SetActive(true);
                    foodPrefab[0].ActivateFoodPieces();
                    return new Food[] { foodPrefab[0] };
                case 2:
                    foodPrefab[1].gameObject.SetActive(true);
                    foodPrefab[1].ActivateFoodPieces();
                    return new Food[] { foodPrefab[1] };
                case 3:
                    foodPrefab[2].gameObject.SetActive(true);
                    foodPrefab[2].ActivateFoodPieces();
                    return new Food[] { foodPrefab[2] };
                case 6:
                    foreach (Food f in foodPrefab)
                    {
                        f.gameObject.SetActive(true);
                        f.ActivateFoodPieces();
                    }
                    return new Food[] { foodPrefab[0], foodPrefab[1], foodPrefab[2]};
            }

            return null;
        }
    }
}
