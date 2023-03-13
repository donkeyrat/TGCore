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
            teamHolder = GetComponent<TeamHolder>();
        }
    
        private void Update()
        {
            if (pointUp) transform.up = Vector3.up;
        
            if (target)
            {
                var targetPos = target.data.mainRig.position;
                var directionToMoveIn = new Vector3(targetPos.x, moveOnY ? targetPos.y : transform.position.y, targetPos.z) - transform.position;
                transform.position += directionToMoveIn.normalized * (Time.deltaTime * moveSpeed);

                if (Vector3.Distance(transform.position, directionToMoveIn) < rangeToStopAt) StartCoroutine(AddUnitToHitlist(target));
            }
        
            SetTarget();
        }

        private IEnumerator AddUnitToHitlist(Unit target)
        {
            if (target) hitList.Add(target);
            
            yield return new WaitForSeconds(1.5f);
            
            if (target && hitList.Contains(target)) hitList.Remove(target);
        }

        public void SetTarget()
        {
            var hits = Physics.SphereCastAll(transform.position, targetingRange, Vector3.up, 0.1f, LayerMask.GetMask("MainRig"));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => teamHolder && x && !x.data.Dead && x.Team != teamHolder.team && !hitList.Contains(x))
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
        
            if (foundUnits.Length > 0) target = foundUnits[0];
        }

        private Unit target;

        private TeamHolder teamHolder;
        
        public bool moveOnY;

        public bool pointUp = true;

        public float rangeToStopAt = 0.5f;

        public float targetingRange = 100f;

        public float moveSpeed = 1f;

        private List<Unit> hitList = new List<Unit>();
    }
}