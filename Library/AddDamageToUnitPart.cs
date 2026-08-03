using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class AddDamageToUnitPart : MonoBehaviour
	{
		private CollisionWeapon NewDamage;
		private CollisionWeaponToggleable NewToggleableDamage;
		private CollisionSound NewSound;
		private MeleeWeaponAddEffect NewEffect;
		private MeleeWeaponSpawn NewSpawn;
		private MeleeWeaponSpawnToggleable NewToggleableSpawn;
		private Unit OwnUnit;
		
		public enum BodyTarget
		{
			Head,
			RightFoot,
			LeftFoot,
			RightHand,
			LeftHand,
			MainRig,
			Hip,
			MainWeapon
		}
		
		public BodyTarget bodyTarget;
		public bool destroyOnFail;
		
		private void Start()
		{
			OwnUnit = transform.root.GetComponent<Unit>();

			GameObject chosenPart = null;
			switch (bodyTarget)
			{
				case BodyTarget.Head:
					if (OwnUnit.data.head) chosenPart = OwnUnit.data.head.gameObject;
					break;
				case BodyTarget.LeftFoot:
					if (OwnUnit.data.footLeft) chosenPart = OwnUnit.data.footLeft.gameObject;
					break;
				case BodyTarget.RightFoot:
					if (OwnUnit.data.footRight)chosenPart = OwnUnit.data.footRight.gameObject;
					break;
				case BodyTarget.LeftHand:
					if (OwnUnit.data.leftHand) chosenPart = OwnUnit.data.leftHand.gameObject;
					break;
				case BodyTarget.RightHand:
					if (OwnUnit.data.rightHand) chosenPart = OwnUnit.data.rightHand.gameObject;
					break;
				case BodyTarget.Hip:
					if (OwnUnit.data.hip) chosenPart = OwnUnit.data.hip.gameObject;
					break;
				case BodyTarget.MainWeapon:
					if (OwnUnit.WeaponHandler && (OwnUnit.WeaponHandler.rightWeapon || OwnUnit.WeaponHandler.leftWeapon))
					{
						chosenPart = OwnUnit.WeaponHandler.rightWeapon ? OwnUnit.WeaponHandler.rightWeapon.gameObject : OwnUnit.WeaponHandler.leftWeapon.gameObject;
					}
					break;
				case BodyTarget.MainRig:
				default:
					chosenPart = OwnUnit.data.mainRig.gameObject;
					break;
			}

			if (!chosenPart)
			{
				if (destroyOnFail) Destroy(gameObject);
				return;
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
				NewDamage.playSoundWhenHitNonRigidbodies = ownDamage.playSoundWhenHitNonRigidbodies;
				NewDamage.screenShakeMultiplier = ownDamage.screenShakeMultiplier;
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
				NewToggleableDamage.playSoundWhenHitNonRigidbodies = ownToggleable.playSoundWhenHitNonRigidbodies;
				NewToggleableDamage.screenShakeMultiplier = ownToggleable.screenShakeMultiplier;
				NewToggleableDamage.canDealDamage = ownToggleable.canDealDamage;
				Destroy(ownToggleable);
			}
			
			var ownSound = GetComponent<CollisionSound>();
			if (ownSound)
			{
				NewSound = chosenPart.AddComponent<CollisionSound>();
				NewSound.SoundEffectRef = ownSound.SoundEffectRef;
				NewSound.multiplier = ownSound.multiplier;
				NewSound.onlySoundOnRig = ownSound.onlySoundOnRig;
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
			
			var ownSpawnToggleable = GetComponent<MeleeWeaponSpawnToggleable>();
			if (ownSpawnToggleable)
			{
				NewToggleableSpawn = chosenPart.AddComponent<MeleeWeaponSpawnToggleable>();
				NewToggleableSpawn.objectToSpawn = ownSpawnToggleable.objectToSpawn;
				NewToggleableSpawn.pos = ownSpawnToggleable.pos;
				NewToggleableSpawn.rot = ownSpawnToggleable.rot;
				NewToggleableSpawn.cooldown = ownSpawnToggleable.cooldown;
				NewToggleableSpawn.spawnEvent = ownSpawnToggleable.spawnEvent;
				NewToggleableSpawn.impactMultiplier = ownSpawnToggleable.impactMultiplier;
				NewToggleableSpawn.startOnCooldown = ownSpawnToggleable.startOnCooldown;
				NewToggleableSpawn.useWeaponToToggle = ownSpawnToggleable.useWeaponToToggle;
				NewToggleableSpawn.collisionTarget = ownSpawnToggleable.collisionTarget;
				NewToggleableSpawn.toggled = ownSpawnToggleable.toggled;
				Destroy(ownSpawn);
			}
		}

		public void RemoveDamage()
		{
			if (NewDamage) Destroy(NewDamage);
			if (NewToggleableDamage) Destroy(NewToggleableDamage);
			if (NewSound) Destroy(NewSound);
			if (NewEffect) Destroy(NewEffect);
			if (NewSpawn) Destroy(NewSpawn);
			if (NewToggleableSpawn) Destroy(NewToggleableSpawn);
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
		
		public void SetCanSpawn(bool value)
		{
			if (NewToggleableSpawn) NewToggleableSpawn.Toggle(value);
		}
	
		private void OnDestroy()
		{
			RemoveDamage();
		}
	}
}