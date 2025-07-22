using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Assertions;
using Minigame;
using Audio;

namespace BeerNBooze
{
    [DisallowMultipleComponent]
    public class BeerNBoozeManager : MinigameManager
    {
        [SerializeField] private BarCounter barCounterPrefab = null;
        [SerializeField] private Animator playerAnimator = null;

        [Serializable] public sealed class DamageEvent : UnityEngine.Events.UnityEvent { }
        public DamageEvent OnDamage = new DamageEvent();

        protected BarCounter[] barCounter = null;
        private List<BarCounter> barCounterToBoost = new List<BarCounter>();
        private Coroutine beerSpawnCoroutine = null;
        private float litersDrankInThisRound = 0.0f;
        private int currentRound = 1;
        private float totalLiters = 0.0f;
        private float beerSpawnDelay = 1.0f;
        private int currentHp = 0;

        private const string DRINK_STATE_ANIMATION_TRIGGER = "DrinkState";
        private const string DRINK_ANIMATION_BOOL = "Drink";

        protected BeerNBoozeSettings settings { get { return (BeerNBoozeSettings)_settings; } }
        protected BeerNBoozeUIManager uiManager { get { return (BeerNBoozeUIManager)_uiManager; } }

        private float litersRequired
        {
            get
            {
                int round = Mathf.Clamp(currentRound, 1, settings.roundLock);
                return settings.startingLitersPerRound + (round - 1) / barCounter.Length;
            }
        }

        private float drinkingSpeed
        {
            get
            {
                int round = Mathf.Clamp(currentRound, 1, settings.roundLock);
                return settings.startingDrinkingSpeed - settings.drinkingSpeedDelta * ((round - 1) / barCounter.Length);
            }
        }

        private void Awake() => Assert.IsNotNull(settings, "Missing minigame settings");

        public override void SetUpMinigame(BonusModifier bonus, bool tournament, int record)
        {
            base.SetUpMinigame(bonus, tournament, record);

            playerAnimator.SetTrigger(DRINK_STATE_ANIMATION_TRIGGER);
            currentHp = tournament ? 1 : settings.hp;
            uiManager.InitPlayerHealthBar(tournament);
            
            beerSpawnDelay = settings.startingSpawnDelay;
            SpawnBarCounters();
        }

        public override void StopMinigame()
        {
            StopAllCoroutines();

            foreach (BarCounter bc in barCounter)
                bc.Stop();
        }

        public virtual void SpawnBeers()
        {
            if (beerSpawnCoroutine != null)
                return;

            beerSpawnCoroutine = StartCoroutine(SpawnCoroutine());
        }

        /// <summary>
        /// spawn beer on a random bar counter every beerSpawnDelay seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnCoroutine()
        {
            int barCounterIndex;
            int beerSizeIndex;
            while (true)
            {
                yield return new WaitForSeconds(beerSpawnDelay);

                beerSizeIndex = Random.Range(0, settings.beerSize.Length);
                barCounterIndex = Random.Range(0, barCounter.Length);
                barCounter[barCounterIndex].SpawnBeer(settings.beerSize[beerSizeIndex], settings.beerDestructionHeight);

                yield return null;
            }
        }

        /// <summary>
        /// spawn and initialize the bar counters
        /// </summary>
        protected virtual void SpawnBarCounters()
        {
            barCounter = new BarCounter[settings.numberOfBarCounters];
            Vector3 currentBarCounterPosition = settings.firstSpawnPosition;
            for (int i = 0; i < barCounter.Length; i++)
            {
                BarCounter bc = Instantiate<BarCounter>(barCounterPrefab, currentBarCounterPosition, Quaternion.identity);
                bc.name = $"BarCounter [{i}]";
                bc.ID = i;
                bc.speed = settings.startingMovementSpeed;
                bc.OnBeerLitersUpdate += HandleLitersUpdate;
                bc.OnBeerWasted += GetDamage;
                bc.OnBeerEmpty += HandleBeerEmpty;
                bc.recatchBeer = settings.recatchBeer;

                barCounter[i] = bc;
                barCounterToBoost.Add(bc);
                currentBarCounterPosition += Vector3.right * (settings.barCounterGap + barCounterPrefab.transform.localScale.x);
            }
        }

        /// <summary>
        /// called when a beer is empty
        /// </summary>
        private void HandleBeerEmpty()
        {
            playerAnimator.SetBool(DRINK_ANIMATION_BOOL, false);
            uiManager.DisableSlider();
        }

