using UnityEngine;

namespace Movement
{
    [DisallowMultipleComponent]
    [RequireComponent (typeof(Rigidbody))]
    public class PhysicsOscillatoryMovement : OscillatoryBase
    {
        /*
         Oscillatory class based on a rigidbody.
         It moves the object using the Rigidbody.Move method in the fixed update
        */

        private Rigidbody rb = null;

        protected override void Start()
        {
            base.Start();
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate() => Move();

        protected override void Move()
        {
            if (!move)
                return;

            currentTime = (currentTime + Time.fixedDeltaTime) % period;

            sine = Mathf.Sin(DOUBLE_PI * currentTime * oscillationsPerSec);

            rb.Move(new Vector3(startingPositionX + sine * direction * absoluteMovementRange,
                                transform.position.y,
                                transform.position.z),
                    transform.rotation);

            // direction change detection
            if (Mathf.Abs(sine) >= directionChangeDetectionOffset && previousDirection != Mathf.Sign(sine * direction))
            {
                previousDirection = Mathf.Sign(sine * direction);
                InvokeDirectionEvent();
            }
        }
    }
}
