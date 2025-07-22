using UnityEngine;

namespace TillLastTooth
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/Minigames/TillLastTooth/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        [Header("--- Health -------------------")]
        [Min(1)] public int health = 3;

        [Header("--- Time Actions -------------------")]
        [Header("1) Dodge Cooldown time\n")]
        [Min(0.01f), Tooltip("The Dodge Cooldown time at round 1")]
        public float startingDodgeCooldownTime = 3.0f;
        [Min(1), Tooltip("The Dodge Cooldown time will be decreased every this number of rounds")]
        public int dodgeCooldownTimeRoundRate = 3;
        [Min(0.0f), Tooltip("The Dodge Cooldown time will be decreased by this value every dodgeCooldownTimeRoundRate number of rounds")]
        public float dodgeCooldownTimeDelta = 0.25f;

        [Header("2) Dodge time\n")]
        [Min(0.01f), Tooltip("The Dodge time at round 1")]
        public float startingDodgeTime = 0.5f;
        [Min(1), Tooltip("The Dodge time will be decreased every this number of rounds")]
        public int dodgeTimeRoundRate = 0;
        [Min(0.0f), Tooltip("The Dodge time will be decreased by this value every dodgeTimeRoundRate number of rounds")]
        public float dodgeTimeDelta = 0.1f;

        [Header("3) Attack time\n")]
        [Min(0.01f), Tooltip("The Attack time at round 1")]
        public float startingAttackTime = 2.0f;
        [Min(1), Tooltip("The Attack time will be decreased every this number of rounds")]
        public int attackTimeRoundRate = 3;
        [Min(0.0f), Tooltip("The Attack time will be decreased by this value every attackTimeRoundRate number of rounds")]
        public float attackTimeDelta = 0.25f;

        [Header("4) Return to Idle time\n")]
        [Min(0.01f), Tooltip("The Return to Idle time at round 1")]
        public float startingReturnToIdleTime = 2.0f;
        [Min(1), Tooltip("The Return to Idle time will be decreased every this number of rounds")]
        public int returnToIdleTimeRoundRate = 3;
        [Min(0.0f), Tooltip("The Return to Idle time will be decreased by this value every returnToIdleTimeRoundRate number of rounds")]
        public float returnToIdleTimeDelta = 0.25f;
    }
}
