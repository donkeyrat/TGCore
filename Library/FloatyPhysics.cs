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
			data = transform.root.GetComponentInChildren<DataHandler>();
			
			if (data.footRight) rightFootRig = data.footRight.GetComponent<Rigidbody>();
			if (data.footLeft) leftFootRig = data.footLeft.GetComponent<Rigidbody>();
			if (data.head) headRig = data.head.GetComponent<Rigidbody>();
			
			m_gameStateManager = ServiceLocator.GetService<GameStateManager>();

			distance = Vector3.Distance(data.hip.position,
				new Vector3(data.hip.position.x,
					data.footLeft.GetComponentsInChildren<Collider>()[0].bounds.min.y,
					data.hip.position.z)) * distanceMultiplier;

			flightForce *= data.unit.unitBlueprint.sizeMultiplier * data.unit.unitBlueprint.sizeMultiplier *
			               (data.unit.unitBlueprint.sizeMultiplier >= 1.4f
				               ? data.unit.unitBlueprint.sizeMultiplier * data.unit.unitBlueprint.sizeMultiplier
				               : 1f);
			if (data.unit.gameObject.name.Contains("Blackbeard")) flightForce *= 2f * 2f * 2f;
			
			if (transform.root.GetComponent<Mount>() && transform.root.GetComponentInChildren<StandingHandler>()) transform.root.GetComponentInChildren<StandingHandler>().enabled = true;
			else if (transform.root.GetComponentInChildren<StandingHandler>()) transform.root.GetComponentInChildren<StandingHandler>().enabled = false;
		}

		private void FixedUpdate()
		{
			if (!active || (!data.legLeft.gameObject.activeSelf && !data.legRight.gameObject.activeSelf)) return;

			if (!hasKilledVelocity && data.Dead)
			{
				hasKilledVelocity = true;
				foreach (var rig in data.allRigs.AllRigs.Where(x => x != null)) rig.velocity += Vector3.down * rig.velocity.y;
			}

			if (!hasHalvedForce && ((!data.legLeft.gameObject.activeSelf && data.legRight.gameObject.activeSelf) ||
			                        (data.legLeft.gameObject.activeSelf && !data.legRight.gameObject.activeSelf)))
			{
				hasHalvedForce = true;
				flightForce *= 0.3f;
			}

			Physics.Raycast(new Ray(transform.position, Vector3.down), out var hitInfo, distance, groundMask);
			if (hitInfo.transform)
			{
				if (headRig)
				{
					headRig.AddForce(Vector3.up * (flightForce * headForceMultiplier * flightCurve.Evaluate(hitInfo.distance) * data.ragdollControl), ForceMode.Acceleration);
				}
				data.mainRig.AddForce(Vector3.up * (flightForce * flightCurve.Evaluate(hitInfo.distance) * data.ragdollControl), ForceMode.Acceleration);
				if (rightFootRig)
				{
					rightFootRig.AddForce(Vector3.up * (flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(hitInfo.distance) * data.ragdollControl), ForceMode.Acceleration);
				}
				if (rightFootRig)
				{
					leftFootRig.AddForce(Vector3.up * (flightForce * legForceMultiplier * 0.5f * flightCurve.Evaluate(hitInfo.distance) * data.ragdollControl), ForceMode.Acceleration);
				}
				
				data.TouchGround(hitInfo.point, hitInfo.normal);
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
		
		private GameStateManager m_gameStateManager;
		private DataHandler data;
		private Rigidbody rightFootRig;
		private Rigidbody leftFootRig;
		private Rigidbody headRig;
		private float distance;
		private bool hasKilledVelocity;
		private bool hasHalvedForce;
		
		public LayerMask groundMask;

		public AnimationCurve flightCurve;

		public float flightForce = 150f;

		public float legForceMultiplier;

		public float headForceMultiplier;

		public bool active = true;

		public float distanceMultiplier = 1f;
	}
}
