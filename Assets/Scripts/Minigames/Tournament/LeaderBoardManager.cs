using GameSave;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tournament
{
    public class LeaderBoardManager : MonoBehaviour
    {
        [SerializeField] private TournamentPlayer tournamentPlayerPrefab = null;
        [SerializeField] private TournamentSettings settings = null;
        [SerializeField] private Transform viewportContent = null;

        /// <summary>
        /// spawn leaderboard entries under the leaderboard canvas.
        /// The entries are stored in a scriptable object.
        /// This player's entry is added in the correct order based on the tournament score in the game saves.
        /// </summary>
        /// <param name="gameData">the game save data</param>
        public void InitLeaderboard(GameData gameData)
        {
            // create the list of the tournament entries and order it by the score
            List<PlayerInfo> tournamentParticipants = settings.opponents.ToList<PlayerInfo>();
            PlayerInfo player = settings.player;
            player.score = gameData.playerTournamentRecord;
            tournamentParticipants.Add(player);
            tournamentParticipants.Sort((x, y) => y.score.CompareTo(x.score));

            // spawn the entries and sets the corresponding ui elements
            for (int i = 0; i < tournamentParticipants.Count; i++)
            {
                TournamentPlayer opponent = Instantiate<TournamentPlayer>(tournamentPlayerPrefab, viewportContent);
                opponent.image.sprite = tournamentParticipants[i].profilePicture;
                opponent.positionTextArea.text = $"{i+1}.";
                opponent.nameTextArea.text = tournamentParticipants[i].name;
                opponent.scoreTextArea.text = tournamentParticipants[i].score.ToString();

                if (tournamentParticipants[i].linkedinURL == "")
                    opponent.URLButton.gameObject.SetActive(false);
                else
                    opponent.URLButton.url = tournamentParticipants[i].linkedinURL;
            }
        }
    }
}
