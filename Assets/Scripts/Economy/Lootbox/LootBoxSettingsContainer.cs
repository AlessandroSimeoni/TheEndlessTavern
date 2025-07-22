using System;
using UnityEngine;

namespace Economy
{
    [CreateAssetMenu(fileName = "LootBoxSettingsContainer", menuName = "ScriptableObjects/Shop/LootBoxSettingsContainer")]
    public class LootBoxSettingsContainer : ScriptableObject
    {
        [Serializable]
        public struct LootBoxSize
        {
            public Currency size;
            public LootBoxSettings settings;
        }

        public LootBoxSize[] lootBoxSize = new LootBoxSize[3];

        [Header("Armor pieces to gem conversion")]
        [Min(0.01f), Tooltip("player will get 1 gem every armorToGemConversionRate armor exceeding piece")]
        public float armorToGemConversionRate = 1;
    }
}
