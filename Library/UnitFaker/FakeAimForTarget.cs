using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeAimForTarget : MonoBehaviour
    {
        private void Start()
        {
            OwnUnit = transform.root.GetComponent<FakeUnit>();
            StartForward = transform.parent.InverseTransformDirection(transform.forward);
        }

        private void Update()
        {
            if (OwnUnit && OwnUnit.target)
            {
                var vector = transform.parent.TransformDirection(StartForward);
                var normalized = (OwnUnit.target.data.mainRig.position - transform.position).normalized;
                var time = Vector3.Angle(normalized, vector);
                
                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward,
                    Vector3.Lerp(vector, normalized, curve.Evaluate(time)), lerpSpeed * Time.deltaTime));
            }
        }
        
        private FakeUnit OwnUnit;
        private Vector3 StartForward;
        
        public AnimationCurve curve;
        public float lerpSpeed;
    }
}