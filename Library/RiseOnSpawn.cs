using System.Collections;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class RiseOnSpawn : MonoBehaviour
    {
        private void Start()
        {
            ownData = GetComponent<Unit>().data;
            StartCoroutine(DoRise());
        }

        private IEnumerator DoRise()
        {
            ownData.mainRig.isKinematic = true;
            yield return new WaitForSeconds(startDelay);
            
            var t = 0f;
            while (t < time)
            {
                foreach (var rig in ownData.allRigs.AllRigs)
                {
                    if (setRigsKinematic && (setArmsKinematic || (rig.transform != ownData.leftArm && rig.transform != ownData.rightArm && rig.transform != ownData.leftHand && rig.transform != ownData.rightHand)))
                    {
                        rig.isKinematic = true;
                    }
                }
                
                transform.position += Vector3.up * (Mathf.Clamp(t * 0.1f, 0f, 1f) * Time.deltaTime * moveMultiplier);
                
                t += Time.deltaTime;
                yield return null;
            }

            foreach (var rig in ownData.allRigs.AllRigs)
            {
                rig.isKinematic = false;
            }
        }
        
        private DataHandler ownData;

        public float startDelay;

        public float time = 2f;

        public float moveMultiplier = 0.6f;

        public bool setRigsKinematic;
        
        public bool setArmsKinematic;
    }
}