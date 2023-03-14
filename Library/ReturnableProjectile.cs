using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class ReturnableProjectile : MonoBehaviour
    {
        private void Start()
        {
            weapon = GetComponent<TeamHolder>().spawnerWeapon;
            stick = GetComponent<ProjectileStick>();
            returnObject = weapon.transform.FindChildRecursive(objectToReturnTo);
        }
        
        public void Return()
        {
            StartCoroutine(DoReturn());
        }

        private IEnumerator DoReturn()
        {
            if (stick) Destroy(stick);
            
            var t = 0f;
            var beginPosition = transform.position;
            var beginRotation = transform.rotation;
            var endPosition = returnObject.position + returnObject.TransformPoint(offset);
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(beginPosition, endPosition, t);
                transform.rotation = Quaternion.Lerp(beginRotation, returnObject.rotation, t);
                
                t += Time.deltaTime * speed;
                yield return null;
            }

            var delay = weapon.GetComponent<DelayEvent>();
            if (delay) delay.Go();
            Destroy(gameObject);
        }

        private Transform returnObject;

        private GameObject weapon;

        private ProjectileStick stick;

        public string objectToReturnTo;
        public Vector3 offset;

        public float speed = 1f;
    }
}