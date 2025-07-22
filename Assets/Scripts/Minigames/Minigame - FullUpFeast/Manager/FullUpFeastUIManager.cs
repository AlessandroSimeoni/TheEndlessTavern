using EndlessTavernUI;
using UnityEngine;
using UnityEngine.UI;

namespace FullUpFeast
{
    public class FullUpFeastUIManager : MinigameUIManager
    {
        [Header("Chef's Fury")]
        [SerializeField] private Slider furySlider = null;

        public void SetFury(float value) => furySlider.value = value;
    }
}
