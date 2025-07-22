using UnityEngine;

namespace Movement
{
    public class OscillatoryBase : MonoBehaviour
    {
        [Min(0.001f)] public float _oscillationsPerSec = 1.0f;
        [SerializeField, Min(0), Tooltip("The object will move from its position for absoluteMovementRange meters forward and backward")]
        protected float absoluteMovementRange = 3.25f;
        [SerializeField, Range(0.0f, 1.0f), Tooltip("The change in direction will be detected when the sine reaches this absolute value")]
        protected float directionChangeDetectionOffset = 0.98f;
        [SerializeField] protected bool reverseDirection = false;

        public delegate void DirectionChange();
        public event DirectionChange OnDirectionChange = null;

        private float currentPhase = 0.0f;

        public float oscillationsPerSec
        {
            get { return _oscillationsPerSec; }
            set
            {
                // when changing the frequence the phase also change causing a skip in the movement.
                // By saving the current phase before changing the frequence
                // and updating the current time so that the phase doesn't change, the movement skip is avoided
                currentPhase = DOUBLE_PI * currentTime * oscillationsPerSec;
                _oscillationsPerSec = value;
                period = 1 / oscillationsPerSec;
                currentTime = currentPhase / (DOUBLE_PI * oscillationsPerSec);
            }
        }

        protected float currentTime = 0.0f;
        protected float period = 1.0f;
        protected float sine = 0.0f;
        protected float startingPositionX = 0;
        protected float direction = 0.0f;
        protected float previousDirection = 0.0f;

        protected const float DOUBLE_PI = 2 * Mathf.PI;

        public bool move { get; set; } = false;

        protected virtual void Start()
        {
            period = 1 / oscillationsPerSec;
            startingPositionX = transform.position.x;
            direction = reverseDirection ? -1.0f : 1.0f;
            previousDirection = -direction;
        }

        protected virtual void Move()
        {
            if (!move)
                return;

            currentTime = (currentTime + Time.deltaTime) % period;

            sine = Mathf.Sin(DOUBLE_PI * currentTime * oscillationsPerSec);

            transform.position = new Vector3(startingPositionX + sine * direction * absoluteMovementRange,
                                             transform.position.y,
                                             transform.position.z);

            // direction change detection
            if (Mathf.Abs(sine) >= directionChangeDetectionOffset && previousDirection != Mathf.Sign(sine * direction))
            {
                previousDirection = Mathf.Sign(sine * direction);
                OnDirectionChange?.Invoke();
            }
        }

        protected void InvokeDirectionEvent() => OnDirectionChange?.Invoke();
    }
}
