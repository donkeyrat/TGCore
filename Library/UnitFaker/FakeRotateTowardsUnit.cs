using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeRotateTowardsUnit : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (!ownUnit.onGround) return;
            
            var forward = ownUnit.core.transform.forward;
            var forward3 = ownUnit.movementDirection.forward;
            
            if (ignoreY)
            {
                forward.y = 0f;
                forward3.y = 0f;
            }
            if (lockYToZero) forward3.y = 0f;
            
            ownUnit.core.AddTorque(
                -Vector3.Cross(forward3, forward).normalized *
                (Mathf.Clamp(Vector3.Angle(forward3, forward), 0f, 15f) * rotationForce), ForceMode.Acceleration);
        }
        
        public FakeUnit ownUnit;

        [Header("Stats")] 
        
        public float rotationForce = 25f;

        public bool ignoreY;
        public bool lockYToZero;
    }
}