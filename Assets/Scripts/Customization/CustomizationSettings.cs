using GameSave;
using UnityEngine;

namespace Customization
{
    [System.Serializable]
    public struct ArmorSet
    {
        public ArmorID ID;
        public ArmorRenderer armorRenderer;
        public Material[] material;
        public Mesh sweater;
        public Mesh hands;
        public Mesh pants;
        public Mesh boots;
    }

    [CreateAssetMenu(fileName = "CustomizationSettings", menuName = "ScriptableObjects/Customization/CustomizationSettings")]
    public class CustomizationSettings : ScriptableObject
    {
        public ArmorSet[] armorSet = new ArmorSet[0];
    }
}
