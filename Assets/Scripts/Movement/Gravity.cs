using UnityEngine;

namespace Movement
{
    public class Gravity : MonoBehaviour
    {
        [Header("Ground Check")]
        [SerializeField] private float sphereCastStartingOffsetHeight = 0.25f;
        [SerializeField] private float sphereCastRadius = 0.2f;
        [SerializeField] private float sphereCastDistance = 0.5f;
        [SerializeField] private float floorDetectionOffset = 0.02f;
        [SerializeField] private LayerMask ignoreLayers;

        public Vector3 currentAppliedGravity { private get; set; } = Vector3.zero;

        private bool isGrounded
        {
            get
            {
                Ray ray = new Ray(transform.position + Vector3.up * sphereCastStartingOffsetHeight, Physics.gravity.normalized);
                float trueDistance = sphereCastDistance - sphereCastRadius + floorDetectionOffset;
                return Physics.SphereCast(ray, sphereCastRadius, trueDistance, ~ignoreLayers);
            }
        }

        private void FixedUpdate()
        {
            currentAppliedGravity = isGrounded ? Vector3.zero : currentAppliedGravity + Physics.gravity * Time.fixedDeltaTime;
            transform.position += currentAppliedGravity * Time.fixedDeltaTime;
        }
    }
}
