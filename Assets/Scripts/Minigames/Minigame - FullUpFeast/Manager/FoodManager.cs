using System.Collections.Generic;
using UnityEngine;

namespace FullUpFeast
{
    public class FoodManager : MonoBehaviour
    {
        [SerializeField] protected Food[] foodPrefab = null;
        [SerializeField] private Vector3 foodDefaultPosition = Vector3.zero;

        protected List<Food> foodAvailable = new List<Food>();


        /// <summary>
        /// Instantiate the required foods in the scene
        /// </summary>
        /// <param name="maxFoodNumber">the maximum possible foods on the plate</param>
        /// <param name="maxTapRequired">the maximum possible number of tap to finish the plate</param>
        public virtual void InstantiateFood(int maxFoodNumber, int maxTapRequired)
        {
            for (int i = 0; i < foodPrefab.Length; i++)
            {
                int spawnQuantity = maxTapRequired / foodPrefab[i].tapRequired;
                if (spawnQuantity > maxFoodNumber)
                    spawnQuantity = maxFoodNumber;

                for (int j = 0; j < spawnQuantity; j++)
                {
                    Food food = Instantiate<Food>(foodPrefab[i], transform);
                    food.gameObject.SetActive(false);
                    food.transform.localPosition = foodDefaultPosition;
                    food.name = $"{foodPrefab[i].name} [{j}]";

                    foodAvailable.Add(food);
                }
            }
        }

        /// <summary>
        /// Deactivate all the food's gameobjects in the scene and move them at a default position
        /// </summary>
        protected void ResetFoodStorage()
        {
            foreach(Food food in foodAvailable)
            {
                food.transform.parent = this.transform;
                food.gameObject.SetActive(false);
                food.transform.localPosition = foodDefaultPosition;
            }
        }

        /// <summary>
        /// Randomly prepare the food that will be put on the plate.
        /// </summary>
        /// <param name="tapRequired">the number of tap required for all the food</param>
        /// <param name="maxFoodNumber">the maximum food number that the plate can support</param>
        /// <returns>An array of Foods</returns>
        public virtual Food[] PrepareFood(int tapRequired, int maxFoodNumber)
        {
            ResetFoodStorage();

            List<Food> foodOutput = new List<Food>();
            List<Food> foodBuffer = new List<Food>();
            int remainingTaps = tapRequired;
            int tap;
            int index;
            int minPossibleTap;

            while (remainingTaps > 0)
            {
                minPossibleTap = GetMinTapRange(remainingTaps, maxFoodNumber);
                tap = Random.Range(minPossibleTap, GetMaxTapRange(remainingTaps) + 1);

                // put all the food's with that tap value in a list
                foreach (Food food in foodAvailable)
                    // if a food is not active, it means that it is available for the choice
                    if (!food.gameObject.activeInHierarchy && food.tapRequired == tap)
                        foodBuffer.Add(food);

                // pick a random food from the buffer list
                index = Random.Range(0, foodBuffer.Count);
                foodOutput.Add(foodBuffer[index]);
                foodBuffer[index].gameObject.SetActive(true);
                foodBuffer[index].ActivateFoodPieces();

                foodBuffer.Clear();
                remainingTaps -= tap;
                maxFoodNumber--;
            }

            return foodOutput.ToArray();
        }


        /// <summary>
        /// Get the maximum number of possible taps that can be chosen considering the 
        /// remaining number of taps 
        /// </summary>
        /// <param name="remainingTaps">the remaining number of taps</param>
        /// <returns>the maximum number of taps that can be chosen</returns>
        private int GetMaxTapRange(int remainingTaps)
        {
            int max = 0;

            foreach(Food food in foodAvailable)
                // if a food is not active, it means that it is available for the choice
                if (!food.gameObject.activeInHierarchy && food.tapRequired <= remainingTaps && food.tapRequired > max)
                    max = food.tapRequired;

            return max;
        }

        /// <summary>
        /// Get the minimum number of possible tap that can be chosen considering the 
        /// remaining number of taps and the remaining number of food slots.
        /// The goal is that the sum of the tapRequired of all the foods on the plate must be 0 avoiding
        /// wasting a food slot with a food that has a tapRequired too low to achieve this.
        /// 
        /// Example (considering foods with a maximum of 3 tapRequired):
        ///     remainingTaps = 6
        ///     remainingFoodSlots = 2
        ///     
        /// --> if a food with 2 tapRequired is chosen:
        ///     remainingTaps = 6 - 2 = 4
        ///     remainingFoodSlots = 2 - 1 = 1
        ///     
        /// --> having foods with a maximum of 3 tapRequired, now we cannot cover all the remainingTaps with only 1 slot;
        ///     we must avoid this situation!
        /// </summary>
        /// <param name="remainingTaps">the number of remaining taps</param>
        /// <param name="remainingFoodSlots">the number of remaining food slots</param>
        /// <returns>the mininum number of taps that can be chosen</returns>
        private int GetMinTapRange(int remainingTaps, int remainingFoodSlots)
        {
            // if remains only 1 slot then return the remainig taps
            if (remainingFoodSlots == 1)
                return remainingTaps;

            // get the maximum number of tap available between the foods
            int maxFoodTapPossible = GetMaxTapRange(remainingTaps);
            int minFoodTapPossible = remainingTaps;

            // get the minimum tap available
            foreach (Food food in foodAvailable)
                // if a food is not active, it means that it is available for the choice
                if (!food.gameObject.activeInHierarchy && food.tapRequired < minFoodTapPossible)
                    minFoodTapPossible = food.tapRequired;

            
            int output = maxFoodTapPossible;
            for (int i = minFoodTapPossible; i <= maxFoodTapPossible; i++)
            {
                if (Mathf.CeilToInt((remainingTaps - i) / (float)(remainingFoodSlots - 1)) > maxFoodTapPossible)
                    continue;

                output = i;
                break;
            }

            return output;
        }
    }
}
