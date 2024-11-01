using System.Collections;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class RiseOnSpawn : MonoBehaviour
    {
        private void Start()
        {
            OwnData = GetComponent<Unit>().data;
            StartCoroutine(DoRise());
        }

        private IEnumerator DoRise()
        {
            OwnData.mainRig.isKinematic = true;
            yield return new WaitForSeconds(startDelay);
            
            var t = 0f;
            while (t < time)
            {
                foreach (var rig in OwnData.allRigs.AllRigs)
                {
                    if (setRigsKinematic && (setArmsKinematic || (rig.transform != OwnData.leftArm && rig.transform != OwnData.rightArm && rig.transform != OwnData.leftHand && rig.transform != OwnData.rightHand)))
                    {
                        rig.isKinematic = true;
                    }
                }
                
                transform.position += Vector3.up * (Mathf.Clamp(t * 0.1f, 0f, 1f) * Time.deltaTime * moveMultiplier);
                
                t += Time.deltaTime;
                yield return null;
            }

            foreach (var rig in OwnData.allRigs.AllRigs)
            {
                rig.isKinematic = false;
            }
        }
        
        private DataHandler OwnData;

        public float startDelay;

        public float time = 2f;

        public float moveMultiplier = 0.6f;

        public bool setRigsKinematic;
        
        public bool setArmsKinematic;
    }
}