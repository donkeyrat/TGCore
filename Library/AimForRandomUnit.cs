using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class AimForRandomUnit : MonoBehaviour
    {
        private void Start()
        {
            OwnTeamHolder = GetComponent<TeamHolder>();

            var target = SetTarget();
            if (target)
            {
                var targetRig = target.data.mainRig;
                
                var compensation = GetComponent<Compensation>();
                var move = GetComponent<MoveTransform>();
                if (compensation && move)
                {
                    transform.rotation = Quaternion.LookRotation(
                        compensation.GetCompensation(targetRig.position, targetRig.velocity, 0f) +
                        0.01f * spread * Random.insideUnitSphere);

                    var ownUnit = OwnTeamHolder.spawner.GetComponent<Unit>();
                    var ownWeapon = OwnTeamHolder.spawnerWeapon.GetComponentInParent<RangeWeapon>();
                    if (ownWeapon.extraSpreadInMelee != 0f && ownUnit && ownUnit.data.distanceToTarget < 5f)
                    {
                        transform.Rotate(ownWeapon.extraSpreadInMelee * Random.insideUnitSphere); 
                    }
                    
                    move.Initialize();
                }
            }
        }

        private Unit SetTarget() 
        {
            var hits = Physics.OverlapSphere(transform.position, maxRange, LayerMask.GetMask(new string[] { "MainRig" }));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != OwnTeamHolder.team)
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();

            if (foundUnits.Length > 0)
            {
                return foundUnits[useRandom ? Random.Range(0, foundUnits.Length - 1) : 0];
            }

            return null;
        }

        private TeamHolder OwnTeamHolder;

        public float maxRange = 30f;
        public float spread;
        public bool useRandom = true;
    }
}