using GameSave;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Customization;
using Random = UnityEngine.Random;
using Audio;

namespace Economy
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class SpinWheel : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private SpinWheelSettings settings = null;
        [SerializeField] private CustomizationUpgradeSettings armorUpgradeSettings = null;
        [Header("Spin Button")]
        [SerializeField] private CurrencyBasedButton spinButton = null;
        [Header("Buttons to be disabled during spin wheel's animation")]
        [SerializeField] private Button[] buttonToBeDisabled = new Button[0];
        [Header("Animation")]
        [SerializeField] private Animator animator = null;
        [Header("Reward")]
        [SerializeField] private Image rewardImage = null;
        [SerializeField] private TextMeshProUGUI rewardQuantityTextArea = null;
        [Header("Audio")]
        [SerializeField] private AudioClip spinWheelSFX = null;

        public delegate void StartSpinEvent(int price);
        public event StartSpinEvent OnSpin = null;

        public delegate void SpinRewardEvent(Currency rewardCurrency, int reward);
        public event SpinRewardEvent OnReward = null;

        public delegate void ArmorPullEvent(Dictionary<ArmorID, int> armorPieceReward);
        public event ArmorPullEvent OnArmorPullDone = null;

        public delegate void EndSpinEvent();
        public event EndSpinEvent OnEnd = null;

        private const string IDLE_TRIGGER = "BackToIdle";
        private const string ARMOR_SPIN_TRIGGER = "Spin1";

        private Currency rewardCurrency = Currency.Other;
        private List<ArmorID> allArmorIDs = new List<ArmorID>();
        private ArmorID armorPulledID = ArmorID.Armor0;
        private Dictionary<ArmorID, int> armorPieceReward = new Dictionary<ArmorID, int>();

        public ArmorPieces[] currentPlayerArmorPieces { private get; set; } = null;

        private void Start() => animator = GetComponent<Animator>();

        public void SetRewardCurrency(Currency currency)
        {
            switch (currency)
            {
                case Currency.Coin:
                    rewardImage.sprite = settings.coinRewardSprite;
                    break;
                case Currency.Ticket:
                    rewardImage.sprite = settings.ticketRewardSprite;
                    break;
                case Currency.Gem:
                    rewardImage.sprite = settings.gemRewardSprite;
                    break;
                case Currency.Other:
                    armorPulledID = allArmorIDs[Random.Range(0, allArmorIDs.Count)];
                    int armorRewardIndex = Array.FindIndex(settings.armorRewardSprite, x => x.ID == armorPulledID);
                    rewardImage.sprite = settings.armorRewardSprite[armorRewardIndex].sprite;
                    break;
            }

            rewardCurrency = currency;
        }

        public void SetRewardAmount(int amount)
        {
            if (rewardCurrency != Currency.Other)
            {
                rewardQuantityTextArea.text = $"x {amount}";
                OnReward.Invoke(rewardCurrency, amount);
            }
            else
            {
                int rewardQuantity = Random.Range((int)settings.armorQuantityRange.min, (int)settings.armorQuantityRange.max+1);
                int armorQuantityOwned = GetArmorQuantityOwned(armorPulledID);
                int maxArmorPiecesRequired = GetMaxArmorPiecesRequired(armorPulledID);
                if (armorQuantityOwned + rewardQuantity <= maxArmorPiecesRequired)
                    armorPieceReward[armorPulledID] = rewardQuantity;
                else
                    armorPieceReward[armorPulledID] = maxArmorPiecesRequired - armorQuantityOwned;

                rewardQuantityTextArea.text = $"x {armorPieceReward[armorPulledID]}";
                OnArmorPullDone?.Invoke(armorPieceReward);
            }
        }

        /// <summary>
        /// spin wheel is based on a set of animations.
        /// To spin, one animation is randomly chosen and played
        /// </summary>
        public void Spin()
        {
            spinButton.button.interactable = false;
            ToggleButtons(false);

            string animationTrigger = GetRandomAnimationTrigger();

            allArmorIDs = ((ArmorID[])Enum.GetValues(typeof(ArmorID))).ToList<ArmorID>();
            for (int i = allArmorIDs.Count - 1; i >= 0; i--)
                if (GetArmorQuantityOwned(allArmorIDs[i]) >= GetMaxArmorPiecesRequired(allArmorIDs[i]))
                    allArmorIDs.RemoveAt(i);

            // if player already has all armor pieces of all armor sets, for simplicity,
            // armors cannot be pulled anymore and the "Spin2" animation is played instead
            if (animationTrigger == ARMOR_SPIN_TRIGGER && allArmorIDs.Count == 0)
                animationTrigger = "Spin2";

            animator.SetTrigger(animationTrigger);
            OnSpin?.Invoke(-spinButton.price);
            AudioPlayer.instance.PlaySFX(spinWheelSFX);
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

        public void SpinDone() => OnEnd?.Invoke();

        public void BackToSpinWheelMenu()
        {
            animator.SetTrigger(IDLE_TRIGGER);
            ToggleButtons(true);
        }

        private void ToggleButtons(bool value)
        {
            foreach (Button b in buttonToBeDisabled)
                b.interactable = value;
        }

        private string GetRandomAnimationTrigger() => settings.spinAnimationTrigger[Random.Range(0, settings.spinAnimationTrigger.Length)];
    }
}
