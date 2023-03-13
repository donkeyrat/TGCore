using UnityEngine;

namespace TGCore.Library
{
    public class SetParentRoot : MonoBehaviour
    {
        private void Awake()
        {
            if (setParentOnAwake) Go();
        }
        
        private void Start()
        {
            if (setParentOnStart) Go();
        }

        public void Go()
        {
            if (resetRotation) transform.up = Vector3.up;
            transform.SetParent(transform.root);
        }

        public bool setParentOnStart = true;
        public bool setParentOnAwake;

        public bool resetRotation;
    }
}