using Minigame;
using UnityEngine;

namespace Tournament
{
    [System.Serializable]
    public struct PlayerInfo
    {
        public string name;
        public Sprite profilePicture;
        public int score;
        public string linkedinURL;
    }

    [CreateAssetMenu(fileName = "TournamentSettings", menuName = "ScriptableObjects/Minigames/Tournament/TournamentSettings")]
    public class TournamentSettings : ScriptableObject
    {
        [Header("Flow")]
        public MinigameSceneIndex[] tournamentFlow = new MinigameSceneIndex[3];

        [Header("Player")]
        public PlayerInfo player;

        [Header("Opponents")]
        public PlayerInfo[] opponents = new PlayerInfo[0];
    }
}
