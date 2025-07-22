using UnityEngine;

namespace Tips
{
    public class TipsTriggerArea : MonoBehaviour
    {
        private const string COIN_TAG = "Coin";

        public delegate void CoinDestruction();
        public event CoinDestruction OnCoinDestruction = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(COIN_TAG))
            {
                Destroy(other.gameObject);
                OnCoinDestruction?.Invoke();
            }
        }
    }
}
