using UnityEngine;
using UnityEngine.UI;

namespace EndlessTavernUI
{
    public class HealthBar : MonoBehaviour
    {
        public Image[] heart = new Image[0];
        [SerializeField] private Sprite emptyHeart = null;
        [SerializeField] private Sprite fullHearth = null;

        public int currentHeartIndex { get; set; } = 0;
         

        protected virtual void Start() => currentHeartIndex = heart.Length - 1;

        /// <summary>
        /// remove a full heart when damaged
        /// </summary>
        public void HealthBarDamage()
        {
            if (currentHeartIndex < 0)
                return;

            heart[currentHeartIndex].sprite = emptyHeart;
            currentHeartIndex--;
        }

        /// <summary>
        /// refill quantity hearts
        /// </summary>
        /// <param name="quantity">how many hearts to refill</param>
        public void RefillHearts(int quantity)
        {
            currentHeartIndex = quantity - 1;

            for (int i = 0; i < quantity; i++)
                heart[i].sprite = fullHearth;
        }

        /// <summary>
        /// refill all hearts
        /// </summary>
        public void RefillHearts()
        {
            foreach (Image i in heart)
                i.sprite = fullHearth;
        }
    }
}
