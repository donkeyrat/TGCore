using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class ProjectileHoming : ProjectileHitEffect 
    {
        public void Start()
        {
            move = GetComponent<MoveTransform>();
            ownTeamHolder = GetComponent<TeamHolder>();
            target = ownTeamHolder.spawner.GetComponent<Unit>().data.targetData.unit;
            
            weapon = transform.GetComponentInParent<Weapon>() ? transform.GetComponentInParent<Weapon>() : transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon;
            returnObject = weapon.transform.FindChildRecursive(objectToReturnTo);
        }

        public override bool DoEffect(HitData hit)
        {
            var unit = hit.transform.root.GetComponent<Unit>();
            if (!unit) return true;
            
            hitList.Add(unit);
            hitLimit -= 1;
            if (hitLimit <= 0) 
            {
                finishEvent.Invoke();
                return false;
            }
            SetTarget();
            return false;
        }

        public void Update() 
        {
            if (!target && !returning) SetTarget();
            else if (autoTarget && target && !target.data.Dead)
            {
                var targetPos = target.data.mainRig.position - transform.position;
                move.velocity = targetPos.normalized * move.selfImpulse.magnitude;
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetPos, Time.deltaTime * rotationSpeed, 0f));
            }
            
            if (returning)
            {
                if (returnCounter >= 1f)
                {
                    weapon.GetComponent<DelayEvent>().Go();
                    Destroy(gameObject);
                    returning = false;
                    return;
                }
                transform.position = Vector3.Lerp(returnPosition, returnObject.position, returnCounter);
                transform.rotation = Quaternion.Lerp(returnRotation, returnObject.rotation, returnCounter);
                returnCounter += Time.deltaTime * returnSpeed;
            }
        }
        public void Return()
        {
            returning = true;
            returnPosition = transform.position;
            returnRotation = transform.rotation;
        }

        public void SetTarget() 
        {
            var hits = Physics.SphereCastAll(transform.position, maxRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != ownTeamHolder.team && !hitList.Contains(x))
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
            
            if (foundUnits.Length > 0) target = foundUnits[0];
            else finishEvent.Invoke();
        }
        
        private MoveTransform move;
        private TeamHolder ownTeamHolder;
        private readonly List<Unit> hitList = new List<Unit>();
        private Unit target;
        
        [Header("Homing Settings")]
        
        public UnityEvent finishEvent = new UnityEvent();
        
        public float rotationSpeed = 5f;
        public float maxRange = 20f;
        public int hitLimit = 10;

        public bool autoTarget = true;

        [Header("Return Settings")]
        
        public float returnSpeed;
        public string objectToReturnTo;
        
        private bool returning;
        
        private float returnCounter;

        private Transform returnObject;
        private Weapon weapon;
        
        private Vector3 returnPosition;
        private Quaternion returnRotation;
    }
}
