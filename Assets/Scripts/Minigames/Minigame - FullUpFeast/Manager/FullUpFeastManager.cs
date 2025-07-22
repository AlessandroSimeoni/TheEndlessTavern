using Audio;
using Minigame;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace FullUpFeast
{
    [DisallowMultipleComponent]
    public class FullUpFeastManager : MinigameManager
    {
        [SerializeField] protected FoodManager foodManager = null;
        [SerializeField] protected Plate plate = null;
        [SerializeField] private AudioClip furyIncreaseSFX = null;
        [SerializeField] protected AudioClip maxFurySFX = null;
        [SerializeField] private PlayerFullUpFeast player = null;
        [SerializeField] private Animator playerAnimator = null;
        [Serializable] public sealed class DamageEvent : UnityEngine.Events.UnityEvent { }
        public DamageEvent OnDamage = new DamageEvent();

        private const string EAT_STATE_ANIMATION_TRIGGER = "EatState";
        private const string EAT_DAMAGE_ANIMATION_TRIGGER = "EatDamage";
        private const string FURY_DAMAGE_ANIMATION_TRIGGER = "FuryDamage";
        private const string EAT_ANIMATION_BOOL = "Eat";

        private int currentHp = 0;
        private int currentFury = 0;
        private int currentRound = 1;
        private int platesAteInRound = 0;
        private bool gameStarted = false;
        private bool deathByFury = false;

        private FullUpFeastSettings settings { get { return (FullUpFeastSettings)_settings; } }
        private FullUpFeastUIManager uiManager { get { return (FullUpFeastUIManager)_uiManager; } }

        private int platesPerRound
        {
            get
            {
                int round = Mathf.Clamp(currentRound, 1, settings.roundLock);
                
                if (round <= settings.thresholdRoundCount)
                    return settings.startingPlatesPerRound;
                
                return settings.startingPlatesPerRound + 
                       Mathf.CeilToInt((round - settings.thresholdRoundCount) / (float)settings.changePlatesPerRoundRate);
            }
        }

        private int plateEscapeSpeedIndex
        {
            get
            {
                int round = Mathf.Clamp(currentRound, 1, settings.roundLock);

                if (round <= settings.thresholdRoundFirstSpeed)
                  return ((round - 1) / settings.changeSpeedRoundRate) % settings.plateEscapeSettings.Length; 
                
                return (settings.plateEscapeSettings.Length - 1) - ((round - 1) / settings.changeSpeedRoundRate) % (settings.plateEscapeSettings.Length - 1);
            }
        }

        private float alternativePlateEscapeSpeed
        {
            get
            {
                int round = Mathf.Clamp(currentRound, 1, settings.roundLock);

                return settings.startingEscapeSpeed - ((round -1) / settings.alternativeEscapeSpeedRoundRate) * settings.decreaseEscapeSpeedValue;
            }
        }

        private int tapRangeIndex 
        { 
            get { return (currentRound - 1) % settings.roundTapsRange.Length; } 
        }

        private void Awake()
        {
            Assert.IsNotNull<FullUpFeastSettings>(settings, "Minigame settings required");
            Assert.IsNotNull<FoodManager>(foodManager, "Food manager required");
            Assert.IsNotNull<Plate>(plate, "Plate required");
        }

        public override void SetUpMinigame(BonusModifier bonus, bool tournament, int record)
        {
            base.SetUpMinigame(bonus, tournament, record);

            playerAnimator.SetTrigger(EAT_STATE_ANIMATION_TRIGGER);
            player.OnEatAnimationEnd += ResetEatAnimationBool;
            currentHp = tournament ? 1 : settings.hp;
            uiManager.InitPlayerHealthBar(tournament);

            plate.OnEmptyPlate += HandleEmptyPlateTap;
            plate.OnPlateEscaped += HandlePlateEscape;
            plate.OnFoodPieceConsumed += HandleFoodConsumed;
            plate.maxWaitingTime = settings.maxWaitingTime;

            foodManager.InstantiateFood(plate.maxPossibleFoods, (int)settings.roundTapsRange[settings.roundTapsRange.Length - 1].max);

            uiManager.SetAllTexts(FullUpFeastUIManager.SCORE_TEXT_TAG, $"Score: {currentScore}");
        }

        public override void StopMinigame() => plate.Stop();

        public virtual void SetUpFirstPlate()
        {
            if (gameStarted)
                return;

            gameStarted = true;
            PreparePlate();
        }

        /// <summary>
        /// Select a random tap number in the tap range of the current round
        /// </summary>
        /// <returns>the tap number</returns>
        private int SelectTapNumber()
        {
           return Random.Range((int)settings.roundTapsRange[tapRangeIndex].min,
                               (int)settings.roundTapsRange[tapRangeIndex].max + 1);
        }

        private void GetDamage(bool damageFromTap)
        {
            // if it's a damage from tap and bonus must be applied do nothing (ignore damage)
            if (damageFromTap && 
                currentBonus != null && 
                currentBonus.id == BonusID.ChefCompassion && 
                Random.Range(0, 101) <= currentBonus.bonusEvent[1].chancePercentage
                )
            {
                bonusAnimator.SetTrigger(FullUpFeastManager.BONUS_ANIMATION_TRIGGER);
                AudioPlayer.instance.PlaySFX(bonusSFX);
                return;
            }

            OnDamage?.Invoke();
            uiManager.UIDamage();
            AudioPlayer.instance.PlaySFX(playerDamageSFX);

            if (--currentHp == 0)
            {
                ShowGameOver();
                uiManager.SetAllTexts(FullUpFeastUIManager.COIN_REWARD_TEXT_TAG, $"+ {(int)(currentScore / settings.scoreToCoinConversionRate)}");
            }
            else
                plate.ForceEscape();
        }

        public virtual void HandleEmptyPlateTap()
        {
            playerAnimator.SetTrigger(EAT_DAMAGE_ANIMATION_TRIGGER);
            GetDamage(true);
        }

        private void HandlePlateEscape(int leftovers)
        {
            CheckLeftovers(leftovers);
            if (currentHp > 0)
            {
                if (platesAteInRound == platesPerRound)
                    IncrementRound();

                PreparePlate();
            }
        }

        private void IncrementRound()
        {
            currentRound++;
            platesAteInRound = 0;
        }

        protected virtual void HandleFoodConsumed()
        {
            if (!playerAnimator.GetBool(EAT_ANIMATION_BOOL))
                playerAnimator.SetBool(EAT_ANIMATION_BOOL, true);

            currentScore += settings.scorePerFoodPiece;
            uiManager.SetAllTexts(FullUpFeastUIManager.SCORE_TEXT_TAG, $"Score: {currentScore}");
        }

        private void ResetEatAnimationBool() => playerAnimator.SetBool(EAT_ANIMATION_BOOL, false);

        protected virtual void CheckLeftovers(int leftovers)
        {
            if (leftovers == 0)
            {
                platesAteInRound++;
                return;
            }

            // if bonus must be applied return (fury ignored)
            if (currentBonus != null && 
                currentBonus.id == BonusID.ChefCompassion && 
                Random.Range(0, 101) <= currentBonus.bonusEvent[0].chancePercentage
                )
            {
                bonusAnimator.SetTrigger(FullUpFeastManager.BONUS_ANIMATION_TRIGGER);
                AudioPlayer.instance.PlaySFX(bonusSFX);
                return;
            }

            currentFury = settings.furyForEachLeftover ?
                          Mathf.Clamp(currentFury + leftovers, 0, settings.maxFury)
                          : currentFury + 1;

            uiManager.SetFury(currentFury / (float)settings.maxFury);

            if (currentFury == settings.maxFury)
            {
                GetDamage(false);
                playerAnimator.SetTrigger(FURY_DAMAGE_ANIMATION_TRIGGER);
                deathByFury = currentHp == 0;
                currentFury = 0;
                uiManager.SetFury(0.0f);
                AudioPlayer.instance.PlaySFX(maxFurySFX);
            }
            else
                AudioPlayer.instance.PlaySFX(furyIncreaseSFX);
        }

        /// <summary>
        /// Prepare the plate for the current round
        /// </summary>
        protected virtual void PreparePlate()
        {
            float bonusEscapeTime = 0;
            // check if more time bonus must be applied
            if (currentBonus != null && 
                currentBonus.id == BonusID.MoreTime && 
                Random.Range(0, 101) <= currentBonus.bonusEvent[0].chancePercentage
                )
            {
                bonusAnimator.SetTrigger(FullUpFeastManager.BONUS_ANIMATION_TRIGGER);
                bonusEscapeTime = currentBonus.bonusEvent[0].bonusValue;
                AudioPlayer.instance.PlaySFX(bonusSFX);
            }

            if (settings.useAlternativeEscapeSpeedVariation)
                plate.SetEscapeSpeed(alternativePlateEscapeSpeed + bonusEscapeTime);
            else
                plate.SetEscapeSpeed(settings.plateEscapeSettings[plateEscapeSpeedIndex].escapeValue + bonusEscapeTime);

            plate.SetUpFood(foodManager.PrepareFood(SelectTapNumber(), plate.maxPossibleFoods));
        }

        public override void InvokeCoinReward()
        {
            if (currentHp > 0)
                return;

            int coinReward = (int)(currentScore / settings.scoreToCoinConversionRate);
            OnCoinReward?.Invoke(coinReward);
        }

        public override void HealPlayer()
        {
            currentHp = 1;
            uiManager.UIHeal(1);
        }

        public void PreparePlateOnContinue()
        {
            if (deathByFury)
                PreparePlate();
        }
    }
}
