using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TillLastTooth
{
    public class DodgeCooldownManager : MonoBehaviour
    {
        /*
         * this class manages the dodge slider (if active in scene)
         */

        [Header("BG Image")]
        [SerializeField] private Image bgImage = null;
        [SerializeField] private Color fromColor = Color.black;
        [SerializeField] private Color toColor = Color.white;
        [SerializeField, Min(0.01f), Tooltip("The time in seconds between fromColor and toColor")]
        private float colorChangeTime = 1.0f;
        [Header("Slider")]
        [SerializeField] private Slider cooldownSlider = null;
        [Header("Text")]
        [SerializeField] private Text dodgeText = null;

        private Coroutine colorCoroutine = null;

        public void EmptyDodgeSlider()
        {
            cooldownSlider.value = 0;
            StopCoroutine(colorCoroutine);
            bgImage.color = fromColor;
            dodgeText.enabled = false;
        }

        public void LoadCooldownSlider(float time) => StartCoroutine(LoadSlider(time));

        public void ForceChangeColor() => colorCoroutine = StartCoroutine(ChangeImageColor());

        private IEnumerator LoadSlider(float time)
        {
            float currentValue = 0.0f;
            while (currentValue < cooldownSlider.maxValue)
            {
                cooldownSlider.value = currentValue;
                currentValue += Time.deltaTime * (cooldownSlider.maxValue / time);
                yield return null;
            }

            cooldownSlider.value = cooldownSlider.maxValue;
            yield return null;

            colorCoroutine = StartCoroutine(ChangeImageColor());
        }

        private IEnumerator ChangeImageColor()
        {
            dodgeText.enabled = true;

            float ratio;
            float period = 2 * colorChangeTime;
            float time = period / 2;
            while (true)
            {
                time = (time + Time.deltaTime) % period;
                ratio = (Mathf.Cos(2 * Mathf.PI * (1/period) * time) + 1) / 2;
                bgImage.color = Color.Lerp(fromColor, toColor, ratio);
                yield return null;
            }
        }
    }
}
