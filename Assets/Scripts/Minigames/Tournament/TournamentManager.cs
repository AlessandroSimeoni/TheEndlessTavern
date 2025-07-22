using Audio;
using EndlessTavernUI;
using UnityEngine;

namespace Tournament
{
    public class TournamentManager : MonoBehaviour
    {
        public TournamentSettings tournamentSettings = null;
        [SerializeField] private TournamentGameOver tournamentGameOver = null;
        [SerializeField] private UIManager uiManager = null;
        [SerializeField] private AudioClip tournamentButtonSFX = null;

        private const string FINAL_SCORE_TAG = "FinalTournamentScoreText";

        public void SetUpEndingScreen(float finalScore)
        {
            if (tournamentGameOver == null || uiManager == null)
                return;

            uiManager.SetText(FINAL_SCORE_TAG, $"SCORE: {finalScore}");
            tournamentGameOver.ToggleElements();
        }

        public void TournamentButtonSFX() => AudioPlayer.instance.PlaySFX(tournamentButtonSFX);
    }
}
