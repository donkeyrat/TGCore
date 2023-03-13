using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class AddDamageToUnitPart : MonoBehaviour
	{
		private void Start()
		{
			ownUnit = transform.root.GetComponent<Unit>();

			GameObject chosenPart;
			switch (bodyTarget)
			{
				case BodyTarget.Head:
					chosenPart = ownUnit.data.head.gameObject;
					break;
				case BodyTarget.LeftFoot:
					chosenPart = ownUnit.data.footLeft.gameObject;
					break;
				case BodyTarget.RightFoot:
					chosenPart = ownUnit.data.footRight.gameObject;
					break;
				case BodyTarget.LeftHand:
					chosenPart = ownUnit.data.leftHand.gameObject;
					break;
				case BodyTarget.RightHand:
					chosenPart = ownUnit.data.rightHand.gameObject;
					break;
				default:
					chosenPart = ownUnit.data.mainRig.gameObject;
					break;
			}
			
			var ownDamage = GetComponent<CollisionWeapon>();
			newDamage = chosenPart.AddComponent<CollisionWeapon>();
			newDamage.damage = ownDamage.damage;
			newDamage.impactMultiplier = ownDamage.impactMultiplier;
			newDamage.onImpactForce = ownDamage.onImpactForce;
			newDamage.massCap = ownDamage.massCap;
			newDamage.ignoreTeamMates = ownDamage.ignoreTeamMates;
			newDamage.staticDamageValue = ownDamage.staticDamageValue;
			newDamage.onlyOncePerData = ownDamage.onlyOncePerData;
			newDamage.cooldown = ownDamage.cooldown;
			newDamage.onlyCollideWithRigs = true;
			newDamage.dealDamageEvent = ownDamage.dealDamageEvent;
			Destroy(ownDamage);
			
			var ownSound = GetComponent<CollisionSound>();
			if (ownSound)
			{
				var newSound = chosenPart.AddComponent<CollisionSound>();
				newSound.SoundEffectRef = ownSound.SoundEffectRef;
				newSound.multiplier = ownSound.multiplier;
				Destroy(ownSound);
			}

			var ownEffect = GetComponent<MeleeWeaponAddEffect>();
			if (ownEffect)
			{
				var newEffect = chosenPart.AddComponent<MeleeWeaponAddEffect>();
				newEffect.EffectPrefab = ownEffect.EffectPrefab;
				newEffect.ignoreTeamMates = ownEffect.ignoreTeamMates;
				Destroy(ownEffect);
			}
		}

		public void RemoveDamage()
		{
			if (newDamage) Destroy(newDamage);
		}
	
		private void OnDestroy()
		{
			RemoveDamage();
		}
		
		private CollisionWeapon newDamage;

		private Unit ownUnit;
		
		public enum BodyTarget
		{
			Head,
			RightFoot,
			LeftFoot,
			RightHand,
			LeftHand
		}
	
		public BodyTarget bodyTarget;
	}
}