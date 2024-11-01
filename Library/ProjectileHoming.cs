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
            Move = GetComponent<MoveTransform>();
            OwnTeamHolder = GetComponent<TeamHolder>();
            Target = OwnTeamHolder.spawner.GetComponent<Unit>().data.targetData.unit;
            
            Weapon = transform.GetComponentInParent<Weapon>() ? transform.GetComponentInParent<Weapon>() : transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon;
            ReturnObject = Weapon.transform.FindChildRecursive(objectToReturnTo);
        }

        public override bool DoEffect(HitData hit)
        {
            var unit = hit.transform.root.GetComponent<Unit>();
            if (!unit) return true;
            
            HitList.Add(unit);
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
            if (!Target && !Returning) SetTarget();
            else if (autoTarget && Target && !Target.data.Dead)
            {
                var targetPos = Target.data.mainRig.position - transform.position;
                Move.velocity = targetPos.normalized * Move.selfImpulse.magnitude;
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetPos, Time.deltaTime * rotationSpeed, 0f));
            }
            
            if (Returning)
            {
                if (ReturnCounter >= 1f)
                {
                    Weapon.GetComponent<DelayEvent>().Go();
                    Destroy(gameObject);
                    Returning = false;
                    return;
                }
                transform.position = Vector3.Lerp(ReturnPosition, ReturnObject.position, ReturnCounter);
                transform.rotation = Quaternion.Lerp(ReturnRotation, ReturnObject.rotation, ReturnCounter);
                ReturnCounter += Time.deltaTime * returnSpeed;
            }
        }
        public void Return()
        {
            Returning = true;
            ReturnPosition = transform.position;
            ReturnRotation = transform.rotation;
        }

        public void SetTarget() 
        {
            var hits = Physics.SphereCastAll(transform.position, maxRange, Vector3.up, 0.1f, LayerMask.GetMask(new string[] { "MainRig" }));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != OwnTeamHolder.team && !HitList.Contains(x))
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToArray();
            
            if (foundUnits.Length > 0) Target = foundUnits[0];
            else finishEvent.Invoke();
        }
        
        private MoveTransform Move;
        private TeamHolder OwnTeamHolder;
        private readonly List<Unit> HitList = new List<Unit>();
        private Unit Target;
        
        [Header("Homing Settings")]
        
        public UnityEvent finishEvent = new UnityEvent();
        
        public float rotationSpeed = 5f;
        public float maxRange = 20f;
        public int hitLimit = 10;

        public bool autoTarget = true;

        [Header("Return Settings")]
        
        public float returnSpeed;
        public string objectToReturnTo;
        
        private bool Returning;
        
        private float ReturnCounter;

        private Transform ReturnObject;
        private Weapon Weapon;
        
        private Vector3 ReturnPosition;
        private Quaternion ReturnRotation;
    }
}
