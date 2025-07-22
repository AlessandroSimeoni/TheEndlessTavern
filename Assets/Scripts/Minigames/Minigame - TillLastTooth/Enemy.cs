using Audio;
using Movement;
using System.Collections;
using UnityEngine;

namespace TillLastTooth
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LinearMovement))]
    public class Enemy : Participant
    {
        public EnemySettings settings = null;

        [SerializeField] private AudioClip entranceSFX = null;
        [SerializeField] private AudioClip damageSFX = null;
        [SerializeField] private AudioClip attackSFX = null;
        [SerializeField] private AudioClip koSFX = null;

        protected float idleTime = 0.0f;
        protected float popUpTime = 0.0f;
        protected Vector3 hitPosition = Vector3.zero;
        private LinearMovement movement = null;
        private bool ko = false;
        private Coroutine attackingStateCoroutine = null;

        private int minHP { 
            get
            {
                return Mathf.Clamp((int)RoundBasedValue(settings.health.min, settings.minHealthRoundRate, -settings.minHealthDelta),
                                   (int)settings.health.min,
                                   (int)settings.health.max);
            }
        }
        private int maxHP
        {
            get
            {
                return Mathf.Clamp((int)RoundBasedValue(settings.startingMaxHealth, settings.maxHealthRoundRate, -settings.maxHealthDelta),
                                   (int)settings.health.min,
                                   (int)settings.health.max);
            }
        }
        private float minIdleTime
        {
            get
            {
                return RoundBasedValue(settings.startingMinIdleTime, settings.minIdleTimeRoundRate, settings.minIdleTimeDelta);
            }
        }
        private float maxIdleTime
        {
            get
            {
                return RoundBasedValue(settings.startingMaxIdleTime, settings.maxIdleTimeRoundRate, settings.maxIdleTimeDelta);
            }
        }
        private float minPopUpTime
        {
            get
            {
                return RoundBasedValue(settings.startingMinPopUpTime, settings.minPopUpTimeRoundRate, settings.minPopUpTimeDelta);
            }
        }
        private float maxPopUpTime
        {
            get
            {
                return RoundBasedValue(settings.startingMaxPopUpTime, settings.maxPopUpTimeRoundRate, settings.maxPopUpTimeDelta);
            }
        }
        private float attackTime
        {
            get
            {
                return RoundBasedValue(settings.startingAttackTime, settings.attackTimeRoundRate, settings.attackTimeDelta);
            }
        }
        private float returnToIdleTime
        {
            get
            {
                return RoundBasedValue(settings.startingReturnToIdleTime, settings.returnToIdleTimeRoundRate, settings.returnToIdleTimeDelta);
            }
        }
        public bool vulnerable { get; private set; } = false;

        public delegate void AttackChargingEvent(Vector3 position, float time);
        public event AttackChargingEvent OnAttackCharging = null;

        public delegate void ImpactEvent(Vector3 impactDirection, float time);
        public event ImpactEvent OnImpact = null;

        public event HealthInitEvent OnHealthInit = null;
        public delegate void HealthInitEvent(int hp);

        public delegate void HealthDamageEvent(int damage);
        public event HealthDamageEvent OnHealthDamage = null;

        private void Start()
        {
            InitParameters();

            movement = GetComponent<LinearMovement>();
            movement.movementSpeed = settings.movementSpeed;
        }

        public virtual void InitParameters()
        {
            currentHealth = Random.Range(minHP, maxHP + 1);
            OnHealthInit?.Invoke(currentHealth);
            idleTime = Random.Range(minIdleTime, maxIdleTime);
            popUpTime = Random.Range(minPopUpTime, maxPopUpTime);
        }

        protected void HealthInitInvoke() => OnHealthInit?.Invoke(currentHealth);

        public override void GetDamage(int damage = 1)
        {
            currentHealth = (currentHealth - damage) <= 0 ? 0 : currentHealth - damage;
            AudioPlayer.instance.PlaySFX(damageSFX);
            OnHealthDamage?.Invoke(damage);

            animator.SetBool(DAMAGE, true);

            if (currentHealth == 0)
            {
                AudioPlayer.instance.PlaySFX(koSFX);
                Stop();
                Exit();
            }
        }

        public void Entrance()
        {
            movement.OnStandardMovementEnd -= InvokeKO;
            movement.OnStandardMovementEnd += StartAttackFlow;

            movement.startingPosition = transform.position;
            movement.movementDistance = Mathf.Abs(transform.position.x);
            movement.movementDirection = new Vector3(-transform.position.x, 0.0f, 0.0f);
            movement.Move();
        }

        private void Exit()
        {
            movement.OnStandardMovementEnd -= StartAttackFlow;
            movement.OnStandardMovementEnd += InvokeKO;

            movement.startingPosition = transform.position;
            movement.Move();
        }

        private void StartAttackFlow()
        {
            AudioPlayer.instance.PlaySFX(entranceSFX);
            ko = false;
            StartCoroutine(LeftOrRightChoice());
        }

        public void AttackSideChoice()
        {
            vulnerable = false;
            animator.SetFloat(PUNCH_SPEED, 1.0f);
            animator.SetBool(LEFT_PUNCH, false);
            animator.SetBool(RIGHT_PUNCH, false);
            animator.SetBool(DAMAGE, false);

            if (ko || attackingStateCoroutine != null)
                return;

            attackingStateCoroutine = StartCoroutine(LeftOrRightChoice());
        }

        /// <summary>
        /// randomly chooses which side the enemy will hit
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator LeftOrRightChoice()
        {
            yield return new WaitForSeconds(idleTime);

            int choice = Random.Range(0, 2);
            hitPosition = (choice == 0) ? Vector3.left : Vector3.right;
            InvokeAttackCharging();
        }

        /// <summary>
        /// invoke attack charging, this will call the pop up manager.
        /// Wrapped in this method for inheritance
        /// </summary>
        protected void InvokeAttackCharging() => OnAttackCharging?.Invoke(hitPosition, popUpTime);

        public override void Stop()
        {
            ko = true;
            vulnerable = false;
            base.Stop();
        }

        /// <summary>
        /// when pop up is done this method is called, triggering the punch animation
        /// </summary>
        public override void Attack()
        {
            AudioPlayer.instance.PlaySFX(attackSFX);
            animator.SetBool((hitPosition == Vector3.left) ? RIGHT_PUNCH : LEFT_PUNCH, true);

            attackingStateCoroutine = null;
        }

        /// <summary>
        /// when punch reaches the target it begins the return to idle phase, and this method is called (by animation event)
        /// </summary>
        public override void ReturnToIdle()
        {
            OnImpact?.Invoke(hitPosition, returnToIdleTime);
            animator.SetFloat(PUNCH_SPEED, DEFAULT_PULL_BACK_PUNCH_TIME / returnToIdleTime);

            if (ko)
                return;

            vulnerable = true;
        }

        /// <summary>
        /// when punching, the animation speed will be adjusted with this method
        /// </summary>
        public override void SetPunchAttackSpeed() => animator.SetFloat(PUNCH_SPEED, DEFAULT_PUNCH_TIME / attackTime);

        public override void HandleDamageTaken() => AttackSideChoice();
    }
}
