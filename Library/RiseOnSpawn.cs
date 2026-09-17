using System.Collections;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class RiseOnSpawn : MonoBehaviour
    {
        private float StartCounter;
        private float Counter;
        private DataHandler OwnData;

        public float startDelay;

        public float time = 2f;

        public float moveMultiplier = 25f;

        public bool setRigsKinematic = true;
        
        public bool setArmsKinematic;
        
        private void Start()
        {
            OwnData = GetComponent<Unit>().data;
            OwnData.mainRig.isKinematic = true;
        }

        private void FixedUpdate()
        {
            StartCounter += Time.deltaTime;
            if (StartCounter < startDelay) return;

            if (Counter < time)
            {
                foreach (var rig in OwnData.allRigs.AllRigs)
                {
                    if (setRigsKinematic && (setArmsKinematic || (rig.transform != OwnData.leftArm && rig.transform != OwnData.rightArm && rig.transform != OwnData.leftHand && rig.transform != OwnData.rightHand)))
                    {
                        rig.isKinematic = true;
                    }
                }
                
                transform.position += Vector3.up * (Mathf.Clamp(Counter * 0.1f, 0f, 1f) * Time.deltaTime * moveMultiplier);
                
                Counter += Time.deltaTime;
                return;
            }

            foreach (var rig in OwnData.allRigs.AllRigs)
            {
                rig.isKinematic = false;
            }
            Destroy(this);
        }
    }
}