using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
	public class TeleportNearby : MonoBehaviour
	{
		private Unit Unit;
		private ParticleSystem.ShapeModule Emission;
		private ParticleSystem Part;
		private List<PhysicsFollowBodyPart> Followers;

		public float distanceToRandomlyTeleport = 5f;
		public float moveDelay = 0.05f;
		public bool meshParticle = true;
		public UnityEvent prePoofEvent;
		public UnityEvent poofEvent;

		[Header("Check")] 
		public float avoidRadius = 1f;
		public LayerMask avoidMask;
		
		private void Start()
		{
			Unit = GetComponentInParent<Unit>();
			if (meshParticle)
			{
				Part = GetComponentInChildren<ParticleSystem>();
				Emission = Part.shape;
				Emission.skinnedMeshRenderer = transform.root.GetComponentInChildren<SkinnedMeshRenderer>();
			}
			Followers = transform.root.GetComponentsInChildren<PhysicsFollowBodyPart>().ToList();
		}
	
		public void DoThePoof()
		{
			StartCoroutine(DoPoof());
		}
	
		private IEnumerator DoPoof()
		{
			var mainRigPos = Unit.data.mainRig.position;
			var tryCount = 0;
			var randomPos = Vector3.zero;
			while (tryCount < 20)
			{
				randomPos = new Vector3(
					mainRigPos.x + Random.Range(-distanceToRandomlyTeleport, distanceToRandomlyTeleport),
					mainRigPos.y,
					mainRigPos.z + Random.Range(-distanceToRandomlyTeleport, distanceToRandomlyTeleport));
				
				var mapColliders = new Collider[20];
				Physics.OverlapSphereNonAlloc(randomPos, avoidRadius, mapColliders, avoidMask);
				if (mapColliders.Where(x => x).ToArray().Length > 0)
				{
					tryCount++;
					continue;
				}

				break;
			}

			if (randomPos == Vector3.zero)
			{
				yield break;
			}
			
			prePoofEvent.Invoke();
			Part?.Emit(25);
			
			yield return new WaitForSeconds(moveDelay);

			var randomDirection = randomPos - Unit.data.mainRig.position;
			for (var j = 0; j < Unit.data.transform.childCount; j++)
			{
				var child = Unit.data.transform.GetChild(j);
				child.position += randomDirection;
			}

			if (Unit.data.weaponHandler)
			{
				if (Unit.data.weaponHandler.rightWeapon != null)
					Unit.data.weaponHandler.rightWeapon.transform.position += randomDirection;

				if (Unit.data.weaponHandler.leftWeapon != null)
					Unit.data.weaponHandler.leftWeapon.transform.position += randomDirection;
			}
			foreach (var follower in Followers)
			{
				follower.transform.position += randomDirection;
			}
			
			poofEvent.Invoke();
			Part?.Play();
		}
	}
}