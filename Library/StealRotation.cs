using UnityEngine;

namespace TGCore.Library
{
    public class StealRotation : MonoBehaviour
    {
        public void CopyRotation()
        {
            targetToStealRotation.rotation = targetToStealRotationFrom.rotation;
        }
        
        public Transform targetToStealRotation;
        public Transform targetToStealRotationFrom;
    }
}