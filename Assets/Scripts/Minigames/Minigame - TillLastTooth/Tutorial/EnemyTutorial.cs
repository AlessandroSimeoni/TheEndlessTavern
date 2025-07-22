using System.Collections;
using UnityEngine;

namespace TillLastTooth
{
    public class EnemyTutorial : Enemy
    {
        public int attackCount { get; private set; } = -1;

        public delegate void EnemyExit();
        public event EnemyExit OnEnemyExit = null;

        public override void InitParameters()
        {
            currentHealth = 2;
            HealthInitInvoke();
            idleTime = 0.01f;
            popUpTime = 0.6f;
        }

        protected override IEnumerator LeftOrRightChoice()
        {
            attackCount++;
            yield return new WaitForSeconds(idleTime);
            hitPosition = (attackCount == 0) ? Vector3.left : Vector3.right;
            InvokeAttackCharging();
        }

        protected override void InvokeKO() => OnEnemyExit?.Invoke();
    }
}
