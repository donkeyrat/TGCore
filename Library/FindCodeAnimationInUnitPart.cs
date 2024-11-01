using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class FindCodeAnimationInUnitPart : MonoBehaviour
    {
        private void Start()
        {
            OwnData = transform.root.GetComponent<Unit>().data;
            
            switch (bodyPart)
            {
                case BodyPart.Head:
                    ChosenPart = OwnData.head;
                    break;
                case BodyPart.Torso:
                    ChosenPart = OwnData.torso;
                    break;
                case BodyPart.Hip:
                    ChosenPart = OwnData.hip.transform;
                    break;
                case BodyPart.ArmLeft:
                    ChosenPart = OwnData.leftArm;
                    break;
                case BodyPart.ArmRight:
                    ChosenPart = OwnData.rightArm;
                    break;
                case BodyPart.ElbowLeft:
                    ChosenPart = OwnData.leftHand;
                    break;
                case BodyPart.ElbowRight:
                    ChosenPart = OwnData.rightHand;
                    break;
                case BodyPart.LegLeft:
                    ChosenPart = OwnData.legLeft;
                    break;
                case BodyPart.LegRight:
                    ChosenPart = OwnData.legRight;
                    break;
                case BodyPart.KneeLeft:
                    ChosenPart = OwnData.footLeft;
                    break;
                case BodyPart.KneeRight:
                    ChosenPart = OwnData.footRight;
                    break;
            }
        }

        public void Go()
        {
            if (!ChosenPart) return;
            
            var animation = ChosenPart.GetComponentInChildren<CodeAnimation>();
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
        
        public void GoUnityAnimation()
        {
            if (!ChosenPart) return;
            
            var animation = ChosenPart.GetComponentInChildren<Animator>();
            animation.Play(animationName);
        }

        private DataHandler OwnData;

        private Transform ChosenPart;
        
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
        
        [Header("Code Animation")]
        public AnimationType animationToPlay;
        
        [Header("Unity Animation")]
        public string animationName;
        
    }
}