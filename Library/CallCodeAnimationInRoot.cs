using System;
using System.Linq;
using UnityEngine;

namespace TGCore.Library
{
    public class CallCodeAnimationInRoot : MonoBehaviour
    {
        public CodeAnimationInstance.AnimationUse animationType;
    
        public void CallAnimation(string objName)
        {
            if (transform.root.GetComponentsInChildren<CodeAnimation>() == null) return;
            
            var foundAnimation = transform.root.GetComponentsInChildren<CodeAnimation>()
                .Where(x => x.gameObject.name == objName).ToArray();
            if (foundAnimation.Length > 0)
            {
                switch (animationType)
                {
                    case CodeAnimationInstance.AnimationUse.In:
                        foundAnimation[0].PlayIn();
                        break;
                    case CodeAnimationInstance.AnimationUse.Out:
                        foundAnimation[0].PlayOut();
                        break;
                    case CodeAnimationInstance.AnimationUse.Boop:
                        foundAnimation[0].PlayBoop();
                        break;
                    case CodeAnimationInstance.AnimationUse.None:
                        break;
                }
            }
        }
    }
}