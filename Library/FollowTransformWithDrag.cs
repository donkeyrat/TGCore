using Landfall.MonoBatch;
using UnityEngine;

namespace TGCore.Library
{
    public class FollowTransformWithDrag : BatchedMonobehaviour
    {
        public override void BatchedUpdate()
        {
            if (target)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + worldOffset,  Mathf.Clamp(Time.deltaTime * positionSpeed, 0f, 1f));
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation,  Mathf.Clamp(Time.deltaTime * rotationSpeed, 0f, 1f));
            }
            else if (destroyOnTargetNull)
            {
                Destroy(gameObject);
            }
        }
        
        public Transform target;
        public Vector3 worldOffset;
        
        public float positionSpeed = 1f;
        public float rotationSpeed = 1f;

        public bool destroyOnTargetNull = true;
    }
}