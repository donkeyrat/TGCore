using UnityEngine;

namespace TGCore.Library 
{
    public class FreezeSelf : MonoBehaviour 
    {
        private void Start()
        {
            AllRigs = transform.root.GetComponentsInChildren<Rigidbody>();
        }
    
        public void Freeze() 
        {
            foreach (var rig in AllRigs) rig.isKinematic = true;
        }
    
        public void UnFreeze() 
        {
            foreach (var rig in AllRigs) rig.isKinematic = false;
        }

        private Rigidbody[] AllRigs;
    }
}