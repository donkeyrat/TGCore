using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Laser : MonoBehaviour
    {
        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        public void Activate(float speed = 1f)
        {
            StartCoroutine(Animate(true, speed));
        }

        public void Deactivate(float speed = 1f)
        {
            StartCoroutine(Animate(false, speed));
        }

        private IEnumerator Animate(bool activating, float speed)
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                
                line.widthMultiplier = Mathf.Lerp(activating ? 0f : scaleMultiplier, activating ? scaleMultiplier : 0f, Mathf.Clamp(t, 0f, 1f));
                yield return null;
            }
        }


        public void Update()
        {
            line.SetPosition(0, p2.transform.position);
            if (Physics.Raycast(new Ray(p2.transform.position, p2.transform.forward), out var hit, maxDistance, layer))
            {
                if (hit.collider)
                {
                    line.SetPosition(1, hit.point);
                    p1.transform.position = hit.point;
                    hitEvent.Invoke();
                }
            }
            else
            {
                var newPos = p2.transform.forward * maxDistance;
                line.SetPosition(1, newPos);
                p1.transform.position = newPos;
            }
        }

        private LineRenderer line;
    
        [Header("Line Settings")]
    
        public GameObject p1;
        public GameObject p2;

        public float maxDistance = 10f;
    
        [Header("Animation Settings")]

        public float scaleMultiplier = 1f;

        [Header("Hit Settings")]
    
        public UnityEvent hitEvent = new UnityEvent();

        public LayerMask layer;
    }
}