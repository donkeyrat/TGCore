using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class FollowUnitPart : MonoBehaviour
    {
        private void Start()
        {
            OwnData = transform.root.GetComponent<Unit>().data;
            
            switch (targetPart)
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
                case BodyPart.LegLeft:
                    ChosenPart = OwnData.legLeft;
                    break;
                case BodyPart.LegRight:
                    ChosenPart = OwnData.legRight;
                    break;
                case BodyPart.ArmLeft:
                    ChosenPart = OwnData.leftArm;
                    break;
                case BodyPart.ArmRight:
                    ChosenPart = OwnData.rightArm;
                    break;
                case BodyPart.KneeLeft:
                    ChosenPart = OwnData.footLeft;
                    break;
                case BodyPart.KneeRight:
                    ChosenPart = OwnData.footRight;
                    break;
                case BodyPart.ElbowLeft:
                    ChosenPart = OwnData.leftHand;
                    break;
                case BodyPart.ElbowRight:
                    ChosenPart = OwnData.rightHand;
                    break;
                default:
                    ChosenPart = OwnData.torso;
                    break;
            }
        }

        private void Update()
        {
            if (ChosenPart) 
            {
                transform.position = ChosenPart.position;
                transform.rotation = ChosenPart.rotation;
            }
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

        public BodyPart targetPart;
    }
}
