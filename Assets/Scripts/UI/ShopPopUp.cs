using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessTavernUI
{
    [DisallowMultipleComponent]
    public class ShopPopUp : MonoBehaviour
    {
        [SerializeField] private RectTransform paymentObject = null;
        [SerializeField] private Image rewardImage = null;
        [SerializeField] private TextMeshProUGUI rewardQuantity = null;

        public delegate void ConfirmEvent();
        public event ConfirmEvent OnPurchaseConfirmed = null;

        private GameObject paymentObjectInstance = null;

        /// <summary>
        /// set the payment object in the shop confirm pop up canvas.
        /// This takes the button with the price gameobject and clones it in the pop up canvas
        /// </summary>
        /// <param name="paymentObject"></param>
        public void SetPaymentObject(GameObject paymentObject)
        {
            paymentObjectInstance = Instantiate(paymentObject, this.paymentObject);

            Button button = paymentObjectInstance.GetComponent<Button>();
            if (button != null)
                Destroy(button);

            RectTransform rect = paymentObjectInstance.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.localPosition = Vector3.zero;
        }

        public void SetRewardImage(Image image) => rewardImage.sprite = image.sprite;

        public void SetRewardQuantity(int quantity) => rewardQuantity.text = quantity > 0 ? $"x {quantity}" : "";

        public void ConfirmPurchase() => OnPurchaseConfirmed?.Invoke();

        private void OnDisable() => Destroy(paymentObjectInstance);
    }
}
