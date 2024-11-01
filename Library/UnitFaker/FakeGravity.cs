using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeGravity : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (ownUnit.onGround) return;

            ownUnit.core.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        
        public FakeUnit ownUnit;
        
        public float gravity;
    }
}