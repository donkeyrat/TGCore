using UnityEngine;

namespace TGCore.Library 
{
    public class FreezeSelf : MonoBehaviour 
    {
        private void Start()
        {
            allRigs = transform.root.GetComponentsInChildren<Rigidbody>();
        }
    
        public void Freeze() 
        {
            foreach (var rig in allRigs) rig.isKinematic = true;
        }
    
        public void UnFreeze() 
        {
            foreach (var rig in allRigs) rig.isKinematic = false;
        }

        private Rigidbody[] allRigs;
    }
}