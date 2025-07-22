using EditorScripting;
using GameSave;
using UnityEngine;

namespace Economy
{
    [CreateAssetMenu(fileName = "LootBoxSettings", menuName = "ScriptableObjects/Shop/LootBoxSettings")]
    public class LootBoxSettings : ScriptableObject
    {
        [System.Serializable]
        public struct ArmorSprite
        {
            public ArmorID ID;
            public Sprite sprite;
        }

        [Header("Chest")]
        public Sprite openedChestSprite = null;

        [Header("Coins")]
        public Sprite coinRewardSprite = null;
        [Range(0, 100)] public float coinsProbability = 100.0f;
        public MinMaxRange coinsObtainableRange = new MinMaxRange(0.0f, 2000.0f) { min = 20, max = 50 };

        [Header("Tickets")]
        public Sprite ticketRewardSprite = null;
        [Range(0, 100)] public float ticketsProbability = 100.0f;
        public MinMaxRange ticketsObtainableRange = new MinMaxRange(0.0f, 50.0f) { min = 1, max = 2 };

        [Header("Gems")]
        public Sprite gemRewardSprite = null;
        [Range(0, 100)] public float gemsProbability = 100.0f;
        public MinMaxRange gemsObtainableRange = new MinMaxRange(0.0f, 200.0f) { min = 1, max = 5 };

        [Header("Armor Pieces")]
        public ArmorSprite[] armorRewardSprite = new ArmorSprite[0];
        [Range(0, 100)] public float armorsProbability = 100.0f;
        public MinMaxRange armorsObtainableRange = new MinMaxRange(0.0f, 100.0f) { min = 5, max = 10 };
    }
}
