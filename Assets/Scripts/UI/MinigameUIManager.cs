using UnityEngine;
using UnityEngine.UI;

namespace EndlessTavernUI
{
    public class MinigameUIManager : UIManager
    {
        [Header("Health")]
        [SerializeField] private HealthBar playerHealthBarPrefab = null;
        [SerializeField] private HealthBar tournamentPlayerHealthBarPrefab = null;
        [SerializeField] protected Transform healthBarParent = null;
        [Header("Modifier Image")]
        [SerializeField] protected Image modifierImage = null;

        private HealthBar playerHealtBarInstance = null;

        public const string COUNTDOWN_TAG = "CountdownCanvas";
        public const string PAUSE_TAG = "PauseCanvas";
        public const string TOURNAMENT_PAUSE_TAG = "TournamentPauseCanvas";
        public const string GAMEOVER_TAG = "GameOverCanvas";
        public const string TOURNAMENT_GAMEOVER_TAG = "TournamentGameOverCanvas";
        public const string ROUND_TEXT_TAG = "RoundText";
        public const string PLAYER_HP_TEXT_TAG = "PlayerHPText";
        public const string SCORE_TEXT_TAG = "ScoreText";
        public const string COIN_REWARD_TEXT_TAG = "CoinRewardText";
        public const string RECORD_TEXT_TAG = "RecordText";

        public void InitPlayerHealthBar(bool tournament)
        {
            if (tournament)
                playerHealtBarInstance = Instantiate<HealthBar>(tournamentPlayerHealthBarPrefab, healthBarParent);
            else
                playerHealtBarInstance = Instantiate<HealthBar>(playerHealthBarPrefab, healthBarParent);
        }

        public void UIDamage()
        {
            playerHealtBarInstance.HealthBarDamage();
        }

        public void UIHeal(int quantity) => playerHealtBarInstance.RefillHearts(quantity);

        public void SetModifierSprite(Sprite modSprite) => modifierImage.sprite = modSprite;
    }
}
