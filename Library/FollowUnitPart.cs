using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class FollowUnitPart : MonoBehaviour
    {
        private void Start()
        {
            ownData = transform.root.GetComponent<Unit>().data;
            
            switch (targetPart)
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
                case BodyPart.LegLeft:
                    chosenPart = ownData.legLeft;
                    break;
                case BodyPart.LegRight:
                    chosenPart = ownData.legRight;
                    break;
                case BodyPart.ArmLeft:
                    chosenPart = ownData.leftArm;
                    break;
                case BodyPart.ArmRight:
                    chosenPart = ownData.rightArm;
                    break;
                case BodyPart.KneeLeft:
                    chosenPart = ownData.footLeft;
                    break;
                case BodyPart.KneeRight:
                    chosenPart = ownData.footRight;
                    break;
                case BodyPart.ElbowLeft:
                    chosenPart = ownData.leftHand;
                    break;
                case BodyPart.ElbowRight:
                    chosenPart = ownData.rightHand;
                    break;
                default:
                    chosenPart = ownData.torso;
                    break;
            }
        }

        private void Update()
        {
            if (chosenPart) 
            {
                transform.position = chosenPart.position;
                transform.rotation = chosenPart.rotation;
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

        public BodyPart targetPart;
    }
}
