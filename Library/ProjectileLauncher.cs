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
			var ranged = GetComponent<RangeWeapon>();
			if (ranged && ranged.ObjectToSpawn)
			{
				(objectToSpawn, ranged.ObjectToSpawn) = (ranged.ObjectToSpawn, objectToSpawn);
			}
		}

		private void Start() 
		{
			AttackEffects = GetComponents<ProjectileLauncherEffect>();
			ShowProjectile = GetComponent<ProjectileLauncherShowProjectile>();
			MyLevel = GetComponent<Level>();
			ShootPosition = GetComponentInChildren<ShootPosition>() ? GetComponentInChildren<ShootPosition>().transform : transform;
			
			var weapon = GetComponent<Weapon>();
			var rootUnit = transform.root.GetComponent<Unit>();
			var teamHolder = GetComponent<TeamHolder>();
			
			if (weapon)
			{
				OwnUnit = weapon.connectedData.unit;
				LevelMultiplier = weapon.levelMultiplier;
			}
			else if (rootUnit) OwnUnit = rootUnit;
			else if (teamHolder) OwnUnit = teamHolder.spawner.GetComponent<Unit>();
			
		}

        public void Throw()
        {
            StartCoroutine(DelayedSwing());
        }

        private IEnumerator DelayedSwing()
        {
			var target = OwnUnit.data.targetData.unit;
			
			if (!target) yield break;
			
			foreach (var effect in AttackEffects) effect.DoEffect(target.data.mainRig);

			attackEvent.Invoke();
			yield return new WaitForSeconds(spawnDelay);
			shootEvent.Invoke();

			if (ShowProjectile) ShowProjectile.pivot.gameObject.SetActive(false);

			var spawnDirection = GetSpawnDirection((target.data.mainRig.position - ShootPosition.position).normalized);

			for (var i = 0; i < projectilesToSpawn; i++)
			{
				var spawnedObject = Instantiate(objectToSpawn, ShootPosition.position,
					Quaternion.LookRotation(spawnDirection + 0.01f * spread * Random.insideUnitSphere),
					parentToMe ? transform : null);

				SetProjectileStats(spawnedObject, spawnDirection,
					(target.data.mainRig.position - ShootPosition.position).normalized, target.data.mainRig,
					ShootPosition.forward, target.data.mainRig.position, target.data.mainRig.velocity);
			
				var team = spawnedObject.AddComponent<TeamHolder>();
				team.spawner = OwnUnit.gameObject;
				team.spawnerWeapon = gameObject;
				team.team = OwnUnit.Team;
				team.target = target.data.mainRig;
			}
        }

		public void SetProjectileStats(GameObject projectile, Vector3 spawnDir, Vector3 directionToTarget, Rigidbody targetRig, Vector3 shootPositionForward, Vector3 targetRigPosition, Vector3 targetRigVelocity)
		{
			for (var i = 0; i < projectile.transform.childCount; i++)
			{
				var rig = projectile.transform.GetChild(i).GetComponent<Rigidbody>();
				if (rig)
				{
					rig.AddForce(spawnDir * 1f, ForceMode.VelocityChange);
					if (MyLevel)
					{
						rig.mass *= Mathf.Pow(MyLevel.level, 1.5f);
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
					addForce.force.z += Mathf.Pow(Mathf.Clamp(Vector3.Distance(targetRigPosition, transform.position), 0f, compensation.clampDistance), compensation.rangePow) * compensation.forwardCompensation;
				}
				var projectileHit = projectile.transform.GetChild(i).GetComponentInChildren<ProjectileHit>();
				if (projectileHit)
				{
					projectileHit.damage *= LevelMultiplier;
					
					if (OwnUnit.data.input.hasControl) projectileHit.alwaysHitTeamMates = true;
					if (MyLevel) projectileHit.ignoreTeamMates = MyLevel.ignoreTeam;
				}
				var collision = projectile.transform.GetChild(i).GetComponentInChildren<CollisionWeapon>();
				if (collision)
				{
					collision.damage *= LevelMultiplier;
				}
			}
			var level = projectile.GetComponent<Level>() ? projectile.GetComponent<Level>() : projectile.AddComponent<Level>();
			level.levelMultiplier = LevelMultiplier;
			if (MyLevel)
			{
				level.ignoreTeam = MyLevel.ignoreTeam;
				level.level = MyLevel.level;
			}
		}

		private Vector3 GetSpawnDirection(Vector3 directionToTarget)
		{
			var result = Vector3.Lerp(directionToTarget, ShootPosition.forward,
				shootHelpAngleCurve.Evaluate(Vector3.Angle(directionToTarget, ShootPosition.forward))).normalized;
			return result;
		}

		private Transform ShootPosition;
		private Unit OwnUnit;
		private ProjectileLauncherEffect[] AttackEffects;
		private ProjectileLauncherShowProjectile ShowProjectile;
		private Level MyLevel;
		private float LevelMultiplier = 1f;
		
		public UnityEvent attackEvent = new UnityEvent();

		public UnityEvent shootEvent = new UnityEvent();
		
		[Header("Spawning")]

		public GameObject objectToSpawn;

		public int projectilesToSpawn = 1;

        public float spawnDelay = 0.4f;

		public bool parentToMe;
		
		[Header("Angling")]
		
		public AnimationCurve shootHelpAngleCurve = new AnimationCurve();
		public float spread;
    }
}
