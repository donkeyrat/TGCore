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
	
		private MeleeWeaponMultiplierPoint multiplierPoint;

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
	
		private readonly List<DataHandler> hitDatas = new List<DataHandler>();
	
		private Holdable holdable;
	
		private DataHandler connectedData;
	
		[HideInInspector]
		public List<Rigidbody> protectedRigs = new List<Rigidbody>();
	
		private CollisionWeaponEffect[] meleeWeaponEffects;
	
		public Weapon weapon;
	
		private TeamHolder teamHolder;
	
		public UnityEvent dealDamageEvent;
	
		private CollisionSound collisionSound;
	
		public Damagable lastHitHealth;
	
		public float cooldown;
	
		private float sinceLastDamage;
	
		[HideInInspector]
		public bool onlyCollideWithRigs;
	
		private Level myLevel;
	
		private Unit ownUnit;
	
		public CallEffectsOn callEffectsOn;
	
		private Rigidbody rig;
	
		private Action<Collision, float> CollisionAction;
	
		private Action<Collision, float, Vector3> DealDamageAction;
	
		public Action ReleaseSelf { get; set; }
	
		protected override void Start()
		{
			base.Start();
			collisionSound = GetComponent<CollisionSound>();
			rig = GetComponent<Rigidbody>();
			holdable = GetComponent<Holdable>();
			teamHolder = GetComponent<TeamHolder>();
			
			if (!weapon && GetComponent<Weapon>()) weapon = GetComponent<Weapon>();
			
			meleeWeaponEffects = GetComponents<CollisionWeaponEffect>();
			multiplierPoint = GetComponentInChildren<MeleeWeaponMultiplierPoint>();
			if (weapon)
			{
				damage *= weapon.levelMultiplier;
			}
			myLevel = GetComponent<Level>();
			if (myLevel)
			{
				onImpactForce *= Mathf.Pow(myLevel.level, 1.5f);
				massCap *= Mathf.Pow(myLevel.level, 1.5f);
			}
		}
	
		public override void BatchedUpdate()
		{
			if (cooldown != 0f)
			{
				sinceLastDamage += Time.deltaTime;
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
			if (rig)
			{
				if (collision.rigidbody)
				{
					multiplier = collision.impulse.magnitude / (rig.mass + 10f) * 0.3f;
				}
				else
				{
					multiplier = collision.impulse.magnitude / rig.mass * 0.3f;
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
				if (collisionSound && playSoundWhenHitNonRigidbodies)
				{
					collisionSound.DoEffect(collision.transform, collision, multiplier);
				}
			}
			
			if (minVelocity != 0f && rig && rig.velocity.magnitude < minVelocity)
			{
				return;
			}
			if (!connectedData && holdable && holdable.held)
			{
				connectedData = holdable.holderData;
			}
			if (collision.transform.root == transform.root || (connectedData && connectedData.transform.root == collision.transform.root) || !collision.rigidbody || protectedRigs.Contains(collision.rigidbody) || sinceLastDamage < cooldown)
			{
				return;
			}
			
			sinceLastDamage = 0f;
			var data = collision.rigidbody.GetComponentInParent<DataHandler>();
			var healthHandler = collision.rigidbody.GetComponentInParent<Damagable>();
			if (healthHandler)
			{
				if (data && onlyOncePerData)
				{
					if (hitDatas.Contains(data)) return;
					
					hitDatas.Add(data);
				}
				if (weapon && lastHitHealth == healthHandler) return;
				
				lastHitHealth = healthHandler;
				
				if (collisionSound) collisionSound.DoEffect(collision.transform, collision, multiplier);
				
				var unit = connectedData ? connectedData.GetComponentInParent<Unit>() : GetComponentInParent<Unit>();
				Unit unit2 = null;
				if (data) unit2 = data.GetComponentInParent<Unit>();
				
				if (unit2 && teamHolder && unit2.Team == teamHolder.team && ignoreTeamMates)
				{
					return;
				}
				
				if (!holdable && unit && unit.data.Dead)
				{
					Destroy(this);
					return;
				}
				
				var multiplier2 = multiplier;
				if (staticDamageValue)
				{
					multiplier2 = 1f;
				}
				if (multiplierPoint && Vector3.Distance(collision.contacts[0].point, multiplierPoint.transform.position) < multiplierPoint.range)
				{
					multiplier2 *= multiplierPoint.multiplier;
				}
				if (unit2 && teamHolder && unit2.Team == teamHolder.team)
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
				
				if (collision.rigidbody.mass < rig.mass)
				{
					collision.rigidbody.velocity *= 0.6f;
					if (data) data.mainRig.velocity *= 0.6f;
				}
				
				if (!ownUnit)
				{
					if (connectedData && connectedData.unit)
					{
						ownUnit = connectedData.unit;
					}
					else if (teamHolder && teamHolder.spawner)
					{
						ownUnit = teamHolder.spawner.GetComponentInParent<Unit>();
					}
				}
				
				DealDamageAction?.Invoke(collision, damage * multiplier2, direction);
				lastHitHealth.TakeDamage(damage * multiplier2, direction, ownUnit);
				
				if (selfDamageMultiplier != 0f && unit)
				{
					unit.data.healthHandler.TakeDamage(damage * multiplier2 * selfDamageMultiplier, direction);
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
			else if (collisionSound)
			{
				collisionSound.DoEffect(collision.transform, collision, multiplier);
			}
			
			DoScreenShake(multiplier, collision);
		}
	
		private void DoCollisionEffects(Transform targetTransform, Collision collision)
		{
			if (meleeWeaponEffects != null && meleeWeaponEffects.Length > 0)
			{
				foreach (var effect in meleeWeaponEffects)
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
			hitDatas.Clear();
		}

		public void SetCanDealDamage(bool value)
		{
			canDealDamage = value;
		}

		public bool IsManagedByPool { get; set; }
	}
}