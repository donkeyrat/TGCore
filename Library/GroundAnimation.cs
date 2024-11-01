using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class GroundAnimation : MonoBehaviour
    {
        private void Update()
        {
            if (maintainGroundPosOutsideAnimation && Physics.Raycast(objectToRaycastFrom.position, Vector3.down, out var raycastHit, maxDistance, groundMask))
            {
                var pos = transform.position;
                transform.position = Vector3.Lerp(pos,
                    new Vector3(pos.x, distanceAboveGround + raycastHit.point.y, pos.z), lerpSpeed * Time.deltaTime);
            }
        }
    
        public void Animate()
        {
            if (Physics.Raycast(objectToRaycastFrom.position, Vector3.down, out var raycastHit, maxDistance, groundMask))
            {
                StartCoroutine(DoAnimation(raycastHit.point.y));
            }
        }

        private IEnumerator DoAnimation(float groundHeight)
        {
            var counter = 0f;
            var animateCounter = 0f;
            var endTime = groundCurve.keys[groundCurve.keys.Length - 1].time;
            while (counter < endTime)
            {
                if (animateCounter < 1f)
                {
                    animateCounter += Time.deltaTime * animationSpeed;
                    animateCounter = Mathf.Clamp(animateCounter, 0f, 1f);
                }
            
                var pos = transform.position;
                transform.position = Vector3.Lerp(pos,
                    new Vector3(pos.x, groundCurve.Evaluate(counter) + groundHeight, pos.z), animateCounter);
                
                counter += Time.deltaTime;
                yield return null;
            }
        }
    
        public Transform objectToRaycastFrom;
        public LayerMask groundMask;
        public float maxDistance = 5f;
        public float lerpSpeed = 1f;
        public bool maintainGroundPosOutsideAnimation = true;
        public float distanceAboveGround = 3f;
    
        [Header("Animation")]
        public AnimationCurve groundCurve;
        public float animationSpeed = 2f;
    }
}