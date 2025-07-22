using Audio;
using Minigame;
using Movement;
using System;
using System.Collections;
using UnityEngine;

namespace Tips
{
    public class TipsManager : MinigameManager
    {
        [Header("ModifierSettings")]
        [SerializeField] private ModifiersSettings modSettings = null;
        [Header("Coin & Mug")]
        [SerializeField] private OscillatoryMovement coinMovement = null;
        [SerializeField] private PhysicsOscillatoryMovement mugMovement = null;
        [Header("Trigger areas")]
        [SerializeField] private TipsTriggerArea gameOverArea = null;
        [SerializeField] private TipsTriggerArea winArea = null;
        [Header("End Game")]
        [SerializeField] private float waitTimeAfterEndGame = 2.0f;
        [Header("Audio")]
        [SerializeField] private AudioClip mugDirectionChangeSFX = null;
        [SerializeField] private AudioClip coinOutSFX = null;
        [SerializeField] private AudioClip coinInSFX = null;

        public delegate void EndGame(BonusModifier bonusMod);
        public event EndGame OnEndGame = null;
        public int minigameRequestedBuildIndex { private get; set; } = 3;

        private TipsSettings settings { get { return (TipsSettings) _settings; } }
        private TipsUIManager uiManager { get { return (TipsUIManager)_uiManager; } }

        private BonusModifier bonusMod 
        {
            get 
            {
                return modSettings.minigameModifier[minigameModifierIndex].modifier[currentModifierIndex]; 
            } 
        }

        private int minigameModifierIndex = 0;
        private int currentModifierIndex = 0;

        public override void SetUpMinigame(BonusModifier bonus, bool tournament, int record)
        {
            coinMovement.OnDirectionChange += HandleCoinDirectionChange;
            coinMovement.oscillationsPerSec = settings.startingCoinSpeed;

            mugMovement.OnDirectionChange += HandleMugDirectionChange;
            mugMovement.oscillationsPerSec = settings.startingMugSpeed;

            gameOverArea.OnCoinDestruction += HandleGameOver;
            winArea.OnCoinDestruction += HandleWin;

            minigameModifierIndex = Array.FindIndex<MinigameModifier>(modSettings.minigameModifier,
                                                              x => x.minigame == (MinigameSceneIndex)minigameRequestedBuildIndex);

            SetModifierUI();
        }

        public override void StartMinigame()
        {
            coinMovement.move = true;
            mugMovement.move = true;
        }

        public override void StopMinigame() => mugMovement.move = false;

        private void HandleCoinDirectionChange()
        {
            if (coinMovement.oscillationsPerSec < settings.coinSpeedLimit)
                coinMovement.oscillationsPerSec = Mathf.Clamp(coinMovement.oscillationsPerSec + settings.coinSpeedIncrease, settings.startingCoinSpeed, settings.coinSpeedLimit);
        }

        private void HandleMugDirectionChange()
        {
            if (mugMovement.oscillationsPerSec < settings.mugSpeedLimit)
                mugMovement.oscillationsPerSec = Mathf.Clamp(mugMovement.oscillationsPerSec + settings.mugSpeedIncrease, settings.startingMugSpeed, settings.mugSpeedLimit);

            currentModifierIndex = (currentModifierIndex + 1) % modSettings.minigameModifier[minigameModifierIndex].modifier.Length;
            SetModifierUI();
            AudioPlayer.instance.PlaySFX(mugDirectionChangeSFX);
        }

        private void SetModifierUI()
        {
            uiManager.SetText(TipsUIManager.MOD_NAME_TAG, bonusMod.bonusName);
            uiManager.SetText(TipsUIManager.MOD_DESC_TAG, bonusMod.bonusDescription);
            uiManager.SetModifierSprite(bonusMod.bonusSprite);
        }

        private void HandleWin()
        {
            AudioPlayer.instance.PlaySFX(coinInSFX);
            HandleEndGame(settings.winColor, bonusMod);
        }

        private void HandleGameOver()
        {
            AudioPlayer.instance.PlaySFX(coinOutSFX);
            HandleEndGame(settings.gameOverColor, null);
        }

        private void HandleEndGame(Color textColor, BonusModifier bonus)
        {
            mugMovement.OnDirectionChange -= HandleMugDirectionChange;
            uiManager.SetTextColor(TipsUIManager.MOD_NAME_TAG, textColor);
            uiManager.SetTextColor(TipsUIManager.MOD_DESC_TAG, textColor);
            uiManager.SetColorSprite(textColor);
            StartCoroutine(EndGameCoroutine(bonus));
        }

        private IEnumerator EndGameCoroutine(BonusModifier bonus)
        {
            yield return new WaitForSeconds(waitTimeAfterEndGame);
            OnEndGame?.Invoke(bonus);
        }

        public override void InvokeCoinReward() { } // do nothing

        public override void HealPlayer() { } // do nothing
    }
}
