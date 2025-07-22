using System.Collections;
using UnityEngine;

namespace Movement
{
    [DisallowMultipleComponent]
    public class LinearMovement : MonoBehaviour
    {
        [Header("Basic movement")]
        [Tooltip("Object will move starting from this position")] 
        public Vector3 startingPosition = Vector3.zero;
        [Tooltip("The movement direction")] 
        public Vector3 movementDirection = Vector3.left;
        [Min(0.0f), Tooltip("The movement speed [m/s]")] 
        public float movementSpeed = 1.0f;
        [Min(0.0f), Tooltip("The total distance [m] to cover")] 
        public float movementDistance = 10.0f;
        [Header("Start & Stop movement")]
        [SerializeField, Tooltip("If enabled the object will stop at stopOffset distance for stopTime seconds, then it will move again")]
        private bool startAndStopEnabled = false;
        [Min(0.0f), Tooltip("The offset [m] after the startingPosition where the object will stop")] 
        public float stopOffset = 5.0f;
        [Min(0.0f), Tooltip("The seconds the object will wait before moving again")] 
        public float stopTime = 3.0f;

        private bool isMoving = false;
        private bool canWait = true;

        public float waitingTime { get; private set; } = 0.0f;

        public delegate void Movement();
        public event Movement OnStop = null;
        public event Movement OnMovementResumed = null;
        public event Movement OnStartAndStopMovementEnd = null;
        public event Movement OnStandardMovementEnd = null;

        public delegate void WaitingEvent(float ratio);
        public event WaitingEvent OnWaiting = null;

#if UNITY_EDITOR
        private bool triggerMove = false;

        private void Update()
        {
            if (!triggerMove) return;

            Move();
        }

        [ContextMenu("ToggleMovement")]
        public void ToggleMovement() => triggerMove = !triggerMove;
#endif

        public void Move()
        {
            if (isMoving) 
                return;

            isMoving = true;
            transform.position = startingPosition;
            if (startAndStopEnabled)
                StartCoroutine(StartAndStopMovement());
            else
                StartCoroutine(StandardMovement(startingPosition, movementDistance));
        }

        /// <summary>
        /// Move the object for stopOffset meters, wait stopTime seconds and then resume the movement
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartAndStopMovement()
        {
            // standard linear movement of stopOffset meters
            yield return StartCoroutine(StandardMovement(startingPosition, stopOffset));

            // object has stopped moving
            OnStop?.Invoke();

            // custom waiting loop that can be interrupted if needed
            waitingTime = 0;
            canWait = true;
            while (waitingTime < stopTime && canWait)
            {
                waitingTime += Time.deltaTime;
                OnWaiting?.Invoke(1.0f - (waitingTime/stopTime));
                yield return null;
            }
            OnWaiting?.Invoke(0.0f);

            Vector3 currentPosition = startingPosition + movementDirection.normalized * stopOffset;
            OnMovementResumed?.Invoke();

            // resume movement to the final destination
            yield return StartCoroutine(StandardMovement(currentPosition, movementDistance - stopOffset));

            isMoving = false;
            OnStartAndStopMovementEnd?.Invoke();
        }

        /// <summary>
        /// Move the object for distance meters from the starting position
        /// </summary>
        /// <param name="startingPosition">the starting position</param>
        /// <param name="distance">the distance to cover</param>
        /// <returns></returns>
        private IEnumerator StandardMovement(Vector3 startingPosition, float distance)
        {
            Vector3 destination = startingPosition + movementDirection.normalized * distance;

            while (transform.position != destination)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            }
            yield return null;

            isMoving = startAndStopEnabled;
            OnStandardMovementEnd?.Invoke();
        }

        public void Stop()
        {
            StopAllCoroutines();
            isMoving = false;
        }
        public void StopWaitingTime() => canWait = false;
    }
}
