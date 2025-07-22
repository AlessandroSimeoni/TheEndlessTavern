using EditorScripting;
using System;
using UnityEngine;

namespace FullUpFeast
{
    public enum PlateEscapeSpeedTag
    {
        SLOW,
        MEDIUM,
        FAST
    }

    [CreateAssetMenu(fileName = "FullUpFeastSettings", menuName = "ScriptableObjects/Minigames/FullUpFeast/GameSettings")]
    public class FullUpFeastSettings : ScriptableObject
    {
        [System.Serializable]
        public struct PlateEscapeSettings
        {
            [Tooltip("The escape speed tag")]
            public PlateEscapeSpeedTag escapeSpeedTag;
            [Min(0.1f), Tooltip("The escape value in seconds of the plate (lesser = faster)")] 
            public float escapeValue;
        }

        [Header("Player")]
        [Min(1)] public int hp = 3;

        [Header("Rounds")]
        [Tooltip("Min and max range taps per round")]
        public MinMaxRange[] roundTapsRange = new MinMaxRange[3];
        [Min(1),Tooltip("Starting from this round the difficulty of the minigame will be fixed")]
        public int roundLock = 21;

        [Header("Plates")]
        [Header("Plate Escape Speed")]
        [Tooltip("Plate Escape Settings")]
        public PlateEscapeSettings[] plateEscapeSettings = new PlateEscapeSettings[3];
        [Min(1), Tooltip("After this number of rounds the first speed in the list above will be ignored")]
        public int thresholdRoundFirstSpeed = 9;
        [Min(1), Tooltip("The speed changes every this number of rounds")]
        public int changeSpeedRoundRate = 3;
        [Min(0), Tooltip("The maximum waiting time before the plate escape after eating all the food")]
        public float maxWaitingTime = 1.0f;

        [Header("---Alternative plate escape speed---")]
        [Tooltip("If true then use this alternative method to calculate the escape speed of the plate")]
        public bool useAlternativeEscapeSpeedVariation = false;
        [Min(0.1f), Tooltip("The starting escape speed of the plate")] 
        public float startingEscapeSpeed = 1.5f;
        [Min(0.0001f), Tooltip("The escape speed will be decreased by this value")]
        public float decreaseEscapeSpeedValue = 0.025f;
        [Min(1), Tooltip("The escape speed will be decreased every this number of rounds")]
        public int alternativeEscapeSpeedRoundRate = 1;

        [Header("Plates per round")]
        [Tooltip("Starting number of plates to eat in order to complete the round")]
        public int startingPlatesPerRound = 3;
        [Min(1), Tooltip("For this number of rounds (included) the number of plates needed to pass the round doesn't change")]
        public int thresholdRoundCount = 9;
        [Min(1), Tooltip("The number of plates to eat changes every this number of rounds but after the threshold above")]
        public int changePlatesPerRoundRate = 6;

        [Header("Food")]
        [Min(0.1f), Tooltip("Score for each piece of food the player eats")] 
        public int scorePerFoodPiece = 25;

        [Header("Chef's fury")]
        [Tooltip("The max value of the chef's fury")]
        public int maxFury= 5;
        [Tooltip("If TRUE, each leftover will contribute to increase the chef's fury. If FALSE, the chef's fury will increase by 1 if the plate contains leftovers.")]
        public bool furyForEachLeftover = false;

        [Header("Score to coin conversion")]
        [Min(0.01f), Tooltip("player will get 1 coin every scoreToCoinConversionRate score")]
        public float scoreToCoinConversionRate = 20.0f;

        private void OnValidate()
        {
            Array.Sort<PlateEscapeSettings>(plateEscapeSettings, (x, y) => x.escapeSpeedTag.CompareTo(y.escapeSpeedTag));

            foreach (MinMaxRange slider in roundTapsRange)
                slider.rangeMin = 1.0f;
        }
    }
}
