using System;
using UnityEngine;

namespace BeerNBooze
{
    [CreateAssetMenu(fileName = "BeerNBoozeSettings", menuName = "ScriptableObjects/Minigames/BeerNBooze/GameSettings")]
    public class BeerNBoozeSettings : ScriptableObject
    {
        [Header("Bar Counters Spawn")]
        [Min(0), Tooltip("The number of bar counter in the scene")] 
        public int numberOfBarCounters = 3;
        [Tooltip("The spawn position of the first counter (the counters will be spawn from left to right starting from this position)")] 
        public Vector3 firstSpawnPosition = Vector3.zero;
        [Min(0.0f), Tooltip("The gap in meter between each bar counter")]
        public float barCounterGap = 0.25f;

        [Header("Player")]
        [Min(1)] public int hp = 3;

        [Header("Rounds")]
        [Min(1), Tooltip("Starting from this round the difficulty of the minigame will be fixed")]
        public int roundLock = 32;
        [Header("Liters per round")]
        [Min(0.0f), Tooltip("The initial quantity of liters the player has to drink in order to pass the round (this value increases by 1 every numberOfBarCounters rounds)")]
        public float startingLitersPerRound = 5.0f;

        [Header("Beer")]
        [Tooltip("If true, player can recatch the beer if not empty and in player range after finger release")]
        public bool recatchBeer = false;
        [Header("Drinking Speed")]
        [Min(0.0f), Tooltip("The initial time in seconds to drink deltaLiters of beer (see at the bottom of this options)")]
        public float startingDrinkingSpeed = 2.0f;
        [Min(0.0f), Tooltip("The drinking speed will be decreased by this value")]
        public float drinkingSpeedDelta = 0.1f;
        [Header("Movement Speed")]
        [Min(0.0f), Tooltip("The initial movement speed of the beer")]
        public float startingMovementSpeed = 1.0f;
        [Min(0.0f), Tooltip("The movement speed will be increased by this value")]
        public float movementSpeedDelta = 0.1f;
        [Header("Spawn")]
        [Min(0.0f), Tooltip("The initial time between the spawn of beers")]
        public float startingSpawnDelay = 2.0f;
        [Min(0.0f), Tooltip("The spawn delay will be decreased by this value")]
        public float spawnDelta = 0.1f;
        [Header("Destruction")]
        [Tooltip("The height at which the beer will be destroyed")]
        public float beerDestructionHeight = 0.1f;
        [Header("Liters")]
        [Tooltip("The possible sizes of the beers in liters")]
        public float[] beerSize = new float[3];
        [Tooltip("The liters drank every drinking speed seconds")]
        public float deltaLiters = 0.5f;
        [Tooltip("The score per deltaLiters liters drank")]
        public float deltaScore = 100.0f;

        [Header("Score to coin conversion")]
        [Min(0.01f), Tooltip("player will get 1 coin every scoreToCoinConversionRate score")]
        public float scoreToCoinConversionRate = 20.0f;

        private void OnValidate()
        {
            Array.Sort<float>(beerSize);
        }
    }
}
