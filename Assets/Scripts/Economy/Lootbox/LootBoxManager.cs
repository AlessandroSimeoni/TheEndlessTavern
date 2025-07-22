using Customization;
using EndlessTavernUI;
using GameSave;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Economy
{
    public class LootBoxManager : MonoBehaviour
    {
        [SerializeField] private LootBoxSettingsContainer settingsContainer = null;
        [SerializeField] private CustomizationUpgradeSettings armorUpgradeSettings = null;
        [SerializeField] private Button continueButton = null;
        [SerializeField] private TextMeshProUGUI quantityTextArea = null;
        [SerializeField] private Image rewardImage = null;
        [SerializeField] private Image chestImage = null;
        [SerializeField] private UIManager uiManager = null;

        private const string ADV_CANVAS = "ADVLootboxCanvas";
        private const string LOOTBOX_CANVAS = "LootboxCanvas";

        public delegate void LootBoxEvent();
        public LootBoxEvent OnLootPullOver = null;

        public delegate void CurrencyPullEvent(Dictionary<Currency, int> currencyReward);
        public event CurrencyPullEvent OnCurrencyPullDone = null;
        
        public delegate void ArmorPullEvent(Dictionary<ArmorID, int> armorPieceReward);
        public event ArmorPullEvent OnArmorPullDone = null;

        private LootBoxSettings currentLootBoxSettings = null;
        private Dictionary<Currency, int> currencyReward = new Dictionary<Currency, int>();
        private Dictionary<ArmorID, int> armorPieceReward = new Dictionary<ArmorID, int>();
        private ArmorID armorIDReward;
        private int currentRewardOnScreen = -1;
        private Currency currentLootBoxSize = Currency.Other;

        public ArmorPieces[] currentPlayerArmorPieces { private get; set; } = null;

        private void ToggleUIElements(bool value)
        {
            continueButton.interactable = value;
            quantityTextArea.enabled = value;
            rewardImage.enabled = value;
        }

        /// <summary>
        /// perform a random pull for the specified lootBoxSize
        /// </summary>
        /// <param name="lootBoxSize">the size of the lootbox</param>
        public void Pull(Currency lootBoxSize)
        {
            ToggleUIElements(false);
            currentRewardOnScreen = -1;
            currentLootBoxSize = lootBoxSize;

            // get correct settings
            int settingsIndex = Array.FindIndex(settingsContainer.lootBoxSize, x => x.size == lootBoxSize);
            currentLootBoxSettings = settingsContainer.lootBoxSize[settingsIndex].settings;
            
            chestImage.sprite = currentLootBoxSettings.openedChestSprite;

            // pull coins
            if (CanPull(currentLootBoxSettings.coinsProbability))
                currencyReward[Currency.Coin] = GetRandomValue((int)currentLootBoxSettings.coinsObtainableRange.min,
                                                               (int)currentLootBoxSettings.coinsObtainableRange.max);

            // pull tickets
            if (CanPull(currentLootBoxSettings.ticketsProbability))
                currencyReward[Currency.Ticket] = GetRandomValue((int)currentLootBoxSettings.ticketsObtainableRange.min,
                                                                 (int)currentLootBoxSettings.ticketsObtainableRange.max);

            // pull gems
            if (CanPull(currentLootBoxSettings.gemsProbability))
                currencyReward[Currency.Gem] = GetRandomValue((int)currentLootBoxSettings.gemsObtainableRange.min,
                                                              (int)currentLootBoxSettings.gemsObtainableRange.max);

            // pull armor pieces
            if (CanPull(currentLootBoxSettings.armorsProbability))
                PullArmorPieces();

            // save pull rewards
            OnCurrencyPullDone?.Invoke(currencyReward);
            if (armorPieceReward.Count > 0)
                OnArmorPullDone?.Invoke(armorPieceReward);

            ShowNextReward();
            ToggleUIElements(true);
        }

        private void PullArmorPieces()
        {
            int rewardQuantity = GetRandomValue((int)currentLootBoxSettings.armorsObtainableRange.min,
                                                (int)currentLootBoxSettings.armorsObtainableRange.max);

            List<ArmorID> allArmorIDs = ((ArmorID[])Enum.GetValues(typeof(ArmorID))).ToList<ArmorID>();
            // remove armor ids if the player already has the max quantity required to unlock customization options
            for (int i = allArmorIDs.Count - 1; i >= 0; i--)
                if (GetArmorQuantityOwned(allArmorIDs[i]) >= GetMaxArmorPiecesRequired(allArmorIDs[i]))
                    allArmorIDs.RemoveAt(i);

            if (allArmorIDs.Count > 0)
            {
                armorIDReward = allArmorIDs[GetRandomValue(0, allArmorIDs.Count - 1)];

                // exceeding armor pieces conversion to gems
                int armorQuantityOwned = GetArmorQuantityOwned(armorIDReward);
                int maxArmorPiecesRequired = GetMaxArmorPiecesRequired(armorIDReward);

                if (armorQuantityOwned + rewardQuantity <= maxArmorPiecesRequired)
                    armorPieceReward[armorIDReward] = rewardQuantity;
                else
                {
                    armorPieceReward[armorIDReward] = maxArmorPiecesRequired - armorQuantityOwned;
                    // exceeding quantity conversion
                    FromArmorPiecesToGemConversion(rewardQuantity - armorPieceReward[armorIDReward]);
                }
            }
            else
                FromArmorPiecesToGemConversion(rewardQuantity);
        }

        /// <summary>
        /// exceeding armor pieces are converted to gems by this method
        /// </summary>
        /// <param name="armorPiecesQuantity">the armor pieces the player owns</param>
        private void FromArmorPiecesToGemConversion(int armorPiecesQuantity)
        {
            if (currencyReward.ContainsKey(Currency.Gem))
                currencyReward[Currency.Gem] += (int)(armorPiecesQuantity / settingsContainer.armorToGemConversionRate);
            else
                currencyReward[Currency.Gem] = (int)(armorPiecesQuantity / settingsContainer.armorToGemConversionRate);
        }

        private int GetArmorQuantityOwned(ArmorID id)
        {
            int armorIndex = Array.FindIndex(currentPlayerArmorPieces, x => x.ID == id);
            return currentPlayerArmorPieces[armorIndex].quantityOwned;
        }

        private int GetMaxArmorPiecesRequired(ArmorID id)
        {
            int armorIndex = Array.FindIndex(armorUpgradeSettings.armorUpgradeInfo, x => x.ID == id);
            int maxArmorPiecesRequired = armorUpgradeSettings.armorUpgradeInfo[armorIndex].materialUpgradeInfo[Enum.GetValues(typeof(ArmorMaterial)).Length - 1].armorPiecesQuantity;
            return maxArmorPiecesRequired;
        }

        public void ShowNextReward()
        {
            bool showArmorReward = false;

            currentRewardOnScreen++;
            while (!currencyReward.ContainsKey((Currency)currentRewardOnScreen))
            {
                if (currentRewardOnScreen >= (int)Currency.Gem)
                {
                    showArmorReward = true;
                    break;
                }
                currentRewardOnScreen++;
            }

            if (showArmorReward)
            {
                if (armorPieceReward.Count > 0)
                {
                    ShowArmorPieceReward();
                    armorPieceReward.Clear();
                }
                else
                    FinishPull();
            }
            else
                ShowCurrencyReward();
        }

        public void HandleADVLootbox()
        {
            uiManager.SwitchCanvas(LOOTBOX_CANVAS);

            if (currentLootBoxSize != Currency.SmallLootBox)
                return;

            uiManager.ShowPopUpCanvas(ADV_CANVAS);
        }

        private void FinishPull()
        {
            currencyReward.Clear();
            armorPieceReward.Clear();
            OnLootPullOver?.Invoke();
            ToggleUIElements(false);
        }

        private void ShowArmorPieceReward()
        {
            int armorRewardIndex = Array.FindIndex(currentLootBoxSettings.armorRewardSprite, x => x.ID == armorIDReward);
            rewardImage.sprite = currentLootBoxSettings.armorRewardSprite[armorRewardIndex].sprite;
            quantityTextArea.text = $"x {armorPieceReward[armorIDReward]}";
        }

        private void ShowCurrencyReward()
        {
            switch ((Currency)currentRewardOnScreen)
            {
                case Currency.Coin:
                    rewardImage.sprite = currentLootBoxSettings.coinRewardSprite;
                    break;
                case Currency.Ticket:
                    rewardImage.sprite = currentLootBoxSettings.ticketRewardSprite;
                    break;
                case Currency.Gem:
                    rewardImage.sprite = currentLootBoxSettings.gemRewardSprite;
                    break;
            }

            quantityTextArea.text = $"x {currencyReward[(Currency)currentRewardOnScreen]}";
        }


        /// <summary>
        /// given a percentage, pick a random number between 0 and 100 and returns true if
        /// this number is lesser or equal than the input percentage, false otherwise.
        /// </summary>
        /// <param name="percentage">the input percentage</param>
        /// <returns>true if a randomly chosen value is lesser or equal than the percentage, false otherwise</returns>
        private bool CanPull(float percentage)
        {
            if (percentage == 0.0f)
                return false;

            return GetRandomValue(0.0f, 100.0f) <= percentage;
        }

        /// <summary>
        /// Get a random integer value between min and max (inclusive)
        /// </summary>
        /// <param name="min">the min value</param>
        /// <param name="max">the max value</param>
        /// <returns>a random integer between min and max (inclusive)</returns>
        private int GetRandomValue(int min, int max) => UnityEngine.Random.Range(min, max+1);
        
        /// <summary>
        /// Get a random float value between min and max (inclusive)
        /// </summary>
        /// <param name="min">the min value</param>
        /// <param name="max">the max value</param>
        /// <returns>a random float between min and max (inclusive)</returns>
        private float GetRandomValue(float min, float max) => UnityEngine.Random.Range(min, max);
    }
}
