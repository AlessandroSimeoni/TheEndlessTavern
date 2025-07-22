using UnityEngine;

namespace FullUpFeast
{
    [DisallowMultipleComponent]
    public class Food : MonoBehaviour
    {
        [SerializeField, Tooltip("Array containing each single piece of food")] 
        private GameObject[] foodPiece = new GameObject[1];
        [Header("Rotation ")]
        [Tooltip("If TRUE the food can be rotated around the X axis when positioned on the plate")]
        public bool xRotationEnabled = false;
        [Tooltip("If TRUE the food can be rotated around the Y axis when positioned on the plate")]
        public bool yRotationEnabled = false;
        [Tooltip("If TRUE the food can be rotated around the Z axis when positioned on the plate")]
        public bool zRotationEnabled = false;
        
        public int tapRequired { get { return foodPiece.Length; } } // number of tap necessary to eat all the food pieces

        public int leftovers
        {
            get
            {
                int output = 0;
                for (int i = 0; i < foodPiece.Length; i++)
                    if (foodPiece[i].activeInHierarchy)
                        output++;
                return output;
            }
        }

        public delegate void FoodConsumed();
        public event FoodConsumed OnFoodConsumed = null;

        protected int nextFoodPiece = 0;

        /// <summary>
        /// Eat the food at the nextFoodPiece index in the foodPiece array
        /// </summary>
        public void Consume()
        {
            // if the food is completely consumed, invoke the event
            if (nextFoodPiece == foodPiece.Length - 1)
                OnFoodConsumed?.Invoke();

            // eat a food piece
            foodPiece[nextFoodPiece++].SetActive(false);
        }

        protected void InvokeFoodConsumed() => OnFoodConsumed?.Invoke();

        public void ActivateFoodPieces()
        {
            nextFoodPiece = 0;
            foreach (GameObject go in foodPiece)
                go.SetActive(true);
        }
    }
}
