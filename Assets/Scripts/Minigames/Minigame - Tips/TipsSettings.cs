using UnityEngine;

namespace Tips
{
    [CreateAssetMenu(fileName = "TipsSettings", menuName = "ScriptableObjects/Minigames/Tips/TipsSettings")]
    public class TipsSettings : ScriptableObject
    {
        [Header("Coin")]
        [Min(0.0f), Tooltip("The coin speed at the start of the game in oscillations per second")]
        public float startingCoinSpeed = 0.5f;
        [Min(0.0f), Tooltip("The coin speed will be increased by this value at every change of direction")]
        public float coinSpeedIncrease = 0.05f;
        [Min(0.0f), Tooltip("The maximum coin speed achievable")]
        public float coinSpeedLimit = 1.0f;
        
        [Header("Mug")]
        [Min(0.0f), Tooltip("The mug speed at the start of the game in oscillations per second")]
        public float startingMugSpeed = 0.5f;
        [Min(0.0f), Tooltip("The mug speed will be increased by this value at every change of direction")]
        public float mugSpeedIncrease = 0.05f;
        [Min(0.0f), Tooltip("The maximum mug speed achievable")]
        public float mugSpeedLimit = 1.0f;

        [Header("Text Colors")]
        public Color winColor = Color.green;
        public Color gameOverColor = Color.red;
    }
}
