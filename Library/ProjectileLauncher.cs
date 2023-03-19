using System.Collections;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class ProjectileLauncher : MonoBehaviour
    {
		private void Awake() 
		{
			if (GetComponent<RangeWeapon>() && GetComponent<RangeWeapon>().ObjectToSpawn)
			{
				var dummy = objectToSpawn;
				objectToSpawn = GetComponent<RangeWeapon>().ObjectToSpawn; 
				GetComponent<RangeWeapon>().ObjectToSpawn = dummy;
			}
		}

		private void Start() 
		{
			attackEffects = GetComponents<ProjectileLauncherEffect>();
			showProjectile = GetComponent<ProjectileLauncherShowProjectile>();
			myLevel = GetComponent<Level>();
			shootPosition = GetComponentInChildren<ShootPosition>() ? GetComponentInChildren<ShootPosition>().transform : transform;
			
			var weapon = GetComponent<Weapon>();
			var rootUnit = transform.root.GetComponent<Unit>();
			var teamHolder = GetComponent<TeamHolder>();
			
			if (weapon)
			{
				ownUnit = weapon.connectedData.unit;
				levelMultiplier = weapon.levelMultiplier;
			}
			else if (rootUnit) ownUnit = rootUnit;
			else if (teamHolder) ownUnit = teamHolder.spawner.GetComponent<Unit>();
			
		}

        public void Throw()
        {
            StartCoroutine(DelayedSwing());
        }

        private IEnumerator DelayedSwing()
        {
			var target = ownUnit.data.targetData.unit;
			
			if (!target) yield break;
			
			foreach (var effect in attackEffects) effect.DoEffect(target.data.mainRig);

			attackEvent.Invoke();
			yield return new WaitForSeconds(spawnDelay);
			shootEvent.Invoke();

			if (showProjectile) showProjectile.pivot.gameObject.SetActive(false);

			spawnedObject = Instantiate(objectToSpawn, shootPosition.position, shootPosition.rotation, parentToMe ? transform : null);
			spawnedObject.GetComponent<MoveTransform>().velocity = (target.data.mainRig.position - transform.position).normalized * spawnedObject.GetComponent<MoveTransform>().selfImpulse.magnitude;
			
			var spawnDirection = GetSpawnDirection((target.data.mainRig.position - shootPosition.position).normalized, target.data.mainRig, new Vector3(0f, 0f, 0f));
			SetProjectileStats(spawnedObject, spawnDirection, (target.data.mainRig.position - shootPosition.position).normalized, target.data.mainRig, shootPosition.forward, target.data.mainRig.position, target.data.mainRig.velocity);
			
			var team = spawnedObject.AddComponent<TeamHolder>();
			team.spawner = ownUnit.gameObject;
			team.spawnerWeapon = gameObject;
			team.team = ownUnit.Team;
			team.target = target.data.mainRig;
        }

		public void SetProjectileStats(GameObject projectile, Vector3 spawnDir, Vector3 directionToTarget, Rigidbody targetRig, Vector3 shootPositionForward, Vector3 targetRigPosition, Vector3 targetRigVelocity)
		{
			for (var i = 0; i < projectile.transform.childCount; i++)
			{
				var rig = projectile.transform.GetChild(i).GetComponent<Rigidbody>();
				if (rig)
				{
					rig.AddForce(spawnDir * 1f, ForceMode.VelocityChange);
					if (myLevel)
					{
						rig.mass *= Mathf.Pow(myLevel.level, 1.5f);
					}
				}
				var compensation = projectile.transform.GetChild(i).GetComponentInChildren<Compensation>();
				if (compensation && targetRig)
				{
					compensation.transform.rotation = Quaternion.LookRotation(compensation.GetCompensation(targetRigPosition, targetRigVelocity, shootHelpAngleCurve.Evaluate(Vector3.Angle(directionToTarget, shootPositionForward))) + Random.insideUnitSphere * 0.01f);
				}
				var moveTransform = projectile.GetComponentInChildren<MoveTransform>();
				if (moveTransform)
				{
					if (compensation && compensation.forwardCompensation > 0f && targetRig)
					{
						moveTransform.selfImpulse.z += Mathf.Pow(Mathf.Clamp(Vector3.Distance(targetRigPosition, transform.position), 0f, compensation.clampDistance), compensation.rangePow) * compensation.forwardCompensation;
					}
				}
				var addForce = projectile.GetComponent<AddForce>();
				if (addForce && compensation && compensation.forwardCompensation > 0f && targetRig)
				{
					addForce.force.z = addForce.force.z + Mathf.Pow(Mathf.Clamp(Vector3.Distance(targetRigPosition, transform.position), 0f, compensation.clampDistance), compensation.rangePow) * compensation.forwardCompensation;
				}
				var projectileHit = projectile.transform.GetChild(i).GetComponentInChildren<ProjectileHit>();
				if (projectileHit)
				{
					projectileHit.damage *= levelMultiplier;
					
					if (ownUnit.data.input.hasControl) projectileHit.alwaysHitTeamMates = true;
					if (myLevel) projectileHit.ignoreTeamMates = myLevel.ignoreTeam;
				}
				var collision = projectile.transform.GetChild(i).GetComponentInChildren<CollisionWeapon>();
				if (collision)
				{
					collision.damage *= levelMultiplier;
				}
			}
			var level = projectile.GetComponent<Level>() ? projectile.GetComponent<Level>() : projectile.AddComponent<Level>();
			level.levelMultiplier = levelMultiplier;
			if (myLevel)
			{
				level.ignoreTeam = myLevel.ignoreTeam;
				level.level = myLevel.level;
			}
		}

		private Vector3 GetSpawnDirection(Vector3 directionToTarget, Rigidbody targetRig, Vector3 forcedDirection)
		{
			var result = Vector3.Lerp(directionToTarget, shootPosition.forward, shootHelpAngleCurve.Evaluate(Vector3.Angle(directionToTarget, shootPosition.forward))).normalized;
			if (ownUnit.data.input.hasControl)
			{
				if (!targetRig)
				{
					result = forcedDirection;
				}
			}
			return result;
		}

		private Transform shootPosition;
		private Unit ownUnit;
		private GameObject spawnedObject;
		private ProjectileLauncherEffect[] attackEffects;
		private ProjectileLauncherShowProjectile showProjectile;
		private Level myLevel;
		private float levelMultiplier = 1f;
		
		[Header("Spawn Settings")]

		public GameObject objectToSpawn;

        public float spawnDelay = 0.4f;

		public bool parentToMe;
		
		[Header("Other Settings")]
		
		public AnimationCurve shootHelpAngleCurve = new AnimationCurve();

		public UnityEvent attackEvent = new UnityEvent();

		public UnityEvent shootEvent = new UnityEvent();
    }
}
