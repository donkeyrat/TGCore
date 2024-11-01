using System;
using Landfall.MonoBatch;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
	public class FakeStand : BatchedMonobehaviour
	{
		protected override void Start()
		{
			base.Start();
			ownUnit = GetComponent<FakeUnit>();
		}

		public override void BatchedFixedUpdate()
		{
			if (!ownUnit.onGround) return;
			
			var standingInstance = standingData.standingInstances[standingData.defaultSate];
			var num = ownUnit.upHillVector.magnitude * 0.05f * standingInstance.staticAngleLowering + standingData.baseOffset;
			
			ownUnit.core.AddForce(
				Vector3.up * standingData.standingForce *
				standingInstance.curve.Evaluate(ownUnit.distanceToGround + selfOffset + num), ForceMode.Acceleration);
		}

		public FakeUnit ownUnit;
		
		public float selfOffset;

		public StandingData standingData;
	}
}
