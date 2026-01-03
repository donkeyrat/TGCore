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
            var foundAnimation = transform.root.GetComponentsInChildren<CodeAnimation>()
                .First(x => x.gameObject.name == objName);
            if (foundAnimation)
            {
                switch (animationType)
                {
                    case CodeAnimationInstance.AnimationUse.In:
                        foundAnimation.PlayIn();
                        break;
                    case CodeAnimationInstance.AnimationUse.Out:
                        foundAnimation.PlayOut();
                        break;
                    case CodeAnimationInstance.AnimationUse.Boop:
                        foundAnimation.PlayBoop();
                        break;
                }
            }
        }
    }
}