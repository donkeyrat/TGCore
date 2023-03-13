using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class FindCodeAnimationInUnitPart : MonoBehaviour
    {
        private void Start()
        {
            ownData = transform.root.GetComponent<Unit>().data;
            
            switch (bodyPart)
            {
                case BodyPart.Head:
                    chosenPart = ownData.head;
                    break;
                case BodyPart.Torso:
                    chosenPart = ownData.torso;
                    break;
                case BodyPart.Hip:
                    chosenPart = ownData.hip.transform;
                    break;
                case BodyPart.ArmLeft:
                    chosenPart = ownData.leftArm;
                    break;
                case BodyPart.ArmRight:
                    chosenPart = ownData.rightArm;
                    break;
                case BodyPart.ElbowLeft:
                    chosenPart = ownData.leftHand;
                    break;
                case BodyPart.ElbowRight:
                    chosenPart = ownData.rightHand;
                    break;
                case BodyPart.LegLeft:
                    chosenPart = ownData.legLeft;
                    break;
                case BodyPart.LegRight:
                    chosenPart = ownData.legRight;
                    break;
                case BodyPart.KneeLeft:
                    chosenPart = ownData.footLeft;
                    break;
                case BodyPart.KneeRight:
                    chosenPart = ownData.footRight;
                    break;
            }
        }

        public void Go()
        {
            if (!chosenPart) return;
            
            var animation = chosenPart.GetComponentInChildren<CodeAnimation>();
            switch (animationToPlay)
            {
                case AnimationType.In:
                    animation.PlayIn();
                    break;
                case AnimationType.Out:
                    animation.PlayOut();
                    break;
                case AnimationType.Boop:
                    animation.PlayBoop();
                    break;
                default:
                    animation.PlayIn();
                    break;
            }
        }

        private DataHandler ownData;

        private Transform chosenPart;
        
        public enum BodyPart
        {
            Head,
            Torso,
            Hip,
            ArmLeft,
            ArmRight,
            ElbowLeft,
            ElbowRight,
            LegLeft,
            LegRight,
            KneeLeft,
            KneeRight
        }

        public enum AnimationType
        {
            In,
            Out,
            Boop
        }

        public BodyPart bodyPart;
        
        public AnimationType animationToPlay;
    }
}