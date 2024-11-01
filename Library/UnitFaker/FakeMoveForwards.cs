using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeMoveForwards : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (!ownUnit.canMove || !ownUnit.onGround) return;
            var inRange = ownUnit.distanceToTarget < ownUnit.distanceToKeep;

            Force = Vector3.Lerp(Force, ownUnit.movementDirection.forward * (movementForce * (inRange ? -0.5f : 1f)),
                Time.deltaTime * lerpSpeed * 10f);
            
            foreach (var rig in ownUnit.rigs)
            {
                rig.AddForce(Force, ForceMode.Acceleration);
            }
        }

        private Vector3 Force;
        
        public FakeUnit ownUnit;

        [Header("Stats")] 
        
        public float lerpSpeed = 10f;
        public float movementForce = 10f;
    }
}