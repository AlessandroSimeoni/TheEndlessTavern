using Audio;
using EndlessTavernUI;
using GameSave;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private LootBoxManager lootBoxManager = null;
        [SerializeField] private ShopPopUp[] shopPopUp = null;
        [SerializeField] private ShopButton[] shopButton = new ShopButton[0];
        [SerializeField] private AudioClip standardButtonSFX = null;

        public delegate void PurchaseEvent(Currency currency, int price, Currency rewardCurrency, int rewardQuantity);
        public event PurchaseEvent OnPurchase = null;

        public delegate void LootBoxEvent(Dictionary<Currency, int> lootBoxReward);
        public event LootBoxEvent OnLootBoxReward = null;

        public delegate void LootBoxArmorEvent(Dictionary<ArmorID, int> lootBoxArmorReward);
        public event LootBoxArmorEvent OnLootBoxArmorReward = null;

        public delegate void LootBoxOverEvent();
        public event LootBoxOverEvent OnLootBoxOver = null;

        public int playerCoins { private get; set; } = 0;
        public int playerTickets { private get; set; } = 0;
        public int playerGems { private get; set; } = 0;
        public ArmorPieces[] playerArmorPieces { private get; set; } = null;

        private Currency priceCurrency;
        private int price;
        private Currency rewardCurrency;
        private int rewardQuantity;

        private void OnEnable() => Refresh();

        private void Start()
        {
            lootBoxManager.OnCurrencyPullDone += HandleLootBoxCurrencyPull;
            lootBoxManager.OnArmorPullDone += HandleLootBoxArmorPull;
            lootBoxManager.OnLootPullOver += HandleLootBoxPullOver;

            foreach (ShopPopUp popUp in shopPopUp)
                popUp.OnPurchaseConfirmed += HandlePurchaseConfirmed;

            foreach (ShopButton button in shopButton)
                button.OnButtonClick += HandlePrePurchaseState;
        }

        private void HandleLootBoxArmorPull(Dictionary<ArmorID, int> armorPieceReward) => OnLootBoxArmorReward?.Invoke(armorPieceReward);

        private void HandleLootBoxPullOver() => OnLootBoxOver?.Invoke();

        private void HandleLootBoxCurrencyPull(Dictionary<Currency, int> currencyLootBoxReward) => OnLootBoxReward?.Invoke(currencyLootBoxReward);

        private void HandlePrePurchaseState(Currency priceCurrency, int price, Currency rewardCurrency, int rewardQuantity)
        {
            AudioPlayer.instance.PlaySFX(standardButtonSFX);
            this.priceCurrency = priceCurrency;
            this.price = price;
            this.rewardCurrency = rewardCurrency;
            this.rewardQuantity = rewardQuantity;
        }

        private void HandlePurchaseConfirmed()
        {
            OnPurchase?.Invoke(priceCurrency, price, rewardCurrency, rewardQuantity);

            if (rewardCurrency >= Currency.SmallLootBox && rewardCurrency <= Currency.LargeLootBox)
            {
                lootBoxManager.currentPlayerArmorPieces = playerArmorPieces;
                lootBoxManager.Pull(rewardCurrency);
            }
        }

        /// <summary>
        /// refresh the shop toggling the buttons
        /// </summary>
        public void Refresh() => shopButton.Toggle(playerCoins, playerTickets, playerGems);
    }
}
