using EditorScripting;
using System;
using UnityEngine;

namespace FullUpFeast
{

    [CreateAssetMenu(fileName = "FoodPositionSettings", menuName = "ScriptableObjects/Minigames/FullUpFeast/FoodPositionSettings")]
    public class FoodPositionSettings : ScriptableObject
    {
        [System.Serializable]
        public struct FoodPosition
        {
            [Min(1), Tooltip("The number of food on plate")]
            public int numberOfFoodOnPlate;
            [Tooltip("The position of each food on plate")]
            public Vector3[] position;
        }

        [Header("Food positions on plate")]
        [Tooltip("List of food number with their positions on plate")]
        public FoodPosition[] foodPosition = new FoodPosition[3];

        [Header("Food rotations")]
        [Header("The possible rotation range around the X axis")]
        public MinMaxRange xAxisRotation = new MinMaxRange(-90.0f, 90.0f);
        [Header("The possible rotation range around the Y axis")]
        public MinMaxRange yAxisRotation = new MinMaxRange(-180.0f, 180.0f);
        [Header("The possible rotation range around the Z axis")]
        public MinMaxRange zAxisRotation = new MinMaxRange(-90.0f, 90.0f);

        private void OnValidate()
        {
            Array.Sort<FoodPosition>(foodPosition, (x, y) => x.numberOfFoodOnPlate.CompareTo(y.numberOfFoodOnPlate));
        }
    }
}
