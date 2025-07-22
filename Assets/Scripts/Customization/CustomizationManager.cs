using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using GameSave;
using System;
using Audio;

namespace Customization
{
    public class CustomizationManager : MonoBehaviour
    {
        [SerializeField] private CustomizationUpgradeSettings upgradeSettings = null;
        [SerializeField] private PlayerCustomization player = null;
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI materialText = null;
        [SerializeField] private string[] materialName = new string[4];
        [SerializeField] private CustomizationButton[] materialButton = new CustomizationButton[4];
        [SerializeField] private ArmorButton[] armorButton = new ArmorButton[4];
        [Header("Audio")]
        [SerializeField] private AudioClip lockedButtonSFX = null;
        [SerializeField] private AudioClip equipArmorSFX = null;
        [SerializeField] private AudioClip materialChangeSFX = null;

        public const string PLAYER_ARMOR_PREF = "PlayerArmorSet";
        public const string PLAYER_MATERIAL_PREF = "PlayerMaterial";

        private GameData gameData = null;

        private void OnEnable()
        {
            HandleCustomizationButtons();
            SetArmorButtonSliders();
        }

        /// <summary>
        /// update sliders under the armor buttons.
        /// Sliders tells how many armor pieces the player has and how much are needed to unlock new material
        /// </summary>
        private void SetArmorButtonSliders()
        {
            foreach(ArmorButton button in armorButton)
            {
                if (button.isLocked)
                    button.SetSlider(0.0f, "LOCKED");
                else
                {
                    int armorOwned = GetArmorOwned(button.armorID);
                    int materialUpgradeValue = GetMaterialToUnlockQuantity(button.armorID, armorOwned);

                    if (materialUpgradeValue == -1)
                        button.SetSlider(1.0f, "COMPLETE");
                    else
                        button.SetSlider(armorOwned/(float)materialUpgradeValue, $"{armorOwned}/{materialUpgradeValue}");
                }
            }
        }

        /// <summary>
        /// get the armor pieces quantity required to unlock the next material for the specified armor ID
        /// </summary>
        /// <param name="armorID">the armor ID</param>
        /// <param name="armorOwned">the current armor pieces owned</param>
        /// <returns>the number of armor pieces required</returns>
        private int GetMaterialToUnlockQuantity(ArmorID armorID, int armorOwned)
        {
            MaterialUpgradeInfo[] materialUpgradeInfo = GetArmorUpgradeInfo(armorID).materialUpgradeInfo;
            for (int i = 0; i < materialUpgradeInfo.Length; i++)
                if (armorOwned < materialUpgradeInfo[i].armorPiecesQuantity)
                    return GetUpgradeMaterialQuantity(armorID, materialUpgradeInfo[i].material);

            return -1;
        }

        private void ToggleArmorButtons()
        {
            for (int i = 0; i < armorButton.Length; i++)
                armorButton[i].Toggle(GetArmorOwned(armorButton[i].armorID) >= GetUpgradeMaterialQuantity(armorButton[i].armorID, ArmorMaterial.Bronze));
        }


        public void ToggleMaterialButtons(int armorID) => ToggleMaterialButtons((ArmorID)armorID);

        private void ToggleMaterialButtons(ArmorID armorID)
        {
            for (int i = 0; i < materialButton.Length; i++)
                materialButton[i].Toggle(GetArmorOwned(armorID) >= GetUpgradeMaterialQuantity(armorID, (ArmorMaterial)i));
        }

        private int GetUpgradeMaterialQuantity(ArmorID armorID, ArmorMaterial armorMat)
        {
            return GetArmorUpgradeInfo(armorID).materialUpgradeInfo[(int)armorMat].armorPiecesQuantity;
        }

        /// <summary>
        /// get the upgrade info for the specified armor ID
        /// </summary>
        /// <param name="armorID">the armor ID</param>
        /// <returns></returns>
        private ArmorUpgradeInfo GetArmorUpgradeInfo(ArmorID armorID)
        {
            int index = Array.FindIndex(upgradeSettings.armorUpgradeInfo, x => x.ID == armorID);
            return upgradeSettings.armorUpgradeInfo[index];
        }

        private int GetArmorOwned(ArmorID armorID)
        {
            int index = Array.FindIndex(gameData.playerArmorPieces, x => x.ID == armorID);
            return gameData.playerArmorPieces[index].quantityOwned;
        }

        /// <summary>
        /// Enable/Disable the correct buttons when entering the customization screen
        /// </summary>
        private void HandleCustomizationButtons()
        {
            int armorIndex = PlayerPrefs.GetInt(PLAYER_ARMOR_PREF, 0);
            TurnOffSelectedButton(armorButton, armorIndex);

            int materialIndex = PlayerPrefs.GetInt(PLAYER_MATERIAL_PREF, 0);
            ToggleMaterialButtons((ArmorID)armorIndex);
            TurnOffSelectedButton(materialButton, materialIndex);

            if (materialName.Length > 0)
                materialText.text = materialName[materialIndex];
        }



        /// <summary>
        /// Set the interactable flag of the button at the index position of the buttonArray to false;
        /// set the flag to true for the others buttons
        /// </summary>
        /// <param name="buttonArray">the button array</param>
        /// <param name="index">the button position in the array</param>
        private void TurnOffSelectedButton(CustomizationButton[] buttonArray, int index)
        {
            for (int i = 0; i < buttonArray.Length; i++)
            {
                if (buttonArray[i].isLocked)
                    continue;

                Button button = buttonArray[i].GetComponent<Button>();
                button.interactable = (i != index);
            }
        }

        public void SetPlayerMesh(int index)
        {
            PlayerPrefs.SetInt(PLAYER_ARMOR_PREF, index);
            player.SetMesh(index);
            SetPlayerMaterial(0);
            TurnOffSelectedButton(armorButton, index);
            TurnOffSelectedButton(materialButton, 0);
        }

        public void SetPlayerMaterial(int index)
        {
            PlayerPrefs.SetInt(PLAYER_MATERIAL_PREF, index);
            player.SetMaterial(index);
            TurnOffSelectedButton(materialButton, index);
            materialText.text = materialName[index];
        }

        public void GetGameData(GameData gameData)
        {
            this.gameData = gameData;
            ToggleArmorButtons();
        }

        /// <summary>
        /// add armor pieces to game data and saves
        /// </summary>
        /// <param name="armorToAdd">the armor quantity to add for the associated armor ID</param>
        public void AddArmorPieces(Dictionary<ArmorID, int> armorToAdd)
        {
            foreach(KeyValuePair<ArmorID, int> kvp in armorToAdd)
            {
                int index = Array.FindIndex(gameData.playerArmorPieces, x => x.ID == kvp.Key);
                gameData.playerArmorPieces[index].quantityOwned += kvp.Value;
            }

            gameData.Save();
            ToggleArmorButtons();
        }

        public void PlayLockedSFX() => AudioPlayer.instance.PlaySFX(lockedButtonSFX);
        public void PlayEquipArmorSFX() => AudioPlayer.instance.PlaySFX(equipArmorSFX);
        public void PlayMaterialChangeSFX() => AudioPlayer.instance.PlaySFX(materialChangeSFX);
    }
}
