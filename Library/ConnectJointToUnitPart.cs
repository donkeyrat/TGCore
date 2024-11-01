using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
	public class ConnectJointToUnitPart : MonoBehaviour
	{
		public void Go()
		{
			if (Done)
			{
				return;
			}
			Done = true;

			OwnData = transform.root.GetComponent<Unit>().data;
			
			Transform chosenPart;
			switch (bodyPart)
			{
				case BodyPart.Head:
					chosenPart = OwnData.head;
					break;
				case BodyPart.Torso:
					chosenPart = OwnData.torso;
					break;
				case BodyPart.Hip:
					chosenPart = OwnData.hip.transform;
					break;
				case BodyPart.ArmLeft:
					chosenPart = OwnData.leftArm;
					break;
				case BodyPart.ArmRight:
					chosenPart = OwnData.rightArm;
					break;
				case BodyPart.ElbowLeft:
					chosenPart = OwnData.leftHand;
					break;
				case BodyPart.ElbowRight:
					chosenPart = OwnData.rightHand;
					break;
				case BodyPart.LegLeft:
					chosenPart = OwnData.legLeft;
					break;
				case BodyPart.LegRight:
					chosenPart = OwnData.legRight;
					break;
				case BodyPart.KneeLeft:
					chosenPart = OwnData.footLeft;
					break;
				case BodyPart.KneeRight:
					chosenPart = OwnData.footRight;
					break;
				default:
					chosenPart = OwnData.torso;
					break;
			}
	
			var joint = GetComponent<ConfigurableJoint>();
			if (chosenPart && joint)
			{
				joint.connectedBody = chosenPart.GetComponent<Rigidbody>();
			}
		}
		
		private bool Done;
	
		private DataHandler OwnData;
		
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
	
		public BodyPart bodyPart;
	}

}