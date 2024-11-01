using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeUnit : MonoBehaviour
    {
        private void Start()
        {
            rigs = GetComponentsInChildren<Rigidbody>();
            
            SetTarget();
        }
        
        private void Update()
        {
            if (!target || target.data.Dead) SetTarget();
            else
            {
                distanceToTarget = Vector3.Distance(target.data.mainRig.position, core.position);
                
                var directionToTarget = (target.data.mainRig.position - core.position).normalized;
                angleToTarget = Vector3.Angle(movementDirection.forward, directionToTarget);
                
                
                var speed = Mathf.Clamp(Time.deltaTime * turnSpeed, 0f, 1f);
                movementDirection.forward = Vector3.Lerp(movementDirection.forward, directionToTarget, speed);
            }
        }
        
        public void SetTarget()
        {
            var hits = Physics.SphereCastAll(core.position, targetingRange, Vector3.up, 0.1f, LayerMask.GetMask("MainRig"));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != team.team)
                .OrderBy(x => (x.data.mainRig.transform.position - core.position).magnitude / (x.targetingPriorityMultiplier != 0 ? x.targetingPriorityMultiplier : 1f))
                .Distinct()
                .ToArray();

            if (foundUnits.Length > 0) target = foundUnits[0];
        }

        [HideInInspector]
        public Rigidbody[] rigs;
        
        public Rigidbody core;
        
        [Header("Stats")] 
        
        public TeamHolder team;

        public bool canMove = true;
        
        [Header("Direction")] 
        
        public Transform movementDirection;
        
        public float turnSpeed;

        [Header("Grounded")] 
        
        public bool onGround;
        public float groundedTime;
        public float groundLeeway = 0.3f;
        public float distanceToGround;
        public Vector3 upHillVector;
        
        [Header("Targeting")]
        
        public Unit target;
        
        public float distanceToTarget = 999f;
        public float angleToTarget = 999f;
        
        public float distanceToKeep = 1.5f;
        public float targetingRange = 50f;
    }
}