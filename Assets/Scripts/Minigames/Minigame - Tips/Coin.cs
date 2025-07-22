using Movement;
using UnityEngine;

namespace Tips
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(OscillatoryMovement))]
    public class Coin : MonoBehaviour
    {
        private Rigidbody rb = null;
        private OscillatoryMovement movement = null;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            movement = GetComponent<OscillatoryMovement>();
        }

        private void Start() => rb.isKinematic = true;

        public void Drop()
        {
            movement.move = false;
            rb.isKinematic = false;
        }
    }
}
