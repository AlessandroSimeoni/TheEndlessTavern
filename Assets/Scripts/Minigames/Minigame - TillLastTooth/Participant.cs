using UnityEngine;

public abstract class Participant : MonoBehaviour
{
    [SerializeField] protected Animator animator = null;

    protected const string FIGHTING_STATE = "IdleFight";
    protected const string RIGHT_PUNCH = "RightPunch";
    protected const string LEFT_PUNCH = "LeftPunch";
    protected const string DAMAGE = "Damage";
    protected const string PUNCH_SPEED = "PunchSpeed";
    protected const float DEFAULT_PUNCH_TIME = 0.34f;
    protected const float DEFAULT_PULL_BACK_PUNCH_TIME = 0.31f;

    public int currentHealth { get; protected set; }
    public int currentRound { protected get; set; }
    public int roundLock { protected get; set; }
    public float leftRightOffset { protected get; set; }

    public delegate void KOEvent();
    public event KOEvent OnKO = null;

    public virtual void GetDamage(int damage = 1)
    {
        currentHealth = (currentHealth - damage) < 0 ? 0 : currentHealth - damage;

        if (currentHealth == 0)
            InvokeKO();
    }
    protected float RoundBasedValue(float startingValue, int roundRate, float delta)
    {
        int round = Mathf.Clamp(currentRound, 1, roundLock);
        return startingValue - (round - 1) / roundRate * delta;
    }

    protected virtual void InvokeKO() => OnKO?.Invoke();
    public virtual void Stop() => StopAllCoroutines();

    public abstract void Attack();
    public abstract void ReturnToIdle();
    public abstract void SetPunchAttackSpeed();
    public abstract void HandleDamageTaken();
}