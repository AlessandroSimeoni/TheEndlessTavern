using UnityEngine;

namespace Minigame
{
    [System.Serializable]
    public struct MinigameModifier
    {
        public MinigameSceneIndex minigame;
        public BonusModifier[] modifier;
    }

    [CreateAssetMenu(fileName = "ModifiersSettings", menuName = "ScriptableObjects/Minigames/Modifiers/ModifiersSettings")]
    public class ModifiersSettings : ScriptableObject
    {
        public MinigameModifier[] minigameModifier = new MinigameModifier[0];
    }
}
