using System.Collections;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class AddRigidbodyThenSinkOnDeath : MonoBehaviour
    {
        private void Start()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
            OwnUnit.data.healthHandler.AddDieAction(Die);
        }

        public void Die()
        {
            if (HasDied) return;

            Rig = addRigidbody ? gameObject.AddComponent<Rigidbody>() : GetComponent<Rigidbody>();
            Rig.mass = mass;
            Rig.drag = drag;
            Rig.angularDrag = angularDrag;
            Rig.interpolation = interpolation;

            HasDied = true;
            
            transform.SetParent(OwnUnit.data.transform, true);

            StartCoroutine(DoSink());
        }

        private IEnumerator DoSink()
        {
            yield return new WaitForSeconds(sinkDelay);
            var t = 0f;
            while (t < 3f)
            {
                Rig.drag *= 2f;
                if (Rig.velocity.magnitude < 1f) t += Time.deltaTime;

                yield return null;
            }

            Rig.isKinematic = true;

            t = 0f;
            while (t < 30f)
            {
                transform.position += Vector3.down * Mathf.Clamp(t * 0.1f, 0f, 1f) * Time.deltaTime * sinkMultiplier;
                t += Time.deltaTime;
                if (scaleAfterDelay && t > scaleDelay)
                {
                    ScaleMultiplier += Time.deltaTime * 0.35f;
                    transform.localScale *= Mathf.Lerp(1f, 0f, ScaleMultiplier);
                }

                yield return null;
            }
        }
        
        private Unit OwnUnit;
        private bool HasDied;
        private Rigidbody Rig;

        [Header("Rigidbody Settings")] 
        
        public bool addRigidbody;
        public float mass = 200f;
        public float drag;
        public float angularDrag;
        public RigidbodyInterpolation interpolation = RigidbodyInterpolation.Interpolate;

        [Header("Sink Settings")] 
        
        public float sinkDelay = 2f;
        public float sinkMultiplier = 0.3f;
        
        public bool scaleAfterDelay = true;
        public float scaleDelay = 15f;
        private float ScaleMultiplier;
    }
}