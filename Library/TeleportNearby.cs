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
			if (Part)
			{
				Part.Emit(25);
			}
			yield return new WaitForSeconds(moveDelay);

			var mainRigPos = Unit.data.mainRig.position;
			var randomPos = new Vector3(
				mainRigPos.x + Random.Range(-distanceToRandomlyTeleport, distanceToRandomlyTeleport),
				mainRigPos.y,
				mainRigPos.z + Random.Range(-distanceToRandomlyTeleport, distanceToRandomlyTeleport));
		
			var randomDirection = (randomPos - Unit.data.mainRig.position).normalized * ((randomPos - Unit.data.mainRig.position).magnitude + distanceToRandomlyTeleport);
			var data = transform.root.GetComponentInChildren<DataHandler>();
			for (var j = 0; j < data.transform.childCount; j++)
			{
				var child = data.transform.GetChild(j);
				child.position += randomDirection;
			}
			var component = data.GetComponent<WeaponHandler>();
			var componentInParent = component.GetComponentInParent<DataHandler>();
			if (component && !componentInParent)
			{
				if (component.rightWeapon)
				{
					component.rightWeapon.transform.position += randomDirection;
				}
				if (component.leftWeapon)
				{
					component.leftWeapon.transform.position += randomDirection;
				}
			}
			foreach (var follower in Followers)
			{
				follower.transform.position += randomDirection;
			}
			poofEvent?.Invoke();
			if (Part)
			{
				Part.Play();
			}
		}

		private Unit Unit;
		private ParticleSystem.ShapeModule Emission;
		private ParticleSystem Part;
		private List<PhysicsFollowBodyPart> Followers;

		public float distanceToRandomlyTeleport = 5f;

		public float moveDelay = 0.05f;

		public bool meshParticle = true;

		public UnityEvent poofEvent;
	}
}