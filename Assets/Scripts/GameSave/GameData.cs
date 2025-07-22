using System;
using System.IO;
using UnityEngine;

namespace GameSave
{
    public enum FTUEStep
    {
        None,   // starting default value
        Step1,  // drink game step
        Step2,  // fight game step
        Step3,  // eat game step
        Step4,  // welcome reward step
        Step5,  // shop step
        Step6,  // customization step
        Completed
    }

    public enum ArmorID
    {
        Armor0,
        Armor1,
        Armor2
    }

    [Serializable]
    public struct ArmorPieces
    {
        public ArmorID ID;
        public int quantityOwned;
    }

    [Serializable]
    public class GameData
    {
        private const string SAVE_PATH = "/EndlessTavern.save";

        public int playerCoins = 0;
        public int playerTickets = 0;
        public int playerGems = 0;
        public int playerTournamentRecord = 0;
        public int beerNBoozeRecord = 0;
        public int tillLastToothRecord = 0;
        public int fullUpFeastRecord = 0;
        public bool beerNBoozeTutorialCompleted = false;
        public bool tillLastToothTutorialCompleted = false;
        public bool fullUpFeastTutorialCompleted = false;
        public ArmorPieces[] playerArmorPieces = InitPlayerArmorPiecesData();
        public FTUEStep ftueCompletedStep = FTUEStep.None;
        public bool masterAudioEnabled = true;
        public bool bgmAudioEnabled = true;
        public bool sfxAudioEnabled = true;

        public void Save()
        {
            string jsonString = JsonUtility.ToJson(this);
            File.WriteAllText(Application.persistentDataPath + SAVE_PATH, jsonString);
        }

        public void Load()
        {
            if (!File.Exists(Application.persistentDataPath + SAVE_PATH))
                return;

            JsonUtility.FromJsonOverwrite(File.ReadAllText(Application.persistentDataPath + SAVE_PATH), this);
        }

        private static ArmorPieces[] InitPlayerArmorPiecesData()
        {
            ArmorPieces[] output = new ArmorPieces[Enum.GetValues(typeof(ArmorID)).Length];
            for(int i = 0; i < output.Length; i++)
            {
                output[i].ID = (ArmorID)i;
                output[i].quantityOwned = 0;
            }

            return output;
        }
    }
}
