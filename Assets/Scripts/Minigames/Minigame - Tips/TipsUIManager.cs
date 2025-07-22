using EndlessTavernUI;
using TMPro;
using UnityEngine;

namespace Tips
{
    public class TipsUIManager : MinigameUIManager
    {
        public const string MOD_NAME_TAG = "ModifierName";
        public const string MOD_DESC_TAG = "ModifierDescription";

        public void SetTextColor(string targetTextTag, Color color)
        {
            TextMeshProUGUI targetTextArea = FindTargetElement<TextMeshProUGUI>(textArea, targetTextTag);

            if (targetTextArea == null)
                return;

            targetTextArea.color = color;
        }

        public void SetColorSprite(Color color) => modifierImage.color = color;
    }
}
