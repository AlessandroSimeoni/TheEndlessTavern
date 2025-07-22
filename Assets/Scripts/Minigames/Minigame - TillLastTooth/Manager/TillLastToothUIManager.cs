using EndlessTavernUI;
using UnityEngine;

namespace TillLastTooth
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PopUpManager))]
    [RequireComponent(typeof(DodgeCooldownManager))]
    public class TillLastToothUIManager : MinigameUIManager
    {
        [Header("UI Enemy Health")]
        [SerializeField] private EnemyHealthBar enemyHealthBarPrefab = null;

        public delegate void PopUpEvent();
        public event PopUpEvent OnPopUpDone = null;

        private PopUpManager popUpManager = null;
        private DodgeCooldownManager dodgeCooldownManager = null;
        private EnemyHealthBar enemyHealthBarInstance = null;

        protected override void Start()
        {
            base.Start();

            popUpManager = GetComponent<PopUpManager>();
            popUpManager.OnPopUpDone += HandlePopUpDone;

            dodgeCooldownManager = GetComponent<DodgeCooldownManager>();
        }

        public void InitEnemyHealthBar(int enemyHp)
        {
            if (enemyHealthBarInstance == null)
                enemyHealthBarInstance = Instantiate<EnemyHealthBar>(enemyHealthBarPrefab, healthBarParent);

            enemyHealthBarInstance.currentHeartIndex = enemyHp - 1;
            enemyHealthBarInstance.RefillHearts();
            enemyHealthBarInstance.heartsSecondRow.SetActive(enemyHp > 2);

            for(int i = enemyHealthBarInstance.heart.Length-1; i >= 0; i--)
                enemyHealthBarInstance.heart[i].gameObject.SetActive(i < enemyHp);
        }

        public void DamageEnemyHealthBar(int damage)
        {
            for (int i = 0; i < damage; i++)
                enemyHealthBarInstance.HealthBarDamage();
        }

        public void GameOver()
        {
            HideAllPopUp(0.0f);
            StopAllCoroutines();
        }

        // pop up
        public void ActivatePopUp(Vector3 position, float fadeTime) => popUpManager.ActivatePopUp(position, fadeTime);
        public void HideAllPopUp(float fadeTime) => popUpManager.HideAllPopUp(fadeTime);
        private void HandlePopUpDone() => OnPopUpDone?.Invoke();
        public void ShowMissPopUp() => popUpManager.TriggerMissPopUp();

        // dodge cooldown slider
        public void EmptyDodgeSlider() => dodgeCooldownManager.EmptyDodgeSlider();
        public void LoadCooldown(float time) => dodgeCooldownManager.LoadCooldownSlider(time);
        public void CooldownReady() => dodgeCooldownManager.ForceChangeColor();
    }
}