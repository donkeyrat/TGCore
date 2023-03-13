using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class FlyTowardsTarget : MonoBehaviour
    {
        private void Start()
        {
            rig = GetComponent<Rigidbody>();
            
            ownUnit = transform.root.GetComponent<Unit>();
            SetTarget(ownUnit);
        }

        private void Update()
        {
            switchCounter += Time.deltaTime;

            var shouldSwitch = switchTargetRandomly && switchCounter >= switchCooldown && switchDistance >= Vector3.Distance(transform.position, target.data.mainRig.position);
            if (target && !target.data.Dead && !shouldSwitch)
            {
                rig.AddForce((target.data.mainRig.position - transform.position).normalized * (forwardForce * Time.deltaTime));
            }
            else if (target.data.Dead || shouldSwitch)
            {
                SetTarget(ownUnit);
            }
        }

		
        private void SetTarget(bool doTeamCheck)
        {
            var hits = Physics.SphereCastAll(transform.position, 80f, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
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

        public float forwardForce = 20000f;

        public bool switchTargetRandomly;
        public float switchDistance = 1f;
        public float switchCooldown = 2f;
    }
}