using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class AddDamageToUnitPart : MonoBehaviour
	{
		private void Start()
		{
			OwnUnit = transform.root.GetComponent<Unit>();

			GameObject chosenPart;
			switch (bodyTarget)
			{
				case BodyTarget.Head:
					chosenPart = OwnUnit.data.head.gameObject;
					break;
				case BodyTarget.LeftFoot:
					chosenPart = OwnUnit.data.footLeft.gameObject;
					break;
				case BodyTarget.RightFoot:
					chosenPart = OwnUnit.data.footRight.gameObject;
					break;
				case BodyTarget.LeftHand:
					chosenPart = OwnUnit.data.leftHand.gameObject;
					break;
				case BodyTarget.RightHand:
					chosenPart = OwnUnit.data.rightHand.gameObject;
					break;
				default:
					chosenPart = OwnUnit.data.mainRig.gameObject;
					break;
			}
			
			var ownDamage = GetComponent<CollisionWeapon>();
			if (ownDamage)
			{
				NewDamage = chosenPart.AddComponent<CollisionWeapon>();
				NewDamage.damage = ownDamage.damage;
				NewDamage.impactMultiplier = ownDamage.impactMultiplier;
				NewDamage.onImpactForce = ownDamage.onImpactForce;
				NewDamage.massCap = ownDamage.massCap;
				NewDamage.ignoreTeamMates = ownDamage.ignoreTeamMates;
				NewDamage.staticDamageValue = ownDamage.staticDamageValue;
				NewDamage.onlyOncePerData = ownDamage.onlyOncePerData;
				NewDamage.cooldown = ownDamage.cooldown;
				NewDamage.onlyCollideWithRigs = true;
				NewDamage.dealDamageEvent = ownDamage.dealDamageEvent;
				Destroy(ownDamage);
			}
			
			var ownSound = GetComponent<CollisionSound>();
			if (ownSound)
			{
				NewSound = chosenPart.AddComponent<CollisionSound>();
				NewSound.SoundEffectRef = ownSound.SoundEffectRef;
				NewSound.multiplier = ownSound.multiplier;
				Destroy(ownSound);
			}

			var ownEffect = GetComponent<MeleeWeaponAddEffect>();
			if (ownEffect)
			{
				NewEffect = chosenPart.AddComponent<MeleeWeaponAddEffect>();
				NewEffect.EffectPrefab = ownEffect.EffectPrefab;
				NewEffect.ignoreTeamMates = ownEffect.ignoreTeamMates;
				Destroy(ownEffect);
			}
			
			var ownSpawn = GetComponent<MeleeWeaponSpawn>();
			if (ownSpawn)
			{
				NewSpawn = chosenPart.AddComponent<MeleeWeaponSpawn>();
				NewSpawn.objectToSpawn = ownSpawn.objectToSpawn;
				NewSpawn.pos = ownSpawn.pos;
				NewSpawn.rot = ownSpawn.rot;
				NewSpawn.cd = ownSpawn.cd;
				Destroy(ownSpawn);
			}
		}

		public void RemoveDamage()
		{
			if (NewDamage) Destroy(NewDamage);
			if (NewSound) Destroy(NewSound);
			if (NewEffect) Destroy(NewEffect);
			if (NewSpawn) Destroy(NewSpawn);
		}
	
		private void OnDestroy()
		{
			RemoveDamage();
		}
		
		private CollisionWeapon NewDamage;
		private CollisionSound NewSound;
		private MeleeWeaponAddEffect NewEffect;
		private MeleeWeaponSpawn NewSpawn;
		private Unit OwnUnit;
		
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