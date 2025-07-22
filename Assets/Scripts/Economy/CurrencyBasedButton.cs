using UnityEngine;
using UnityEngine.UI;

namespace Economy
{
    public class CurrencyBasedButton : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private Currency _priceCurrency = Currency.Coin;
        [SerializeField] private int _price = 100;

        public int price { get { return _price; } }
        public Button button { get { return _button; } }
        public Currency priceCurrency { get { return _priceCurrency; } }
    }
}
