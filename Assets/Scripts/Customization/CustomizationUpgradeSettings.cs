using GameSave;
using System;
using UnityEngine;

namespace Customization
{
    public enum ArmorMaterial
    {
        Bronze,
        Silver,
        Gold,
        Diamond
    }

    [System.Serializable]
    public struct ArmorUpgradeInfo
    {
        public ArmorID ID;
        public MaterialUpgradeInfo[] materialUpgradeInfo;
    }

    [System.Serializable]
    public struct MaterialUpgradeInfo
    {
        public ArmorMaterial material;
        public int price;
        public int armorPiecesQuantity;
    }

    [CreateAssetMenu(fileName = "CustomizationUpgradeSettings", menuName = "ScriptableObjects/Customization/CustomizationUpgradeSettings")]
    public class CustomizationUpgradeSettings : ScriptableObject
    {
        public ArmorUpgradeInfo[] armorUpgradeInfo = new ArmorUpgradeInfo[0];

        private void OnValidate()
        {
            foreach(ArmorUpgradeInfo info in armorUpgradeInfo)
                Array.Sort<MaterialUpgradeInfo>(info.materialUpgradeInfo, (x, y) => x.material.CompareTo(y.material));
        }
    }
}
