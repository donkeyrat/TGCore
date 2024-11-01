using System;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class MeleeWeaponSpawnToggleable : MonoBehaviour
    {
        private void Start()
        {
            Rig = GetComponent<Rigidbody>();
            TeamHolder.GetTeamRelevantComponents(transform, ref Unit, ref RootTeamHolder);
            
            if (!startOnCooldown) Counter = cooldown;
        }

        private void Update()
        {
            Counter += Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Counter < cooldown || !toggled || collision.collider.transform.root == transform.root ||
                !objectToSpawn) return;

            var num = 0f;
            if (Rig)
            {
                if (collision.rigidbody)
                {
                    num = collision.impulse.magnitude / (Rig.mass + 10f) * 0.3f;
                }
                else
                {
                    num = collision.impulse.magnitude / Rig.mass * 0.3f;
                }
            }
            num *= impactMultiplier;
            
            if (num < 1f) return;
            
            switch (collisionTarget)
            {
                case CollisionTarget.Units when !collision.transform.root.GetComponent<Unit>():
                case CollisionTarget.Rigidbodies when !collision.rigidbody:
                case CollisionTarget.Static when collision.rigidbody:
                    return;
                case CollisionTarget.EnemyUnits:
                {
                    var enemyUnit = collision.transform.root.GetComponent<Unit>();
                    if (!enemyUnit || (RootTeamHolder && RootTeamHolder.team == enemyUnit.Team) || (Unit && Unit.Team == enemyUnit.Team))
                    {
                        return;
                    }
                    break;
                }
                default:
                case CollisionTarget.All:
                    break;
            }
            
            var spawnRot = Quaternion.identity;
            switch (rot)
            {
                case Rot.Normal:
                    spawnRot = Quaternion.LookRotation(collision.GetContact(0).normal);
                    break;
                case Rot.InverseNormal:
                    spawnRot = Quaternion.LookRotation(-collision.GetContact(0).normal);
                    break;
                case Rot.TowardsHit:
                    spawnRot = Quaternion.LookRotation(collision.transform.position - transform.position);
                    break;
                case Rot.Upwards:
                    spawnRot = Quaternion.LookRotation(Vector3.up);
                    break;
                case Rot.Default:
                    spawnRot = transform.rotation;
                    break;
            }
            
            var spawnPos = Vector3.zero;
            switch (pos)
            {
                case Pos.TransformPos:
                    spawnPos = transform.position;
                    break;
                case Pos.ContactPoint:
                    spawnPos = collision.GetContact(0).point;
                    break;
            }
            
            TeamHolder.AddTeamHolder(Instantiate(objectToSpawn, spawnPos, spawnRot), Unit, RootTeamHolder);
            spawnEvent.Invoke();
            Counter = 0f;
        }

        public void Toggle(bool toggle)
        {
            toggled = toggle;
        }
        
        public enum CollisionTarget
        {
            All = 0,
            Static = 1,
            Units = 2,
            Rigidbodies = 3,
            EnemyUnits = 4
        }
        
        public enum Rot
        {
            TowardsHit,
            Normal,
            InverseNormal,
            Upwards,
            Default
        }

        public enum Pos
        {
            TransformPos,
            ContactPoint
        }
        
        private float Counter;
        private Unit Unit;
        private Rigidbody Rig;
        private TeamHolder RootTeamHolder;

        public bool toggled = true;
        
        public CollisionTarget collisionTarget;
        public Rot rot;
        public Pos pos;

        public GameObject objectToSpawn;

        public float impactMultiplier;
        public float cooldown = 0.1f;
        public bool startOnCooldown;

        public UnityEvent spawnEvent;
    }
}