        private void GetDamage()
        {
            // if dodge bonus must be applied do nothing
            if (currentBonus != null && 
                currentBonus.id == BonusID.BeersDodge && 
                Random.Range(0, 101) <= currentBonus.bonusEvent[0].chancePercentage
                )
            {
                bonusAnimator.SetTrigger(BeerNBoozeManager.BONUS_ANIMATION_TRIGGER);
                AudioPlayer.instance.PlaySFX(bonusSFX);
                return;
            }

            OnDamage?.Invoke();
            uiManager.UIDamage();   // update healthbar
            AudioPlayer.instance.PlaySFX(playerDamageSFX);

            if (--currentHp == 0)
            {
                ShowGameOver();
                uiManager.SetAllTexts(BeerNBoozeUIManager.COIN_REWARD_TEXT_TAG, $"+ {(int)(currentScore / settings.scoreToCoinConversionRate)}");
            }
        }

        /// <summary>
        /// called when drinking beer.
        /// Updates score and round
        /// </summary>
        /// <param name="currentBeerLiters">remaining liters in the beer</param>
        /// <param name="litersThisFrame">liters drank this frame</param>
        private void HandleLitersUpdate(float currentBeerLiters, float litersThisFrame)
        {
            litersDrankInThisRound += litersThisFrame;
            totalLiters += litersThisFrame;
            uiManager.SetBeerSlider(currentBeerLiters / settings.beerSize[settings.beerSize.Length - 1]);

            UpdateScore();

            // round is updated when litersRequired liters are reached in the current round
            if (Math.Round(litersDrankInThisRound,2) >= litersRequired)
                NextRound();
        }

        private void UpdateScore()
        {
            float previousScore = currentScore;
            currentScore = (int)((int)(totalLiters / settings.deltaLiters) * settings.deltaScore);
            if (previousScore != currentScore)
                uiManager.SetAllTexts(BeerNBoozeUIManager.SCORE_TEXT_TAG, $"Score: {currentScore}");
        }

        /// <summary>
        /// go to the next round and update difficulty
        /// </summary>
        private void NextRound()
        {
            litersDrankInThisRound -= litersRequired;
            currentRound++;

            UpdateSpawnDelay();
            BoostCounterSpeed();
        }

        /// <summary>
        /// increase the speed of a random counter.
        /// when a counter speed is increased, it is removed from the barCounterToBoost list until every other counter speed has increased
        /// </summary>
        private void BoostCounterSpeed()
        {
            if (currentRound >= settings.roundLock || barCounterToBoost.Count == 0)
                return;

            int randomIndex = Random.Range(0, barCounterToBoost.Count);
            barCounterToBoost[randomIndex].speed += settings.movementSpeedDelta;
            barCounterToBoost.RemoveAt(randomIndex);
        }

        private void UpdateSpawnDelay()
        {
            if (currentRound > settings.roundLock)
                return;

            if (barCounterToBoost.Count == 0)
            {
                beerSpawnDelay -= settings.spawnDelta;
                barCounterToBoost = barCounter.ToList<BarCounter>();
            }
        }

        public void HandleHoldInput(bool isHolding, GameObject go)
        {
            if (Time.timeScale == 0)
                return;

            int barCounterID = go.GetComponent<TapArea>().ID;

            // drink if holding and a beer is in player range
            if (isHolding && barCounter[barCounterID].beerAvailable)
            {
                DrinkBeer(barCounterID);
                playerAnimator.transform.position = barCounter[barCounterID].playerPosition;
                playerAnimator.SetBool(DRINK_ANIMATION_BOOL, true);
                return;
            }

            // stop drinking if not holding anymore
            if (!isHolding)
            {
                barCounter[barCounterID].StopDrinking();
                playerAnimator.SetBool(DRINK_ANIMATION_BOOL, false);
            }
        }

        protected virtual void DrinkBeer(int barCounterID)
        {
            // check if cheers bonus must be applied
            if (currentBonus != null &&
                currentBonus.id == BonusID.Cheers &&
                Random.Range(0, 101) <= currentBonus.bonusEvent[0].chancePercentage
                )
            {
                barCounter[barCounterID].DrinkBeer(drinkingSpeed, settings.deltaLiters, true);
                bonusAnimator.SetTrigger(BeerNBoozeManager.BONUS_ANIMATION_TRIGGER);
                AudioPlayer.instance.PlaySFX(bonusSFX);
            }
            else
                barCounter[barCounterID].DrinkBeer(drinkingSpeed, settings.deltaLiters, false);
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
    }
}
