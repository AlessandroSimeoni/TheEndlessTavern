using Audio;
using Minigame;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace TillLastTooth
{
    [DisallowMultipleComponent]
    public class TillLastToothManager : MinigameManager
    {
        [Header("Spawner")]
        [SerializeField] private TillLastToothSpawner spawner = null;
        [Header("Enemy")]
        [SerializeField] private Vector3 enemyStartingPosition = new Vector3(-5.0f, 1.25f, 2.0f);
        [Header("Audio")]
        [SerializeField] private AudioClip playerPunchMissSFX = null;

        [Serializable] public sealed class DamageEvent : UnityEngine.Events.UnityEvent { }
        public DamageEvent OnPlayerDamage = new DamageEvent();

        protected Player playerInstance = null;
        protected Enemy enemyInstance = null;
        private bool gameStarted = false;
        private bool playerTookDamage = false;
        private int currentRound = 1;
        private int enemiesDefeatedInRound = 0;

        private TillLastToothSettings settings { get { return (TillLastToothSettings)_settings; } }
        protected TillLastToothUIManager uiManager { get { return (TillLastToothUIManager)_uiManager; } }

        private void Awake()
        {
            Assert.IsNotNull(uiManager, "Missing minigame ui manager");
            Assert.IsNotNull(spawner, "Missing spawner");
        }

        public override void SetUpMinigame(BonusModifier bonus, bool tournament, int record)
        {
            base.SetUpMinigame(bonus, tournament, record);

            playerInstance = spawner.SpawnPlayer(Vector3.zero, Quaternion.identity);
            InitParticipant(playerInstance);
            uiManager.InitPlayerHealthBar(tournament);

            enemyInstance = spawner.SpawnEnemy(enemyStartingPosition);
            InitParticipant(enemyInstance);

            uiManager.SetAllTexts(TillLastToothUIManager.SCORE_TEXT_TAG, $"Score: {currentScore}");
            uiManager.OnPopUpDone += enemyInstance.Attack;
            uiManager.CooldownReady();
        }

        public override void StopMinigame()
        {
            enemyInstance.Stop();
            playerInstance.Stop();
            uiManager.HideAllPopUp(0.0f);
        }

        public override void ShowGameOver()
        {
            base.ShowGameOver();
            uiManager.GameOver();
        }

        public virtual void FirstEnemyEntrance()
        {
            if (gameStarted)
                return;

            gameStarted = true;
            enemyInstance.Entrance();
        }

        private void InitParticipant(Participant participant)
        {
            participant.leftRightOffset = settings.leftRightOffset;
            participant.roundLock = settings.roundLock;
            participant.currentRound = currentRound;

            switch (participant)
            {
                case Enemy e:
                    e.OnHealthInit += uiManager.InitEnemyHealthBar;
                    e.OnHealthDamage += uiManager.DamageEnemyHealthBar;
                    e.OnAttackCharging += HandleEnemyCharging;
                    e.OnImpact += HandleEnemyImpact;
                    e.OnKO += HandleEnemyKO;
                    break;
                case Player p:
                    p.SetPlayerHealth(isTournament);
                    p.OnKO += HandlePlayerKO;
                    p.OnImpact += HandlePlayerImpact;
                    p.OnDodge += uiManager.EmptyDodgeSlider;
                    p.OnDodgeDone += uiManager.LoadCooldown;
                    p.InitFightingState();
                    break;
            }
        }

        private void HandlePlayerImpact()
        {
            if (playerTookDamage)
                return;

            if (!enemyInstance.vulnerable)
            {
                uiManager.ShowMissPopUp();
                AudioPlayer.instance.PlaySFX(playerPunchMissSFX);
                return;
            }
            
            // check if bonus must be applied
            if (enemyInstance.currentHealth > 1 &&      // activate bonus only if the enemy has more than 1 hp
                currentBonus != null && 
                currentBonus.id != BonusID.None && 
                Random.Range(0, 101) <= currentBonus.bonusEvent[0].chancePercentage
                )
            {
                switch (currentBonus.id)
                {
                    case BonusID.DoubleDamage:
                        currentScore += settings.scorePerPunch * 2;
                        enemyInstance.GetDamage(2);
                        break;
                    case BonusID.OnePunchKnight:    // one shot the enemy
                        currentScore += settings.scorePerPunch * enemyInstance.currentHealth;
                        enemyInstance.GetDamage(enemyInstance.currentHealth);
                        break;
                }
                bonusAnimator.SetTrigger(TillLastToothManager.BONUS_ANIMATION_TRIGGER);
                AudioPlayer.instance.PlaySFX(bonusSFX);
            }
            else
            {
                enemyInstance.GetDamage();
                currentScore += settings.scorePerPunch;
            }

            uiManager.SetAllTexts(TillLastToothUIManager.SCORE_TEXT_TAG, $"Score: {currentScore}");
        }

        private void HandlePlayerKO()
        {
            uiManager.OnPopUpDone -= enemyInstance.Attack;
            ShowGameOver();
            uiManager.SetAllTexts(TillLastToothUIManager.COIN_REWARD_TEXT_TAG, $"+ {(int)(currentScore / settings.scoreToCoinConversionRate)}");
        }

        private void HandleEnemyCharging(Vector3 position, float time)
        {
            playerTookDamage = false;
            uiManager.ActivatePopUp(position, time);
        }
        private void HandleEnemyKO()
        {
            IncrementRound();
            enemyInstance.InitParameters();
            enemyInstance.transform.position = enemyStartingPosition;
            enemyInstance.Entrance();
        }

        private void IncrementRound()
        {
            if (++enemiesDefeatedInRound == settings.enemiesPerRound)
            {
                currentRound++;
                enemiesDefeatedInRound = 0;

                playerInstance.currentRound = currentRound;
                enemyInstance.currentRound = currentRound;
            }
        }

        private void HandleEnemyImpact(Vector3 impactDirection, float time)
        {
            // player gets damage in the center and in the direction of the hit
            if (Vector3.Dot(impactDirection, playerInstance.transform.position) >= -settings.damageDetectionOffset)
            {
                playerInstance.GetDamage();
                uiManager.UIDamage();
                playerTookDamage = true;
                OnPlayerDamage?.Invoke();
                AudioPlayer.instance.PlaySFX(playerDamageSFX);
            }

            uiManager.HideAllPopUp(time);
        }

        public virtual void MovePlayer(Vector2 direction)
        {
            if (playerInstance == null || Time.timeScale == 0)
                return;

            playerInstance.Dodge(direction);
        }

        public override void InvokeCoinReward()
        {
            if (playerInstance.currentHealth > 0)
                return;

            int coinReward = (int)(currentScore / settings.scoreToCoinConversionRate);
            OnCoinReward?.Invoke(coinReward);
        }

        public override void HealPlayer()
        {
            playerInstance.Heal(1);
            uiManager.UIHeal(1);
        }

        public void ContinueFromGameOver() => uiManager.OnPopUpDone += enemyInstance.Attack;
    }
}
