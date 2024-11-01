using System.Linq;
using Landfall.TABS.GameState;
using Photon.Bolt;
using UnityEngine;

namespace TGCore.Library
{
	public class FloatyPhysics : MonoBehaviour
	{
		private void Start()
		{
			Data = transform.root.GetComponentInChildren<DataHandler>();
			
			if (Data.footRight) RightFootRig = Data.footRight.GetComponent<Rigidbody>();
			if (Data.footLeft) LeftFootRig = Data.footLeft.GetComponent<Rigidbody>();
			if (Data.head) HeadRig = Data.head.GetComponent<Rigidbody>();
			
			MGameStateManager = ServiceLocator.GetService<GameStateManager>();

			Distance = Vector3.Distance(Data.hip.position,
				new Vector3(Data.hip.position.x,
					Data.footLeft.GetComponentsInChildren<Collider>()[0].bounds.min.y,
					Data.hip.position.z)) * distanceMultiplier;

			flightForce *= Data.unit.unitBlueprint.sizeMultiplier * Data.unit.unitBlueprint.sizeMultiplier *
			               (Data.unit.unitBlueprint.sizeMultiplier >= 1.4f
				               ? Data.unit.unitBlueprint.sizeMultiplier * Data.unit.unitBlueprint.sizeMultiplier
				               : 1f);
			if (Data.unit.gameObject.name.Contains("Blackbeard")) flightForce *= 2f * 2f * 2f;
			
			if (transform.root.GetComponent<Mount>() && transform.root.GetComponentInChildren<StandingHandler>())
				transform.root.GetComponentInChildren<StandingHandler>().enabled = true;
			else if (transform.root.GetComponentInChildren<StandingHandler>())
				transform.root.GetComponentInChildren<StandingHandler>().enabled = false;
		}

		private void FixedUpdate()
		{
			if (!active || (!Data.legLeft.gameObject.activeSelf && !Data.legRight.gameObject.activeSelf)) return;

			if (!HasKilledVelocity && Data.Dead)
			{
				HasKilledVelocity = true;
				foreach (var rig in Data.allRigs.AllRigs.Where(x => x != null)) rig.velocity += Vector3.down * rig.velocity.y;
			}

			if (!HasHalvedForce && ((!Data.legLeft.gameObject.activeSelf && Data.legRight.gameObject.activeSelf) ||
			                        (Data.legLeft.gameObject.activeSelf && !Data.legRight.gameObject.activeSelf)))
			{
				HasHalvedForce = true;
				flightForce *= 0.3f;
			}

			Physics.Raycast(new Ray(transform.position, Vector3.down), out var hitInfo, Distance, groundMask);
			if (hitInfo.transform)
			{
				if (HeadRig)
				{
					HeadRig.AddForce(Vector3.up * (flightForce * headForceMultiplier * flightCurve.Evaluate(hitInfo.distance) * Data.ragdollControl), ForceMode.Acceleration);
				}
				Data.mainRig.AddForce(Vector3.up * (flightForce * flightCurve.Evaluate(hitInfo.distance) * Data.ragdollControl), ForceMode.Acceleration);
				if (RightFootRig)
				{
					RightFootRig.AddForce(Vector3.up * (flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(hitInfo.distance) * Data.ragdollControl), ForceMode.Acceleration);
				}
				if (RightFootRig)
				{
					LeftFootRig.AddForce(Vector3.up * (flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(hitInfo.distance) * Data.ragdollControl), ForceMode.Acceleration);
				}
				
				Data.TouchGround(hitInfo.point, hitInfo.normal);
			}
		}

		public void EnableHover()
		{
			active = true;
		}

		public void DisableHover()
		{
			active = false;
		}
		
		private GameStateManager MGameStateManager;
		private DataHandler Data;
		private Rigidbody RightFootRig;
		private Rigidbody LeftFootRig;
		private Rigidbody HeadRig;
		private float Distance;
		private bool HasKilledVelocity;
		private bool HasHalvedForce;
		
		public LayerMask groundMask;

		public AnimationCurve flightCurve;

		public float flightForce = 150f;

		public float legForceMultiplier;

		public float headForceMultiplier;

		public bool active = true;

		public float distanceMultiplier = 1f;
	}
}
