using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{ 
    public class MeleeWeaponSpawnParented : CollisionWeaponEffect
    {
        private void Update()
        {
            counter += Time.deltaTime;
        }
    
        public override void DoEffect(Transform hitTransform, Collision collision)
        {
            if (counter >= cooldown)
            {
                counter = 0f;
                var rotation = Quaternion.identity;
                switch (rot)
                {
                    case Rot.Normal:
                        rotation = Quaternion.LookRotation(collision.contacts[0].normal);
                        break;
                    case Rot.InverseNormal:
                        rotation = Quaternion.LookRotation(-collision.contacts[0].normal);
                        break;
                    case Rot.TowardsHit:
                        rotation = Quaternion.LookRotation(hitTransform.position - transform.position);
                        break;
                }

                var position = Vector3.zero;
                if (pos == Pos.TransformPos) position = transform.position;
                else if (pos == Pos.ContactPoint) position = collision.contacts[0].point;
                
                TeamHolder.AddTeamHolder(Instantiate(objectToSpawn, position, rotation, transform), base.gameObject);
                
                spawnEvent.Invoke();
            }
        }
        
        private float counter;
        
        public enum Rot
        {
            TowardsHit,
            Normal,
            InverseNormal
        }
    
        public enum Pos
        {
            ContactPoint,
            TransformPos
        }
    
        public Rot rot;
    
        public Pos pos;
    
        public GameObject objectToSpawn;
    
        public UnityEvent spawnEvent;
        
        public float cooldown = 0.1f;
    }
}