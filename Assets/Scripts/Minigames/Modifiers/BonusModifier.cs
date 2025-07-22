using UnityEngine;

namespace Minigame
{
    public enum BonusID
    {
        None,
        Cheers,
        BeersDodge,
        DoubleDamage,
        OnePunchKnight,
        MoreTime,
        ChefCompassion
    }

    [System.Serializable]
    public class BonusModifier
    {
        [System.Serializable]
        public struct BonusEvent
        {
            [Range(0.0f, 100.0f)] public float chancePercentage;
            public float bonusValue;
        }

        public BonusID id = BonusID.None;
        public Sprite bonusSprite = null;
        public string bonusName = "";
        [TextArea(2, 5)] public string bonusDescription = "";
        public BonusEvent[] bonusEvent = new BonusEvent[0];
    }
}
