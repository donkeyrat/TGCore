using UnityEngine;

namespace TGCore.Library
{
    public class LockUpRotation : MonoBehaviour
    {
        private void Update()
        {
            transform.up = Vector3.up;
        }
    }
}