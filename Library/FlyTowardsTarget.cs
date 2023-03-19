using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class FlyTowardsTarget : MonoBehaviour
    {
        private void Start()
        {
            rig = GetComponent<Rigidbody>();
            
            ownUnit = transform.root.GetComponent<Unit>();
            SetTarget(ownUnit, targetingRadius);
        }

        private void Update()
        {
            switchCounter += Time.deltaTime;

            var shouldSwitch = target && switchTargetRandomly && switchCounter >= switchCooldown && switchDistance >= Vector3.Distance(transform.position, target.data.mainRig.position);
            
            if (target && !target.data.Dead && !shouldSwitch)
            {
                rig.AddForce((target.data.mainRig.position - transform.position).normalized * (forwardForce * Time.deltaTime));
            }
            else if (!target || target.data.Dead) SetTarget(ownUnit, targetingRadius);
            else if (shouldSwitch) SetTarget(ownUnit, switchRadius);

            if (!target && !ranEvent)
            {
                lostTargetEvent.Invoke();
                if (runEventOnce) ranEvent = true;
            }
        }

		
        private void SetTarget(bool doTeamCheck, float radius)
        {
            var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && (doTeamCheck && x.Team != ownUnit.Team || !doTeamCheck))
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
            if (foundUnits.Length != 0) target = switchTargetRandomly ? foundUnits[Random.Range(0, foundUnits.Length - 1)] : foundUnits[0];
        }
        
        private Unit target;
        private Unit ownUnit;
        private Rigidbody rig;
        private float switchCounter;
        private bool ranEvent;
        
        [Header("Fly Settings")]

        public float forwardForce = 20000f;

        [Header("Target Settings")] 
        
        public UnityEvent lostTargetEvent = new UnityEvent();
        
        public float targetingRadius = 80f;
        public bool runEventOnce = true;

        [Header("Switch Settings")]
        
        public bool switchTargetRandomly;
        public float switchDistance = 1f;
        public float switchCooldown = 2f;
        public float switchRadius = 10f;
    }
}