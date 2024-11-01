using System.Collections;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeGroundCaster : MonoBehaviour
    {
        private void Start()
        {
            OwnUnit = transform.root.GetComponent<FakeUnit>();
        }
        
        private void Update()
        {
            if (Raycast(out var hit))
            {
                OwnUnit.onGround = true;
                OwnUnit.groundedTime += Time.deltaTime;
                OwnUnit.distanceToGround = Vector3.Distance(OwnUnit.core.position, hit.point);
                OwnUnit.upHillVector = Vector3.Cross(Vector3.Cross(hit.normal, Vector3.up), hit.normal);
                
                if (OwnUnit.groundedTime > 0.05f)
                {
                    OwnUnit.core.AddTorque(groundNormalTorque * Vector3.Angle(OwnUnit.core.transform.up, hit.normal) * Vector3.Cross(OwnUnit.core.transform.up, hit.normal).normalized, ForceMode.Acceleration);
                }
            }
            else if (!UnGrounding)
            {
                StartCoroutine(DelayUnGround());
            }
        }

        private bool Raycast(out RaycastHit rayHit)
        {
            var cast = Physics.Raycast(new Ray(OwnUnit.core.position, Vector3.down), out var hit, height, groundMask);
            rayHit = hit;
            return cast;
        }

        private IEnumerator DelayUnGround()
        {
            UnGrounding = true;
            yield return new WaitForSeconds(OwnUnit.groundLeeway);
            UnGrounding = false;
            
            if (!Raycast(out _))
            {
                OwnUnit.onGround = false;
                OwnUnit.groundedTime = 0f;
            }
        }

        private FakeUnit OwnUnit;
        private bool UnGrounding;

        public float height = 1f;
        public float groundNormalTorque = 1f;
        
        public LayerMask groundMask;
    }
}