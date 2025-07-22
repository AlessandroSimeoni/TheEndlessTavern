using UnityEngine;

namespace Economy
{
    public class ShopButton : CurrencyBasedButton
    {
        [SerializeField] private Currency rewardCurrency;
        [SerializeField, Min(0)] private int rewardQuantity;

        public delegate void ButtonEvent(Currency priceCurrency, int price, Currency rewardCurrency, int rewardQuantity);
        public event ButtonEvent OnButtonClick = null;

        public void HandleClick() => OnButtonClick?.Invoke(priceCurrency, price, rewardCurrency, rewardQuantity);
    }
}
