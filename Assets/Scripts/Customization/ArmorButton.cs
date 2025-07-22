using GameSave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Customization
{
    public class ArmorButton : CustomizationButton
    {
        [SerializeField] private ArmorID _armorID = ArmorID.Armor0;
        [SerializeField] private Slider slider = null;
        [SerializeField] private TextMeshProUGUI sliderTextArea = null;

        public ArmorID armorID { get { return _armorID; } }

        public void SetSlider(float value, string text)
        {
            slider.value = value;
            sliderTextArea.text = text;
        }
    }
}
