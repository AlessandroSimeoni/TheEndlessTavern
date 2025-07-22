using Audio;
using GameSave;
using Minigame;
using SceneLoad;
using System.IO;
using Tips;
using Tournament;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EndlessTavern
{
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private MyAudioSettings audioSettings = null;
        [SerializeField] private AudioClip mainMenuBGM = null;
        [Header("Minigames")]
        [SerializeField] private MinigameManager minigame = null;
        [SerializeField] private TournamentManager tournamentManager = null;
        [System.Serializable] public sealed class LoadGameDataEvent : UnityEngine.Events.UnityEvent<GameData> { }
        public LoadGameDataEvent OnGameDataLoaded = new LoadGameDataEvent();

        private GameData gameData = new GameData();

        private static BonusModifier minigameBonus = null;  // used to tell the minigame manager which bonus must be applied
        private static string minigameRequested = null;     // used to handle the tips minigame
        private static int tournamentNextSceneIndex = -1;   // if negative we are not in the tournament
        private static int tournamentCurrentScore = 0;      // the current score in the tournament

        private const string TIPS_SCENE_NAME = "Minigame - Tips";

        private void Awake()
        {
            gameData.Load();
            OnGameDataLoaded?.Invoke(gameData);

            if (gameData.ftueCompletedStep != FTUEStep.Completed)
                PlayerPrefs.DeleteAll();

            SceneLoader.instance.OnTargetSceneReady += HandleSceneReady;
            SceneLoader.instance.OnLoadingCompleted += HandleLoadingCompleted;

            AudioPlayer.instance.settings = audioSettings;
        }

        private void Start()
        {
            AudioPlayer.instance.ToggleMaster(gameData.masterAudioEnabled, false);
            AudioPlayer.instance.ToggleBGM(gameData.bgmAudioEnabled, false);
            AudioPlayer.instance.ToggleSFX(gameData.sfxAudioEnabled);
            if (minigame == null)
            {
                Camera.main.GetComponent<AudioListener>().enabled = true;
                AudioPlayer.instance.PlayBGM(mainMenuBGM);
            }

#if UNITY_EDITOR
            if (!SceneLoader.instance.loading && minigame != null)
            {
                Camera.main.GetComponent<AudioListener>().enabled = true;
                SceneLoader.instance.OnTargetSceneReady -= HandleSceneReady;
                SceneLoader.instance.OnLoadingCompleted -= HandleLoadingCompleted;
                minigame.OnGameOver += HandleMinigameGameOver;
                minigame.SetUpMinigame(minigameBonus, tournamentNextSceneIndex >= 0, GetMinigameRecord());
                minigameBonus = null;
                minigame.StartMinigame();
            }
#endif
        }

        private int GetMinigameRecord()
        {
            if (minigame != null)
            {
                switch (minigame.minigameIndex)
                {
                    case MinigameSceneIndex.BeerNBooze:
                        return gameData.beerNBoozeRecord;
                    case MinigameSceneIndex.TillLastTooth:
                        return gameData.tillLastToothRecord;
                    case MinigameSceneIndex.FullUpFeast:
                        return gameData.fullUpFeastRecord;
                }
            }

            return -1;
        }
        
        public void SetNewRecord(int score)
        {
            switch (minigame.minigameIndex)
            {
                case MinigameSceneIndex.BeerNBooze:
                    gameData.beerNBoozeRecord = score;
                    break;
                case MinigameSceneIndex.TillLastTooth:
                    gameData.tillLastToothRecord = score;
                    break;
                case MinigameSceneIndex.FullUpFeast:
                    gameData.fullUpFeastRecord = score;
                    break;
            }

            gameData.Save();
        }

        private void HandleSceneReady()
        {
            if (minigame == null)
                return;

            if (minigame is TipsManager && minigameRequested != null)
            {
                (minigame as TipsManager).minigameRequestedBuildIndex = SceneUtility.GetBuildIndexByScenePath(@"Assets/Scenes/" + minigameRequested + ".unity");
                (minigame as TipsManager).OnEndGame += HandleTipsMinigameEnding;
            }

            minigame.OnGameOver += HandleMinigameGameOver;
            minigame.SetUpMinigame(minigameBonus, tournamentNextSceneIndex >= 0, GetMinigameRecord());
            minigameBonus = null;
        }

        /// <summary>
        /// Handle game over screen
        /// </summary>
        private void HandleMinigameGameOver(int score)
        {
            FreezeGame();
            HandleTournamentGameOver(score);
        }

        /// <summary>
        /// handle game over in the tournament, if it's the game over of the last tournament minigame then toggle elements
        /// </summary>
        /// <param name="score"></param>
        private void HandleTournamentGameOver(int score)
        {
            if (tournamentNextSceneIndex >= 0)
                tournamentCurrentScore += score;

            if (tournamentNextSceneIndex == tournamentManager.tournamentSettings.tournamentFlow.Length - 1)
            {
                tournamentManager.SetUpEndingScreen(tournamentCurrentScore);
                if (tournamentCurrentScore > gameData.playerTournamentRecord)
                {
                    gameData.playerTournamentRecord = tournamentCurrentScore;
                    gameData.Save();
                }
            }
        }


        /// <summary>
        /// handle tutorial completion of a minigame and loads the real minigame scene
        /// </summary>
        /// <param name="minigame">the minigame build index</param>
        public void HandleTutorialCompleted(MinigameSceneIndex minigame)
        {
            switch (minigame)
            {
                case MinigameSceneIndex.BeerNBooze:
                    gameData.beerNBoozeTutorialCompleted = true;
                    break;
                case MinigameSceneIndex.TillLastTooth:
                    gameData.tillLastToothTutorialCompleted = true;
                    break;
                case MinigameSceneIndex.FullUpFeast:
                    gameData.fullUpFeastTutorialCompleted = true;
                    break;
            }

            gameData.Save();
            StopBGM();
            LoadSceneFromBuildIndex(minigame);
        }

        private void HandleTipsMinigameEnding(BonusModifier bonus)
        {
            StopBGM();
            minigameBonus = bonus;
            LoadScene(minigameRequested);
        }

        private void HandleLoadingCompleted()
        {
            if (minigame == null)
                return;

            minigame.StartMinigame();
        }

        public void LoadScene(string sceneName)
        {
            UnfreezeGame();
            ResetTournamentState();
            SceneLoader.instance.LoadScene(sceneName);
        }


        /// <summary>
        /// Loads a minigame.
        /// If the tutorial of the minigame requested has been completed then load the minigame, otherwise load the tutorial
        /// </summary>
        /// <param name="minigameIndex">the minigame scene build index</param>
        public void RequestMinigameLoading(int minigameIndex)
        {
            bool loadMinigame = true;
            MinigameSceneIndex minigameTutorial = MinigameSceneIndex.BeerNBoozeTutorial;

            switch ((MinigameSceneIndex)minigameIndex)
            {
                case MinigameSceneIndex.BeerNBooze:
                    loadMinigame = gameData.beerNBoozeTutorialCompleted;
                    minigameTutorial = MinigameSceneIndex.BeerNBoozeTutorial;
                    break;
                case MinigameSceneIndex.TillLastTooth:
                    loadMinigame = gameData.tillLastToothTutorialCompleted;
                    minigameTutorial = MinigameSceneIndex.TillLastToothTutorial;
                    break;
                case MinigameSceneIndex.FullUpFeast:
                    loadMinigame = gameData.fullUpFeastTutorialCompleted;
                    minigameTutorial = MinigameSceneIndex.FullUpFeastTutorial;
                    break;
            }

            LoadSceneFromBuildIndex(loadMinigame ? (MinigameSceneIndex)minigameIndex : minigameTutorial);
        }

        private static void ResetTournamentState()
        {
            tournamentNextSceneIndex = -1;
            tournamentCurrentScore = 0;
        }

        /// <summary>
        /// load the next minigame in the tournament flow 
        /// </summary>
        public void LoadTournamentMinigame()
        {
            tournamentNextSceneIndex++;
            LoadSceneFromBuildIndex(tournamentManager.tournamentSettings.tournamentFlow[tournamentNextSceneIndex]);
        }

        private void LoadSceneFromBuildIndex(MinigameSceneIndex buildIndex)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex((int)buildIndex);
            SceneLoader.instance.LoadScene(Path.GetFileNameWithoutExtension(scenePath));
        }

        /// <summary>
        /// Load the tips minigame scene saving the minigame to load at the end of it
        /// </summary>
        /// <param name="minigameSceneName">the minigame requested</param>
        public void LoadTipsScene(string minigameSceneName)
        {
            minigameRequested = minigameSceneName;
            LoadScene(TIPS_SCENE_NAME);
        }

        public void FreezeGame() => Time.timeScale = 0.0f;
        public void UnfreezeGame() => Time.timeScale = 1.0f;

        public void ToggleMasterAudio(bool value)
        {
            gameData.masterAudioEnabled = value;
            gameData.Save();
            AudioPlayer.instance.ToggleMaster(gameData.masterAudioEnabled);
        }
        
        public void ToggleBGMAudio(bool value)
        {
            gameData.bgmAudioEnabled = value;
            gameData.Save();
            AudioPlayer.instance.ToggleBGM(gameData.bgmAudioEnabled);
        }

        public void ToggleSFXAudio(bool value)
        {
            gameData.sfxAudioEnabled = value;
            gameData.Save();
            AudioPlayer.instance.ToggleSFX(gameData.sfxAudioEnabled);
        }

        public void StopBGM() => AudioPlayer.instance.StopBGM();
    }
}
