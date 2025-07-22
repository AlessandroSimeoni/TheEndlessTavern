using UnityEngine;

namespace TillLastTooth
{
    [CreateAssetMenu(fileName = "TillLastToothSettings", menuName = "ScriptableObjects/Minigames/TillLastTooth/GameSettings")]
    public class TillLastToothSettings : ScriptableObject
    {
        [Header("Rounds")]
        [Min(1), Tooltip("Starting from this round the difficulty of the minigame will be fixed")]
        public int roundLock = 21;
        [Tooltip("Number of enemies to defeat in order to complete each round")]
        public int enemiesPerRound = 1;

        [Header("Left/Right")]
        [Min(0.1f), Tooltip("The offset from x=0 to determine the left and right positions")]
        public float leftRightOffset = 1.0f;

        [Header("Score")]
        [Min(0), Tooltip("The score for each punch delivered with success to the enemy")]
        public int scorePerPunch = 50;

        [Header("Player damage detection offset")]
        [Min(0.0f), Tooltip("Player will be damaged if he hasn't dodged in the right direction for at least this number of meters")]
        public float damageDetectionOffset = 0.0f;

        [Header("Score to coin conversion")]
        [Min(0.01f), Tooltip("player will get 1 coin every scoreToCoinConversionRate score")]
        public float scoreToCoinConversionRate = 20.0f;
    }
}
