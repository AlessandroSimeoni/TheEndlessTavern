using Audio;
using EndlessTavernUI;
using System;
using System.Collections;
using UnityEngine;

namespace Minigame
{
    public enum MinigameSceneIndex
    {
        BeerNBooze = 1,
        TillLastTooth = 2,
        FullUpFeast = 3, 
        BeerNBoozeTutorial = 6,
        TillLastToothTutorial = 7,
        FullUpFeastTutorial = 8
    }

    public abstract class MinigameManager : MonoBehaviour
    {
        public MinigameSceneIndex minigameIndex = MinigameSceneIndex.BeerNBooze;
        [Header("Settings")]
        [SerializeField] protected ScriptableObject _settings = null;
        [Header("UI")]
        [SerializeField] protected MinigameUIManager _uiManager = null;
        [SerializeField] protected Animator bonusAnimator = null;
        [Header("Audio")]
        [SerializeField] private AudioClip minigameBGM = null;
        [SerializeField] private AudioClip countdownStartMinigameSFX = null;
        [SerializeField] private AudioClip gameOverSFX = null;
        [SerializeField] private AudioClip newRecordSFX = null;
        [SerializeField] protected AudioClip playerDamageSFX = null;
        [SerializeField] protected AudioClip bonusSFX = null;

        [Serializable] public sealed class CoinRewardEvent : UnityEngine.Events.UnityEvent<int> { }
        public CoinRewardEvent OnCoinReward = new CoinRewardEvent();
        [Serializable] public sealed class RecordEvent : UnityEngine.Events.UnityEvent<int> { }
        public RecordEvent OnNewRecord = new RecordEvent();

        protected int currentScore = 0;

        public delegate void GameOverEvent(int score);
        public GameOverEvent OnGameOver = null;

        protected BonusModifier currentBonus = null;
        protected bool isTournament = false;
        protected int previousRecord = 0;

        protected const string BONUS_ANIMATION_TRIGGER = "Bonus";

        private bool bgmStarted = false;

        public abstract void InvokeCoinReward();

        /// <summary>
        /// initialize minigame
        /// </summary>
        /// <param name="bonus">the bonus if present, null otherwise</param>
        /// <param name="tournament">tournament?</param>
        /// <param name="record">the previous score record</param>
        public virtual void SetUpMinigame(BonusModifier bonus, bool tournament, int record)
        {
            if (bonus != null)
            {
                currentBonus = bonus;
                _uiManager.SetModifierSprite(bonus.bonusSprite);
                bonusAnimator.gameObject.SetActive(true);
            }

            isTournament = tournament;
            previousRecord = record;
            _uiManager.SetAllTexts(MinigameUIManager.RECORD_TEXT_TAG, $"Record: {record}");
            _uiManager.SetAllTexts(MinigameUIManager.SCORE_TEXT_TAG, $"Score: {currentScore}");
        }

        public virtual void ShowPause()
        {
            UpdateUIRecord();

            if (isTournament)
                _uiManager.ShowPopUpCanvas(MinigameUIManager.TOURNAMENT_PAUSE_TAG);
            else
                _uiManager.ShowPopUpCanvas(MinigameUIManager.PAUSE_TAG);
        }

        private void UpdateUIRecord() => _uiManager.SetAllTexts(MinigameUIManager.RECORD_TEXT_TAG, $"Record: {(currentScore > previousRecord ? currentScore : previousRecord)}");

        public virtual void ShowGameOver()
        {
            OnGameOver?.Invoke(currentScore);

            AudioPlayer.instance.PlaySFX(gameOverSFX);

            UpdateUIRecord();

            if (currentScore > previousRecord)
            {
                OnNewRecord?.Invoke(currentScore);
                StartCoroutine(NewRecordSFXCoroutine());
                previousRecord = currentScore;
            }

            if (isTournament)
                _uiManager.ShowCanvas(MinigameUIManager.TOURNAMENT_GAMEOVER_TAG);
            else
                _uiManager.ShowCanvas(MinigameUIManager.GAMEOVER_TAG);
        }

        private IEnumerator NewRecordSFXCoroutine()
        {
            yield return new WaitForSecondsRealtime(gameOverSFX.length);
            AudioPlayer.instance.PlaySFX(newRecordSFX);
        }

        public virtual void StartMinigame()
        {
            _uiManager.ShowPopUpCanvas(MinigameUIManager.COUNTDOWN_TAG);
            AudioPlayer.instance.PlaySFX(countdownStartMinigameSFX);
        }
        public abstract void StopMinigame();

        public void StartMinigameBGM()
        {
            if (bgmStarted)
                return;

            AudioPlayer.instance.PlayBGM(minigameBGM);
            bgmStarted = true;
        }

        public abstract void HealPlayer();
    }
}
