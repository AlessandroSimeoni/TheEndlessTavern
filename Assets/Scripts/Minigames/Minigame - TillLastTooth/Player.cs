using Audio;
using Movement;
using System.Collections;
using UnityEngine;

namespace TillLastTooth
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LinearMovement))]
    public class Player : Participant
    {
        public PlayerSettings settings = null;

        public AudioClip dodgeSFX { private get; set; } = null;

        private LinearMovement movement = null;
        private bool canDodge = true;

        private float dodgeCooldownTime
        {
            get
            {
                return RoundBasedValue(settings.startingDodgeCooldownTime, settings.dodgeCooldownTimeRoundRate, settings.dodgeCooldownTimeDelta);
            }
        }
        private float dodgeTime
        {
            get
            {
                return RoundBasedValue(settings.startingDodgeTime, settings.dodgeTimeRoundRate, settings.dodgeTimeDelta);
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

        public delegate void PlayerEvent();
        public event PlayerEvent OnImpact = null;
        public event PlayerEvent OnDodge = null;

        public delegate void DodgeDoneEvent(float time);
        public event DodgeDoneEvent OnDodgeDone = null;

        private void Start() => movement = GetComponent<LinearMovement>();

        public void SetPlayerHealth(bool tournament) => currentHealth = tournament ? 1 : settings.health;

        public override void Stop()
        {
            base.Stop();
            movement.Stop();
            canDodge = false;
        }
        public override void GetDamage(int damage = 1)
        {
            base.GetDamage(damage);
            canDodge = false;
            animator.SetTrigger(DAMAGE);
        }

        public void Dodge(Vector2 direction)
        {
            if (!canDodge)
                return;

            if (direction == Vector2.up || direction == Vector2.down)
                return;

            movement.OnStandardMovementEnd += Attack;

            canDodge = false;
            OnDodge?.Invoke();
            Shift(new Vector3(direction.x, 0.0f, direction.y));
            AudioPlayer.instance.PlaySFX(dodgeSFX);
        }

        /// <summary>
        /// Initialize the player's animation fighting state
        /// </summary>
        public void InitFightingState()
        {
            if (animator == null)
                animator = GetComponent<Animator>();

            animator.SetTrigger(FIGHTING_STATE);
        }

        private void Shift(Vector3 direction)
        {
            movement.startingPosition = transform.position;
            movement.movementDirection = direction;
            movement.movementDistance = leftRightOffset;
            movement.movementSpeed = leftRightOffset / dodgeTime;
            movement.Move();
        }
        
        /// <summary>
        /// Player will move in the central position.
        /// (Called by an event in the punch animation)
        /// </summary>
        public void BackToCenter()
        {
            animator.SetFloat(PUNCH_SPEED, 1.0f);
            animator.SetBool(LEFT_PUNCH, false);
            animator.SetBool(RIGHT_PUNCH, false);

            movement.OnStandardMovementEnd -= Attack;
            movement.OnStandardMovementEnd += StartDodgeCooldown;

            Shift(-transform.position);
        }

        /// <summary>
        /// Called by animation event to handle damage animation
        /// </summary>
        public override void HandleDamageTaken()
        {
            if (transform.position.x != 0.0f)
                BackToCenter();

            canDodge = true;
        }

        private void StartDodgeCooldown() => StartCoroutine(DodgeCooldown());

        private IEnumerator DodgeCooldown()
        {
            OnDodgeDone?.Invoke(dodgeCooldownTime);
            yield return new WaitForSeconds(dodgeCooldownTime);
            movement.OnStandardMovementEnd -= StartDodgeCooldown;
            canDodge = true;
        }

        public override void Attack()
        {
            animator.SetBool((transform.position.x > 0.0f) ? RIGHT_PUNCH : LEFT_PUNCH, true);
        }

        public override void ReturnToIdle()
        {
            OnImpact?.Invoke();
            animator.SetFloat(PUNCH_SPEED, DEFAULT_PULL_BACK_PUNCH_TIME / returnToIdleTime);
        }

        /// <summary>
        /// Set the animation speed based on the attack time
        /// </summary>
        public override void SetPunchAttackSpeed() => animator.SetFloat(PUNCH_SPEED, DEFAULT_PUNCH_TIME / attackTime);

        public void Heal(int quantity) => currentHealth += quantity;
    }
}
