using System;
using System.Collections.Generic;
using Landfall.MonoBatch;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
	public class CollisionWeaponToggleable : BatchedMonobehaviour, GameObjectPooling.IPoolable
	{
		public enum CallEffectsOn
		{
			Rigidbodies,
			Ground,
			All
		}
	
		private MeleeWeaponMultiplierPoint MultiplierPoint;

		public bool canDealDamage;
	
		public float damage = 80f;
	
		public float selfDamageMultiplier;
	
		public float impactMultiplier = 1f;
	
		public float minVelocity;
	
		public float screenShakeMultiplier = 1f;
	
		public float onImpactForce;
	
		public float massCap = 5f;
	
		public bool ignoreTeamMates;
	
		public float teamDamage = 0.1f;
	
		public bool staticDamageValue;
	
		public bool onlyOncePerData;
	
		public bool hitFasterAfterDealDamage = true;
	
		public bool useHitDirection;
	
		public bool playSoundWhenHitNonRigidbodies = true;
	
		private readonly List<DataHandler> HitDatas = new List<DataHandler>();
	
		private Holdable Holdable;
	
		private DataHandler ConnectedData;
	
		[HideInInspector]
		public List<Rigidbody> protectedRigs = new List<Rigidbody>();
	
		private CollisionWeaponEffect[] MeleeWeaponEffects;
	
		public Weapon weapon;

		public UnityEvent dealDamageEvent;
	
		private CollisionSound CollisionSound;
	
		public Damagable lastHitHealth;
	
		public float cooldown;
	
		private float SinceLastDamage;
	
		[HideInInspector]
		public bool onlyCollideWithRigs;
	
		private Level MyLevel;
	
		private Unit OwnUnit;
	
		public CallEffectsOn callEffectsOn;
	
		private Rigidbody Rig;
	
		private Action<Collision, float> CollisionAction;
	
		private Action<Collision, float, Vector3> DealDamageAction;
	
		public Action ReleaseSelf { get; set; }
	
		protected override void Start()
		{
			base.Start();
			CollisionSound = GetComponent<CollisionSound>();
			Rig = GetComponent<Rigidbody>();
			Holdable = GetComponent<Holdable>();
			OwnUnit = GetOwnUnit();
			
			if (!weapon && GetComponent<Weapon>()) weapon = GetComponent<Weapon>();
			
			MeleeWeaponEffects = GetComponents<CollisionWeaponEffect>();
			MultiplierPoint = GetComponentInChildren<MeleeWeaponMultiplierPoint>();
			if (weapon)
			{
				damage *= weapon.levelMultiplier;
			}
			MyLevel = GetComponent<Level>();
			if (MyLevel)
			{
				onImpactForce *= Mathf.Pow(MyLevel.level, 1.5f);
				massCap *= Mathf.Pow(MyLevel.level, 1.5f);
			}
		}

		private Unit GetOwnUnit()
		{
			var teamHolder = GetComponent<TeamHolder>();
			if (teamHolder)
			{
				return teamHolder.spawner.transform.root.GetComponent<Unit>();
			}

			var rootUnit = transform.root.GetComponent<Unit>();
			if (rootUnit) return rootUnit;

			if (ConnectedData) return ConnectedData.unit;
			
			return null;
		}
	
		public override void BatchedUpdate()
		{
			if (cooldown != 0f)
			{
				SinceLastDamage += Time.deltaTime;
			}
		}
	
		private void OnCollisionEnter(Collision collision)
		{
			if (onlyCollideWithRigs && !collision.rigidbody)
			{
				return;
			}
			CollisionAction?.Invoke(collision, 0f);
			if (!canDealDamage || (weapon && weapon.GetType() == typeof(MeleeWeapon) && !((MeleeWeapon)weapon).canDealDamage))
			{
				return;
			}
			var multiplier = 0f;
			if (Rig)
			{
				if (collision.rigidbody)
				{
					multiplier = collision.impulse.magnitude / (Rig.mass + 10f) * 0.3f;
				}
				else
				{
					multiplier = collision.impulse.magnitude / Rig.mass * 0.3f;
				}
			}
			multiplier *= impactMultiplier;
			multiplier = Mathf.Clamp(multiplier, 0f, 2f);
			
			if (multiplier < 1f) return;
			
			if (!collision.rigidbody)
			{
				DoScreenShake(multiplier, collision);
				if (callEffectsOn == CallEffectsOn.All || callEffectsOn == CallEffectsOn.Ground)
				{
					DoCollisionEffects(collision.transform, collision);
				}
				if (CollisionSound && playSoundWhenHitNonRigidbodies)
				{
					CollisionSound.DoEffect(collision.transform, collision, multiplier);
				}
			}
			
			if (minVelocity != 0f && Rig && Rig.velocity.magnitude < minVelocity)
			{
				return;
			}
			if (!ConnectedData && Holdable && Holdable.held)
			{
				ConnectedData = Holdable.holderData;
			}
			if (collision.transform.root == transform.root || (ConnectedData && ConnectedData.transform.root == collision.transform.root) || !collision.rigidbody || protectedRigs.Contains(collision.rigidbody) || SinceLastDamage < cooldown)
			{
				return;
			}
			
			SinceLastDamage = 0f;
			var data = collision.rigidbody.GetComponentInParent<DataHandler>();
			var healthHandler = collision.rigidbody.GetComponentInParent<Damagable>();
			if (healthHandler)
			{
				if (data && onlyOncePerData)
				{
					if (HitDatas.Contains(data)) return;
					
					HitDatas.Add(data);
				}
				if (weapon && lastHitHealth == healthHandler) return;
				
				lastHitHealth = healthHandler;
				
				if (CollisionSound) CollisionSound.DoEffect(collision.transform, collision, multiplier);

				Unit unit2 = null;
				if (data) unit2 = data.GetComponentInParent<Unit>();
				
				
				if (unit2 && OwnUnit && unit2.Team == OwnUnit.Team && ignoreTeamMates)
				{
					return;
				}
				
				if (!Holdable && OwnUnit && OwnUnit.data.Dead)
				{
					Destroy(this);
					return;
				}
				
				var multiplier2 = multiplier;
				if (staticDamageValue)
				{
					multiplier2 = 1f;
				}
				if (MultiplierPoint && Vector3.Distance(collision.contacts[0].point, MultiplierPoint.transform.position) < MultiplierPoint.range)
				{
					multiplier2 *= MultiplierPoint.multiplier;
				}
				if (unit2 && OwnUnit && unit2.Team == OwnUnit.Team)
				{
					multiplier2 *= teamDamage;
				}
				
				var direction = transform.forward;
				if (useHitDirection)
				{
					direction = collision.transform.position - transform.position;
				}
				
				if (data)
				{
					WilhelmPhysicsFunctions.AddForceWithMinWeight(data.mainRig, (staticDamageValue ? 5f : Mathf.Sqrt(multiplier * 50f)) * direction * onImpactForce, ForceMode.Impulse, massCap);
					WilhelmPhysicsFunctions.AddForceWithMinWeight(collision.rigidbody, (staticDamageValue ? 5f : Mathf.Sqrt(multiplier * 50f)) * direction * onImpactForce, ForceMode.Impulse, massCap);
				}
				
				if (collision.rigidbody.mass < Rig.mass)
				{
					collision.rigidbody.velocity *= 0.6f;
					if (data) data.mainRig.velocity *= 0.6f;
				}
				
				if (!OwnUnit) OwnUnit = GetOwnUnit();
				
				DealDamageAction?.Invoke(collision, damage * multiplier2, direction);
				lastHitHealth.TakeDamage(damage * multiplier2, direction, OwnUnit);
				
				if (selfDamageMultiplier != 0f && OwnUnit)
				{
					OwnUnit.data.healthHandler.TakeDamage(damage * multiplier2 * selfDamageMultiplier, direction);
				}

				dealDamageEvent?.Invoke();

				if (weapon && hitFasterAfterDealDamage)
				{
					weapon.internalCounter += UnityEngine.Random.Range(0f, weapon.internalCooldown * 0.5f);
				}
				if (data && (callEffectsOn == CallEffectsOn.All || callEffectsOn == CallEffectsOn.Rigidbodies))
				{
					DoCollisionEffects(data.mainRig.transform, collision);
				}
			}
			else if (CollisionSound)
			{
				CollisionSound.DoEffect(collision.transform, collision, multiplier);
			}
			
			DoScreenShake(multiplier, collision);
		}
	
		private void DoCollisionEffects(Transform targetTransform, Collision collision)
		{
			if (MeleeWeaponEffects != null && MeleeWeaponEffects.Length > 0)
			{
				foreach (var effect in MeleeWeaponEffects)
				{
					effect.DoEffect(targetTransform, collision);
				}
			}
		}
	
		private void DoScreenShake(float impact, Collision collision)
		{
			if (ScreenShake.Instance != null)
			{
				ScreenShake.Instance.AddForce(transform.forward * Mathf.Sqrt(impact * 0.5f) * 0.5f * screenShakeMultiplier, collision.contacts[0].point);
			}
		}
		
		public void AddCollisionAction(Action<Collision, float> action)
		{
			CollisionAction = (Action<Collision, float>)Delegate.Combine(CollisionAction, action);
		}
	
		public void AddDealDamageAction(Action<Collision, float, Vector3> action)
		{
			DealDamageAction = (Action<Collision, float, Vector3>)Delegate.Combine(DealDamageAction, action);
		}
	
		public void Initialize()
		{
		}
	
		public void Reset()
		{
		}
	
		public void Release()
		{
			HitDatas.Clear();
		}

		public void SetCanDealDamage(bool value)
		{
			canDealDamage = value;
		}

		public bool IsManagedByPool { get; set; }
	}
}