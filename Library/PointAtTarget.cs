using UnityEngine;

namespace TGCore.Library
{
    public class PointAtTarget : MonoBehaviour 
    {
        private void Update() 
        {
            if (target) transform.LookAt(target);
            else if (destroyIfTargetNull) Destroy(gameObject);
            else Destroy(this);
        }

        public Transform target;

        public bool destroyIfTargetNull;
    }
}