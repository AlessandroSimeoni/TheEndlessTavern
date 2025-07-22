using UnityEngine;
using UnityEngine.UI;
using EndlessTavernUI;

namespace BeerNBooze
{
    public class BeerNBoozeUIManager : MinigameUIManager
    {
        [Header("Beer slider")]
        [SerializeField] private Slider beerSlider = null;

        public const string LITERS_TEXT_TAG = "LitersText";

        public void SetBeerSlider(float value)
        {
            beerSlider.value = value;

            if (!beerSlider.gameObject.activeInHierarchy)
                beerSlider.gameObject.SetActive(true);
        }

        public void DisableSlider() => beerSlider.gameObject.SetActive(false);
    }
}
