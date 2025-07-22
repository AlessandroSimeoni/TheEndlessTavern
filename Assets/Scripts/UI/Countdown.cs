using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace EndlessTavernUI
{
    public class Countdown : MonoBehaviour
    {
        [SerializeField] private int countdownLength = 3;
        [SerializeField, Min(0.1f), Tooltip("The time in seconds between each countdown digit")]
        private float countdownTime = 0.5f;
        [SerializeField] private TextMeshProUGUI countdownText = null;
        [Serializable] public sealed class CountdownEvent : UnityEngine.Events.UnityEvent { }
        public CountdownEvent OnCountdownOver = new CountdownEvent();

        private void OnEnable() => StartCoroutine(CountdownCoroutine(countdownLength));

        private IEnumerator CountdownCoroutine(int countdownLength)
        {
            countdownText.text = countdownLength.ToString();
            for (int i = countdownLength - 1; i >= 0; i--)
            {
                yield return new WaitForSecondsRealtime(countdownTime);
                countdownText.text = i.ToString();
            }

            OnCountdownOver?.Invoke();
        }
    }
}
