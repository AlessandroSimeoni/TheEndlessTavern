using EditorScripting;
using UnityEngine;
using static Economy.LootBoxSettings;

namespace Economy
{
    [CreateAssetMenu(fileName = "SpinWheelSettings", menuName = "ScriptableObjects/SpinWheel/SpinWheelSettings")]
    public class SpinWheelSettings : ScriptableObject
    {
        [Header("Armor quantity range")]
        public MinMaxRange armorQuantityRange = new MinMaxRange(1, 100);
        public string[] spinAnimationTrigger = new string[0];
        public Sprite coinRewardSprite;
        public Sprite ticketRewardSprite;
        public Sprite gemRewardSprite;
        public ArmorSprite[] armorRewardSprite = new ArmorSprite[0];
    }
}
