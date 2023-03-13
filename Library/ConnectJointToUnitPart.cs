using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
	public class ConnectJointToUnitPart : MonoBehaviour
	{
		public void Go()
		{
			if (done)
			{
				return;
			}
			done = true;

			ownData = transform.root.GetComponent<Unit>().data;
			
			Transform chosenPart;
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
				default:
					chosenPart = ownData.torso;
					break;
			}
	
			var joint = GetComponent<ConfigurableJoint>();
			if (chosenPart && joint)
			{
				joint.connectedBody = chosenPart.GetComponent<Rigidbody>();
			}
		}
		
		private bool done;
	
		private DataHandler ownData;
		
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