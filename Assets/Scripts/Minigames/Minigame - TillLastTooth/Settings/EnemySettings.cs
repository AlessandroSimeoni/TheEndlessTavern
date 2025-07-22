using EditorScripting;
using UnityEngine;

namespace TillLastTooth
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "ScriptableObjects/Minigames/TillLastTooth/EnemySettings")]
    public class EnemySettings : ScriptableObject
    {
        [Header("--- Health -------------------")]
        [Header("Health range")]
        public MinMaxRange health = new MinMaxRange(1.0f, 15.0f);
        [Min(1), Tooltip("The max health at round 1")]
        public int startingMaxHealth = 3;
        [Header("Minimum health increase")]
        [Min(1), Tooltip("The minimum health will update every this number of rounds")]
        public int minHealthRoundRate = 7;
        [Min(1), Tooltip("The minimum health will increase by this value every minHealthRoundRate rounds")]
        public int minHealthDelta = 1;
        [Header("Maximum health increase")]
        [Min(1), Tooltip("The maximum health will update every this number of rounds")]
        public int maxHealthRoundRate = 3;
        [Min(1), Tooltip("The maximum health will increase by this value every maxHealthRoundRate rounds")]
        public int maxHealthDelta = 1;

        [Header("--- Scene enter/exit -------------------")]
        [Min(0.1f), Tooltip("Enemy will enter/exit the scene with this speed")]
        public float movementSpeed = 20.0f;

        [Header("--- Time Actions -------------------")]
        [Header("1) Idle time\n")]
        [Min(0.01f), Tooltip("The minimum idle time at round 1")]
        public float startingMinIdleTime = 1.5f;
        [Min(0.01f), Tooltip("The maximum idle time at round 1")]
        public float startingMaxIdleTime = 2.0f;
        [Header("Minimum Idle time decrease")]
        [Min(1), Tooltip("The Minimum Idle time will be decreased every this number of rounds")]
        public int minIdleTimeRoundRate = 2;
        [Min(0.0f), Tooltip("The Minimum Idle time will be decreased by this value every minIdleTimeRoundRate number of rounds")]
        public float minIdleTimeDelta = 0.1f;
        [Header("Maximum Idle time decrease")]
        [Min(1), Tooltip("The Maximum Idle time will be decreased every this number of rounds")]
        public int maxIdleTimeRoundRate = 2;
        [Min(0.0f), Tooltip("The Maximum Idle time will be decreased by this value every maxIdleTimeRoundRate number of rounds")]
        public float maxIdleTimeDelta = 0.1f;

        [Header("2) Pop Up time\n")]
        [Min(0.01f), Tooltip("The minimum Pop Up time at round 1")]
        public float startingMinPopUpTime = 1.0f;
        [Min(0.01f), Tooltip("The maximum Pop Up time at round 1")]
        public float startingMaxPopUpTime = 1.5f;
        [Header("Minimum Pop Up time decrease")]
        [Min(1), Tooltip("The Minimum Pop Up time will be decreased every this number of rounds")]
        public int minPopUpTimeRoundRate = 3;
        [Min(0.0f), Tooltip("The Minimum Pop Up time will be decreased by this value every minPopUpTimeRoundRate number of rounds")]
        public float minPopUpTimeDelta = 0.1f;
        [Header("Maximum Pop Up time decrease")]
        [Min(1), Tooltip("The Maximum Pop Up time will be decreased every this number of rounds")]
        public int maxPopUpTimeRoundRate = 3;
        [Min(0.0f), Tooltip("The Maximum Pop Up time will be decreased by this value every maxPopUpTimeRoundRate number of rounds")]
        public float maxPopUpTimeDelta = 0.1f;

        [Header("3) Attack time\n")]
        [Min(0.01f), Tooltip("The Attack time at round 1")]
        public float startingAttackTime = 0.75f;
        [Min(1), Tooltip("The Attack time will be decreased every this number of rounds")]
        public int attackTimeRoundRate = 1;
        [Min(0.0f), Tooltip("The Attack time will be decreased by this value every attackTimeRoundRate number of rounds")]
        public float attackTimeDelta = 0.1f;

        [Header("4) Return to Idle time\n")]
        [Min(0.01f), Tooltip("The Return to Idle time at round 1")]
        public float startingReturnToIdleTime = 2.0f;
        [Min(1), Tooltip("The Return to Idle time will be decreased every this number of rounds")]
        public int returnToIdleTimeRoundRate = 3;
        [Min(0.0f), Tooltip("The Return to Idle time will be decreased by this value every returnToIdleTimeRoundRate number of rounds")]
        public float returnToIdleTimeDelta = 0.25f;
    }
}
