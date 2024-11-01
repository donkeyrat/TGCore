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
            Rig = GetComponent<Rigidbody>();
            
            OwnUnit = transform.root.GetComponent<Unit>();
            SetTarget(OwnUnit, targetingRadius);
        }

        private void Update()
        {
            SwitchCounter += Time.deltaTime;

            var shouldSwitch = Target && switchTargetRandomly && SwitchCounter >= switchCooldown && switchDistance >= Vector3.Distance(transform.position, Target.data.mainRig.position);
            
            if (Target && !Target.data.Dead && !shouldSwitch)
            {
                Rig.AddForce((Target.data.mainRig.position - transform.position).normalized * (forwardForce * Time.deltaTime));
            }
            else if (!Target || Target.data.Dead) SetTarget(OwnUnit, targetingRadius);
            else if (shouldSwitch) SetTarget(OwnUnit, switchRadius);

            if (!Target && !RanEvent)
            {
                lostTargetEvent.Invoke();
                if (runEventOnce) RanEvent = true;
            }
        }

		
        private void SetTarget(bool doTeamCheck, float radius)
        {
            var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && (doTeamCheck && x.Team != OwnUnit.Team || !doTeamCheck))
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
            if (foundUnits.Length != 0) Target = switchTargetRandomly ? foundUnits[Random.Range(0, foundUnits.Length - 1)] : foundUnits[0];
        }
        
        private Unit Target;
        private Unit OwnUnit;
        private Rigidbody Rig;
        private float SwitchCounter;
        private bool RanEvent;
        
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