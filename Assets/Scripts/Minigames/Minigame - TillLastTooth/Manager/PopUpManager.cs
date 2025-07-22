using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TillLastTooth
{
    [DisallowMultipleComponent]
    public class PopUpManager : MonoBehaviour
    {
        [Header("Attack Pop Up")]
        [SerializeField] private Image leftPopUpImage = null;
        [SerializeField] private Image rightPopUpImage = null;
        [Min(1), Tooltip("The number of flashes the pop up make before the start of the attack")]
        [SerializeField] private int flashes = 5;
        [Min(0.01f), Tooltip("The interval in seconds between each flash")]
        [SerializeField] private float flashTime = 0.025f;
        [Header("Colors")]
        [SerializeField] private Color baseColor = Color.red;
        [SerializeField] private Color lastFlashColor = Color.yellow;
        [Header("Player Miss Pop Up")]
        [SerializeField] private Image missPopUp = null;
        [SerializeField] private float missPopUpFadeTime = 0.5f;

        public delegate void PopUpEvent();
        public event PopUpEvent OnPopUpDone = null;

        public void ActivatePopUp(Vector3 position, float fadeTime)
        {
            float dot = Vector3.Dot(position, Vector3.right);
            StartCoroutine(ShowPopUp(dot > 0 ? rightPopUpImage : leftPopUpImage, fadeTime));
        }

        /// <summary>
        /// shows the enemy pop up with the fading and blinking effect.
        /// Changes the pop up color on the last blink and when done invoke an event
        /// </summary>
        /// <param name="popUpImage">the pop up image</param>
        /// <param name="fadeTime">the time of fading</param>
        /// <returns></returns>
        private IEnumerator ShowPopUp(Image popUpImage, float fadeTime)
        {
            popUpImage.canvasRenderer.SetAlpha(0.0f);
            popUpImage.gameObject.SetActive(true);
            popUpImage.CrossFadeAlpha(1.0f, fadeTime, false);
            popUpImage.color = baseColor;

            while (popUpImage.canvasRenderer.GetAlpha() < 1.0f)
                yield return null;

            yield return null;

            for (int i = 0; i < flashes; i++)
            {
                if (i == flashes - 1)
                    popUpImage.color = lastFlashColor;

                popUpImage.gameObject.SetActive(false);
                yield return new WaitForSeconds(flashTime);

                popUpImage.gameObject.SetActive(true);
                yield return new WaitForSeconds(flashTime);
            }

            yield return null;

            OnPopUpDone?.Invoke();
        }

        public void HideAllPopUp(float fadeTime)
        {
            leftPopUpImage.CrossFadeAlpha(0.0f, fadeTime, false);
            rightPopUpImage.CrossFadeAlpha(0.0f, fadeTime, false);
        }

        /// <summary>
        /// Shows the "miss" pop up on screen when the player miss the enemy
        /// </summary>
        public void TriggerMissPopUp()
        {
            missPopUp.enabled = false;
            missPopUp.CrossFadeAlpha(1.0f, 0.0f, false);
            missPopUp.enabled = true;
            missPopUp.CrossFadeAlpha(0.0f, missPopUpFadeTime, false);
        }
    }
}
