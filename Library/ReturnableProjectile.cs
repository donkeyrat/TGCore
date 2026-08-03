using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class ReturnableProjectile : MonoBehaviour
    {
        private void Start()
        {
            var teamHolder = GetComponent<TeamHolder>();
            if (!teamHolder)
            {
                return;
            }
            
            Weapon = teamHolder.spawnerWeapon;
            Stick = GetComponent<ProjectileStick>();
            ReturnObject = Weapon.transform.FindChildRecursive(objectToReturnTo);
        }
        
        public void Return()
        {
            if (!Weapon) return;
            StartCoroutine(DoReturn());
        }

        private IEnumerator DoReturn()
        {
            if (Stick) Destroy(Stick);
            
            var t = 0f;
            var beginPosition = transform.position;
            var beginRotation = transform.rotation;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(beginPosition, ReturnObject.TransformPoint(offset), t);
                transform.rotation = Quaternion.Lerp(beginRotation, ReturnObject.rotation, t);
                
                t += Time.deltaTime * speed;
                yield return null;
            }

            var returnEvent = Weapon.GetComponent<ReturnableProjectileEvent>();
            if (returnEvent) returnEvent.Go();
            Destroy(gameObject);
        }

        private Transform ReturnObject;
        private GameObject Weapon;
        private ProjectileStick Stick;

        public string objectToReturnTo;
        public Vector3 offset;

        public float speed = 1f;
    }
}