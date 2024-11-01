using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{ 
    public class ProjectileAimAtRandomUnit : MonoBehaviour
    {
        private void Start()
        {
            TeamHolder = GetComponent<TeamHolder>();
            if (!TeamHolder) return;
            
            SetTarget();

            if (Target)
            {
                var compensation = GetComponent<Compensation>();
                var move = GetComponent<MoveTransform>();
                transform.rotation = Quaternion.LookRotation(compensation.GetCompensation(Target.data.mainRig.position, Target.data.mainRig.velocity, 0f));
                move.Initialize();
            }
        }

        public void SetTarget()
        {
            var hits = Physics.SphereCastAll(transform.position, targetingRadius, Vector3.up, 0.1f, mainRigMask);
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != TeamHolder.team)
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
            if (foundUnits.Length > 0) Target = foundUnits[Random.Range(0, foundUnits.Length - 1)];
        }

        private Unit Target;
        private TeamHolder TeamHolder;
        
        public float targetingRadius = 5f;
        
        public LayerMask mainRigMask;
    }

}