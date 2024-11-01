using System.Collections;
using Landfall.TABS;
using TFBGames;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
	public class LaunchableWeapon : MonoBehaviour
	{
		private void Start()
		{
			RootUnit = transform.root.GetComponent<Unit>();
			StartingMass = throwableRig.mass;
		}
    
		public void Launch()
		{
			StartCoroutine(DoLaunch());
		}

		private IEnumerator DoLaunch()
		{
			preLaunchEvent?.Invoke();

			yield return new WaitForSeconds(delay);
	    
			joint.xMotion = ConfigurableJointMotion.Free;
			joint.yMotion = ConfigurableJointMotion.Free;
			joint.zMotion = ConfigurableJointMotion.Free;

			var targetRig = RootUnit.data.targetMainRig;
        
			var direction = (targetRig.position - throwableRig.position).normalized;

			throwableRig.transform.rotation = Quaternion.LookRotation(direction + 0.01f * spread * Random.insideUnitSphere);
			SetProjectileStats(direction, direction, targetRig, targetRig.position, targetRig.velocity);

			throwableRig.velocity = Vector3.zero;

			throwableRig.AddForce(throwableRig.transform.TransformDirection(force), ForceMode.VelocityChange);
			throwableRig.AddTorque(FixedTimeStepService.TorqueCoefficient * throwableRig.transform.TransformDirection(torque), ForceMode.VelocityChange);
	    
			throwableRig.mass = mass;
			launchEvent?.Invoke();
	    
	    
			yield return new WaitForSeconds(reelDelay);
	    
	    
			throwableRig.mass = StartingMass;
			beginReelEvent?.Invoke();

			var linearLimit = joint.linearLimit;

			joint.linearLimit = linearLimit;
	    
			joint.xMotion = ConfigurableJointMotion.Limited;
			joint.yMotion = ConfigurableJointMotion.Limited;
			joint.zMotion = ConfigurableJointMotion.Limited;

			var counter = 0f;
			while (counter < reelTime)
			{
				counter += Time.deltaTime;
				counter = Mathf.Clamp(counter, 0f, reelTime);
		    
				var relativeCounter = counter / reelTime;
				linearLimit.limit = Mathf.Lerp(100f, 0f, linearLimitCurve.Evaluate(relativeCounter));
		    
				joint.linearLimit = linearLimit;
				yield return null;
			}
	    
			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Locked;
	    
			finishReelEvent?.Invoke();
		}

		public void SetProjectileStats(Vector3 spawnDir, Vector3 directionToTarget,
			Rigidbody targetRig, Vector3 targetRigPosition, Vector3 targetRigVelocity)
		{
			var compensation = throwableRig.GetComponent<Compensation>();
			if (compensation && targetRig)
			{
				throwableRig.transform.rotation = Quaternion.LookRotation(
					compensation.GetCompensation(targetRigPosition, targetRigVelocity,
						0f) + Random.insideUnitSphere * (spread * 0.01f));
				
				if (compensation.forwardCompensation > 0f)
				{
					force.z += Mathf.Pow(
						Mathf.Clamp(Vector3.Distance(targetRigPosition, base.transform.position), 0f,
							compensation.clampDistance), compensation.rangePow) * compensation.forwardCompensation;
				}
			}
		}


		private Unit RootUnit;
		private float StartingMass;
    
		public Rigidbody throwableRig;
		public ConfigurableJoint joint;

		[Header("Launch")] 
    
		public Vector3 force;
		public Vector3 torque;
		public float spread;
		public float mass;
    
		public UnityEvent preLaunchEvent;
		public UnityEvent launchEvent;
		public float delay;

		[Header("Reel")]
    
		public float reelDelay = 3f;
		public float reelTime = 3f;

		public AnimationCurve linearLimitCurve;

		public UnityEvent beginReelEvent;
		public UnityEvent finishReelEvent;
	}
}
