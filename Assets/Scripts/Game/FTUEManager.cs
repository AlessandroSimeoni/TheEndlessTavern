using GameSave;
using System;
using UnityEngine;

namespace EndlessTavern
{
    public class FTUEManager : MonoBehaviour
    {
        [Serializable] public sealed class WelcomeRewardStepEvent : UnityEngine.Events.UnityEvent { }
        public WelcomeRewardStepEvent OnWelcomeRewardStep = new WelcomeRewardStepEvent();
        [Serializable] public sealed class FTUECompletedEvent : UnityEngine.Events.UnityEvent { }
        public FTUECompletedEvent OnFTUECompleted = new FTUECompletedEvent();

        [Header("Animators")]
        [SerializeField] private Animator mainMenuAnimator = null;
        [SerializeField] private Animator drinkPopUpAnimator = null;
        [SerializeField] private Animator fightPopUpAnimator = null;
        [SerializeField] private Animator eatPopUpAnimator = null;
        [SerializeField] private Animator shopAnimator = null;
        [SerializeField] private Animator lootboxConfirmationAnimator = null;
        [SerializeField] private Animator customizationAnimator = null;

        private const string MENU_FTUE_STEP1_TRIGGER = "FTUE1";
        private const string MENU_FTUE_STEP2_TRIGGER = "FTUE2";
        private const string MENU_FTUE_STEP3_TRIGGER = "FTUE3";
        private const string MENU_FTUE_STEP4_TRIGGER = "FTUE4";
        private const string MENU_FTUE_STEP5_TRIGGER = "FTUE5";
        private const string STANDARD_FTUE_TRIGGER = "FTUE";
        private const string FTUE_SHOP_PART2 = "FTUE_SHOP2";
        private const string CUSTOMIZATION_FTUE_ARMOR1_TRIGGER = "FTUEArmor1";
        private const string CUSTOMIZATION_FTUE_ARMOR2_TRIGGER = "FTUEArmor2";
        private const string CUSTOMIZATION_FTUE_END = "FTUEEnd";

        private GameData gameData = null;
        private FTUEStep currentStep = FTUEStep.None;

        private void Start()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            currentStep = gameData.ftueCompletedStep + 1;

            if (mainMenuAnimator != null)
                TriggerFTUEStep();
        }

        private void TriggerFTUEStep()
        {
            switch (currentStep)
            {
                case FTUEStep.Step1:    // drink step
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP1_TRIGGER);
                    drinkPopUpAnimator.enabled = true;
                    break;
                case FTUEStep.Step2:    // fight step
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP2_TRIGGER);
                    fightPopUpAnimator.enabled = true;
                    break;
                case FTUEStep.Step3:    // eat step
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP3_TRIGGER);
                    eatPopUpAnimator.enabled = true;
                    break;
                case FTUEStep.Step4:    // welcome reward step
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP4_TRIGGER);
                    OnWelcomeRewardStep?.Invoke();
                    break;
                case FTUEStep.Step5:    // shop step
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP4_TRIGGER);    // the menu continue to show shop button enabled
                    lootboxConfirmationAnimator.enabled = true;
                    break;
                case FTUEStep.Step6:    // customization step
                    lootboxConfirmationAnimator.enabled = false;
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP5_TRIGGER);    // menu shows customization button enabled
                    if (shopAnimator.gameObject.activeInHierarchy)
                        shopAnimator.SetTrigger(FTUE_SHOP_PART2);
                    break;
            }
        }

        
        public void SaveStepCompleted()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            gameData.ftueCompletedStep = currentStep;
            gameData.Save();
        }

        public void NextFTUEStep()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            SaveStepCompleted();
            currentStep += 1;
            TriggerFTUEStep();
        }

        public void EndFTUE()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            currentStep = FTUEStep.Completed;
            SaveStepCompleted();
            OnFTUECompleted?.Invoke();
        }

        public void SetFTUECanvasAnimation()
        {
            switch (currentStep)
            {
                case FTUEStep.Step5:    // shop step
                    shopAnimator.SetTrigger(STANDARD_FTUE_TRIGGER);
                    break;
                case FTUEStep.Step6:    // customization step
                    mainMenuAnimator.SetTrigger(MENU_FTUE_STEP5_TRIGGER);
                    if (customizationAnimator.gameObject.activeInHierarchy)
                        SetCustomizationAnimation();
                    break;
            }
        }

        private void SetCustomizationAnimation()
        {
            for (int i = 0; i < gameData.playerArmorPieces.Length; i++)
            {
                if (gameData.playerArmorPieces[i].quantityOwned > 0)
                {
                    switch (gameData.playerArmorPieces[i].ID)
                    {
                        case ArmorID.Armor0:
                            customizationAnimator.SetTrigger(STANDARD_FTUE_TRIGGER);
                            return;
                        case ArmorID.Armor1:
                            customizationAnimator.SetTrigger(CUSTOMIZATION_FTUE_ARMOR1_TRIGGER);
                            return;
                        case ArmorID.Armor2:
                            customizationAnimator.SetTrigger(CUSTOMIZATION_FTUE_ARMOR2_TRIGGER);
                            return;
                    }
                }
            }
        }

        public void FTUECustomizationLastAnimation()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            customizationAnimator.SetTrigger(CUSTOMIZATION_FTUE_END);
        }

        public void FTUECustomizationMaterialAnimation()
        {
            if (gameData.ftueCompletedStep == FTUEStep.Completed)
                return;

            customizationAnimator.SetTrigger(STANDARD_FTUE_TRIGGER);
        }

        public void GetGameData(GameData gd) => gameData = gd;

    }
}
