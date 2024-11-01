using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class MoveTowardsTarget : MonoBehaviour
    {
        private void Start()
        {
            TeamHolder = GetComponent<TeamHolder>();
        }
    
        private void Update()
        {
            if (pointUp) transform.up = Vector3.up;
        
            if (Target)
            {
                var targetPos = Target.data.mainRig.position;
                var directionToMoveIn = new Vector3(targetPos.x, moveOnY ? targetPos.y : transform.position.y, targetPos.z) - transform.position;
                transform.position += directionToMoveIn.normalized * (Time.deltaTime * moveSpeed);

                if (Vector3.Distance(transform.position, directionToMoveIn) < rangeToStopAt) StartCoroutine(AddUnitToHitlist(Target));
            }
        
            SetTarget();
        }

        private IEnumerator AddUnitToHitlist(Unit target)
        {
            if (target) HitList.Add(target);
            
            yield return new WaitForSeconds(1.5f);
            
            if (target && HitList.Contains(target)) HitList.Remove(target);
        }

        public void SetTarget()
        {
            var hits = Physics.SphereCastAll(transform.position, targetingRange, Vector3.up, 0.1f, LayerMask.GetMask("MainRig"));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => TeamHolder && x && !x.data.Dead && x.Team != TeamHolder.team && !HitList.Contains(x))
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
        
            if (foundUnits.Length > 0) Target = foundUnits[0];
        }

        private Unit Target;

        private TeamHolder TeamHolder;
        
        public bool moveOnY;

        public bool pointUp = true;

        public float rangeToStopAt = 0.5f;

        public float targetingRange = 100f;

        public float moveSpeed = 1f;

        private List<Unit> HitList = new List<Unit>();
    }
}