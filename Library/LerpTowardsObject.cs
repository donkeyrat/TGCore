using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class LerpTowardsObject : MonoBehaviour
    {
        public void LerpTowards(Transform target)
        {
            StartCoroutine(DoLerp(target, Vector3.zero));
        }

        public void LerpTowardsGround()
        {
            if (Physics.Raycast(lerpTarget.position, Vector3.down, out var hit, distanceToGround, groundMask))
            {
                StartCoroutine(DoLerp(null, hit.point));
            }
        }

        private IEnumerator DoLerp(Transform target, Vector3 point)
        {
            var t = 0f;
            var startPos = lerpTarget.position;
            while (t < 1f)
            {
                lerpTarget.position = Vector3.Lerp(startPos, target ? target.position : point, Mathf.Clamp(t, 0f, 1f));
                
                t += Time.deltaTime * lerpSpeed;
                yield return null;
            }
        }

        [Header("Lerp Settings")]
        
        public Transform lerpTarget;
        public float lerpSpeed;

        [Header("Ground Settings")] 
        
        public float distanceToGround;
        public LayerMask groundMask;
    }
}