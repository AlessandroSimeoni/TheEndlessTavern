using UnityEngine;

namespace Movement
{
    [DisallowMultipleComponent]
    public class OscillatoryMovement : OscillatoryBase
    {
        private void Update() => Move();
    }
}