using RootMotion.FinalIK;
using UnityEngine;

namespace TGCore.Library
{
    public class IKLeg : MonoBehaviour
    {
        private void Update()
        {
            if (!stepping)
            {
                counter += Time.deltaTime;
            }
        }
        
        [HideInInspector]
        public bool stepping;
        [HideInInspector]
        public float counter;
        
        public float cooldown;
        
        public LimbIK leg;
    }
}