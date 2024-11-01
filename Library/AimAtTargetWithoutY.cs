using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class AimForUnitTarget : MonoBehaviour
    {
        private void Start()
        {
            Unit = transform.root.GetComponent<Unit>();
            StartForward = transform.parent.InverseTransformDirection(transform.forward);
        }

        private void Update()
        {
            if (Unit && Unit.data && Unit.data.targetData && Unit.data.targetData.mainRig)
            {
                var vector = transform.parent.TransformDirection(StartForward);
                var normalized = (Unit.data.targetData.mainRig.position - transform.position).normalized;
                normalized.y = 0f;
                var time = Vector3.Angle(normalized, vector);
                
                
                transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward,
                    Vector3.Lerp(vector, normalized, lerpCurve.Evaluate(time)), lerpSpeed * Time.deltaTime));
            }
        }
        
        private Unit Unit;
        private Vector3 StartForward;
        
        public AnimationCurve lerpCurve;
        public float lerpSpeed;
    }
}