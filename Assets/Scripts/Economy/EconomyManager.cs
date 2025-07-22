using Audio;
using Customization;
using EndlessTavernUI;
using GameSave;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Economy
{
    public enum Currency
    {
        Coin,
        Ticket,
        Gem,
        SmallLootBox,
        MediumLootBox,
        LargeLootBox,
        Other
    }

    public class EconomyManager : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager = null;
        [SerializeField] private ShopManager shopManager = null;
        [SerializeField] private SpinWheel spinWheel = null;
        [SerializeField] private CustomizationManager customizationManager = null;
        [SerializeField] private CurrencyBasedButton[] currencyButton = new CurrencyBasedButton[0];
        [SerializeField] private Button[] topScreenShopButton = new Button[3];
        [Header("Audio")]
        [SerializeField] private AudioClip purchaseSFX = null;

        private GameData gameData = null;

        private const string COIN_TEXT_TAG = "PlayerCoinsText";
        private const string TICKET_TEXT_TAG = "PlayerTicketsText";
        private const string GEMS_TEXT_TAG = "PlayerGemsText";

        private void Awake()
        {
            Assert.IsNotNull<ShopManager>(shopManager, "Missing shop manager");
            Assert.IsNotNull<UIManager>(uiManager, "Missing ui manager");
        }

        /// <summary>
        /// update the shop manager setting the amount of each currency owned by the player
        /// </summary>
        private void UpdateShopManager()
        {
            shopManager.playerCoins = gameData.playerCoins;
            shopManager.playerTickets = gameData.playerTickets;
            shopManager.playerGems = gameData.playerGems;
            shopManager.playerArmorPieces = gameData.playerArmorPieces;
        }

        /// <summary>
        /// get the data saved and update the shop manager
        /// </summary>
        /// <param name="gameData">the data saved</param>
        public void GetGameData(GameData gameData)
        {
            this.gameData = gameData;
            RefreshUICurrency();
            ToggleTopScreenShopButtons();

            shopManager.OnPurchase += HandlePurchase;
            shopManager.OnLootBoxReward += HandleLootBoxReward;
            shopManager.OnLootBoxOver += HandleLootBoxOver;
            shopManager.OnLootBoxArmorReward += HandleArmorReward;
            UpdateShopManager();

            UpdateSpinWheel();

            currencyButton.Toggle(gameData.playerCoins, gameData.playerTickets, gameData.playerGems);
        }

        private void UpdateSpinWheel()
        {
            if (spinWheel == null)
                return;
            
            spinWheel.OnSpin += AddPlayerTickets;
            spinWheel.OnEnd += HandleSpinWheelEnd;
            spinWheel.OnReward += HandleSpinWheelReward;
            spinWheel.OnArmorPullDone += HandleArmorReward;
            spinWheel.currentPlayerArmorPieces = gameData.playerArmorPieces;
        }

        private void ToggleTopScreenShopButtons()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            foreach (Button b in topScreenShopButton)
                b.interactable = false;
        }

        private void HandleSpinWheelReward(Currency rewardCurrency, int reward)
        {
            AddCurrencyAmount(rewardCurrency, reward);
            gameData.Save();
        }

        private void HandleSpinWheelEnd()
        {
            RefreshUICurrency();
            RefreshShop();
            spinWheel.currentPlayerArmorPieces = gameData.playerArmorPieces;
            currencyButton.Toggle(gameData.playerCoins, gameData.playerTickets, gameData.playerGems);
        }

        private void HandleArmorReward(Dictionary<ArmorID, int> armorReward)
        {
            customizationManager.AddArmorPieces(armorReward);
        }

        private void RefreshUICurrency()
        {
            uiManager.SetAllTexts(COIN_TEXT_TAG, gameData.playerCoins.ToString());
            uiManager.SetAllTexts(TICKET_TEXT_TAG, gameData.playerTickets.ToString());
            uiManager.SetAllTexts(GEMS_TEXT_TAG, gameData.playerGems.ToString());
        }

        private void HandlePurchase(Currency priceCurrency, int price, Currency rewardCurrency, int rewardQuantity)
        {
            //pay
            AddCurrencyAmount(priceCurrency, -price);

            //get reward
            AddCurrencyAmount(rewardCurrency, rewardQuantity);

            gameData.Save();
            RefreshUICurrency();
            RefreshShop();
            currencyButton.Toggle(gameData.playerCoins, gameData.playerTickets, gameData.playerGems);
        }

        private void HandleLootBoxReward(Dictionary<Currency, int> lootCurrencyReward)
        {
            foreach (KeyValuePair<Currency, int> kvp in lootCurrencyReward)
                AddCurrencyAmount(kvp.Key, kvp.Value);

            gameData.Save();
        }

        /// <summary>
        /// called when the lootbox pull has ended
        /// </summary>
        private void HandleLootBoxOver()
        {
            RefreshUICurrency();
            RefreshShop();

            if (spinWheel != null)
                spinWheel.currentPlayerArmorPieces = gameData.playerArmorPieces;

            currencyButton.Toggle(gameData.playerCoins, gameData.playerTickets, gameData.playerGems);
            uiManager.ShowPreviousCanvas();
        }

        /// <summary>
        /// add the amount to the currency without saving the game
        /// </summary>
        /// <param name="currency">the currency</param>
        /// <param name="amount">the amount to add</param>
        private void AddCurrencyAmount(Currency currency, int amount)
        {
            switch (currency)
            {
                case Currency.Coin:
                    gameData.playerCoins += amount;
                    break;
                case Currency.Ticket:
                    gameData.playerTickets += amount;
                    break;
                case Currency.Gem:
                    gameData.playerGems += amount;
                    break;
            }
        }

        /// <summary>
        /// Increase by amount value the coins owned by the player and save the game
        /// used in the main menu by the bonus buttons
        /// </summary>
        /// <param name="amount">the amount to add</param>
        public void AddPlayerCoins(int amount)
        {
            AddCurrencyAmount(Currency.Coin, amount);
            gameData.Save();
            RefreshUICurrency();
        }

        private void AddPlayerTickets(int amount)
        {
            AddCurrencyAmount(Currency.Ticket, amount);
            gameData.Save();
            RefreshUICurrency();
        }

        public void AddPlayerGems(int amount)
        {
            AddCurrencyAmount(Currency.Gem, amount);
            gameData.Save();
            RefreshUICurrency();
            RefreshShop();
        }

        private void RefreshShop()
        {
            UpdateShopManager();
            shopManager.Refresh();
        }

        public void PlayPurchaseSFX() => AudioPlayer.instance.PlaySFX(purchaseSFX);
    }
}
