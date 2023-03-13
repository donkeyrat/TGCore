using UnityEngine;

namespace TGCore.Library 
{
    public abstract class ProjectileLauncherEffect : MonoBehaviour 
    {
        public abstract void DoEffect(Rigidbody target);
    }
}