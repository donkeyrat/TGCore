using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
	public class PhysicsFollowTarget: MonoBehaviour
	{
		public void Start()
		{
			Rig = GetComponent<Rigidbody>();
			if (playOnStart)
			{
				Go();
			}
		}

		public void Go()
		{
			if (Done || !target) return;
			Done = true;
			
			var targetRig = target.GetComponent<Rigidbody>();
			
			if (targetRig && useCenterOfMass) transform.position = targetRig.worldCenterOfMass;
			else transform.position = target.position;
			
			if (setRotation) transform.rotation = target.rotation;
		}

		private void FixedUpdate()
		{
			if (target)
			{
				Rig.AddForce((target.TransformPoint(offset) - transform.position) * force, ForceMode.Acceleration);
				Rig.velocity *= drag;
				Rig.angularVelocity *= drag;
				Rig.AddTorque(Vector3.Cross(transform.forward, target.forward).normalized * (Vector3.Angle(transform.forward, target.forward) * angularForce), ForceMode.Acceleration);
				Rig.AddTorque(Vector3.Cross(transform.up, Vector3.up).normalized * (Vector3.Angle(transform.up, Vector3.up) * angularForce * 0.2f), ForceMode.Acceleration);
			}
		}
		
		private Rigidbody Rig;
		private bool Done;
		
		[Header("Target Settings")]
		
		public Transform target;
		public Vector3 offset;
		
		public bool playOnStart;
		public bool setRotation;
		public bool useCenterOfMass;
		
		[Header("Force Settings")]
		
		public float force;
		public float angularForce;
		public float drag = 0.8f;
	}
}
