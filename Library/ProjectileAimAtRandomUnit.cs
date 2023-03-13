using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{ 
    public class ProjectileAimAtRandomUnit : MonoBehaviour
    {
        private void Start()
        {
            teamHolder = GetComponent<TeamHolder>();
            if (!teamHolder) return;
            
            SetTarget();

            if (target)
            {
                var compensation = GetComponent<Compensation>();
                var move = GetComponent<MoveTransform>();
                transform.rotation = Quaternion.LookRotation(compensation.GetCompensation(target.data.mainRig.position, target.data.mainRig.velocity, 0f));
                move.Initialize();
            }
        }

        public void SetTarget()
        {
            var hits = Physics.SphereCastAll(transform.position, targetingRadius, Vector3.up, 0.1f, mainRigMask);
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != teamHolder.team)
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
            if (foundUnits.Length > 0) target = foundUnits[Random.Range(0, foundUnits.Length - 1)];
        }

        private Unit target;
        private TeamHolder teamHolder;
        
        public float targetingRadius = 5f;
        
        public LayerMask mainRigMask;
    }

}