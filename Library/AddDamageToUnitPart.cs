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
				case BodyTarget.Hip:
					chosenPart = OwnUnit.data.hip.gameObject;
					break;
				case BodyTarget.MainRig:
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
				NewDamage.callEffectsOn = ownDamage.callEffectsOn;
				Destroy(ownDamage);
			}
			
			var ownToggleable = GetComponent<CollisionWeaponToggleable>();
			if (ownToggleable)
			{
				NewToggleableDamage = chosenPart.AddComponent<CollisionWeaponToggleable>();
				NewToggleableDamage.damage = ownToggleable.damage;
				NewToggleableDamage.impactMultiplier = ownToggleable.impactMultiplier;
				NewToggleableDamage.onImpactForce = ownToggleable.onImpactForce;
				NewToggleableDamage.massCap = ownToggleable.massCap;
				NewToggleableDamage.ignoreTeamMates = ownToggleable.ignoreTeamMates;
				NewToggleableDamage.staticDamageValue = ownToggleable.staticDamageValue;
				NewToggleableDamage.onlyOncePerData = ownToggleable.onlyOncePerData;
				NewToggleableDamage.cooldown = ownToggleable.cooldown;
				NewToggleableDamage.dealDamageEvent = ownToggleable.dealDamageEvent;
				NewToggleableDamage.canDealDamage = ownToggleable.canDealDamage;
				NewToggleableDamage.callEffectsOn = ownToggleable.callEffectsOn;
				Destroy(ownToggleable);
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
				NewSpawn.SpawnEvent = ownSpawn.SpawnEvent;
				Destroy(ownSpawn);
			}
		}

		public void RemoveDamage()
		{
			if (NewDamage) Destroy(NewDamage);
			if (NewToggleableDamage) Destroy(NewDamage);
			if (NewSound) Destroy(NewSound);
			if (NewEffect) Destroy(NewEffect);
			if (NewSpawn) Destroy(NewSpawn);
		}

		public void Release()
		{
			if (NewDamage) NewDamage.Release();
			if (NewToggleableDamage) NewToggleableDamage.Release();
		}

		public void SetCanDealDamage(bool value)
		{
			if (NewToggleableDamage) NewToggleableDamage.SetCanDealDamage(value);
		}
	
		private void OnDestroy()
		{
			RemoveDamage();
		}
		
		private CollisionWeapon NewDamage;
		private CollisionWeaponToggleable NewToggleableDamage;
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
			LeftHand,
			MainRig,
			Hip
		}
		
		public BodyTarget bodyTarget;
	}
}