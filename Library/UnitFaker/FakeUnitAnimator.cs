using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
	public class FakeUnitAnimator : MonoBehaviour
	{
		private void Start()
		{
			OwnUnit = transform.root.GetComponent<FakeUnit>();
			Moves = GetComponents<FakeUnitAnimation>();
		}

		public void DoMove(Transform targetObj)
		{
			targetObject = targetObj;
			DoMove();
		}

		public void DoMove()
		{
			if (!OwnUnit)
			{
				OwnUnit = transform.root.GetComponent<FakeUnit>();
			}

			if (!OwnUnit.target) return;
			
			DoMove(null, OwnUnit.target.data.mainRig, OwnUnit.target.data);
		}

		public void DoMove(Rigidbody enemyWeapon, Rigidbody enemyTorso, DataHandler targetData)
		{
			if (cancelSelf)
			{
				StopAllCoroutines();
			}
			if (!enemyWeapon && !enemyTorso)
			{
				return;
			}
			foreach (var move in Moves)
			{
				StartCoroutine(DoMoveSequence(move, enemyWeapon, enemyTorso, targetData));
			}
		}

		private IEnumerator DoMoveSequence(FakeUnitAnimation move, Rigidbody enemyWeapon, Rigidbody enemyTorso, DataHandler targetData)
		{
			var t = move.forceCurve.keys[move.forceCurve.keys.Length - 1].time;
			var c = 0f;
			move.randomMultiplier = move.randomCurve.Evaluate(Random.value);
			var rigs = new List<Rigidbody>();
			if (move.rigidbodyToMove == FakeUnitAnimation.RigidBodyToMove.Torso)
			{
				rigs.Add(OwnUnit.core);
			}
			else if (move.rigidbodyToMove == FakeUnitAnimation.RigidBodyToMove.AllRigs)
			{
				rigs.AddRange(OwnUnit.rigs);
			}
			else if (move.rigidbodyToMove == FakeUnitAnimation.RigidBodyToMove.This)
			{
				rigs.Add(GetComponent<Rigidbody>());
			}
			else if (move.rigidbodyToMove == FakeUnitAnimation.RigidBodyToMove.Specific && move.specificRig)
			{
				rigs.Add(move.specificRig);
			}
			var forceDirection = Vector3.zero;
			if (rigs.Count >= 1)
			{
				forceDirection = GetDirection(move, enemyWeapon, enemyTorso, rigs[0], targetData);
				for (var num = rigs.Count - 1; num >= 0; num--)
				{
					if (rigs[num] == null)
					{
						rigs.RemoveAt(num);
					}
				}
			}
			var massM = 1f;
			if (divideForceByMass)
			{
				var massSum = rigs.Sum(rig => rig.mass / rigs.Count);
				massM = 1f / massSum;
			}
			while (c < t && OwnUnit)
			{
				if (CheckConditions())
				{
					foreach (var rig in rigs)
					{
						if (move.setDirectionContinuous)
						{
							forceDirection = GetDirection(move, enemyWeapon, enemyTorso, rigs[0], targetData);
						}
						if (move.force != 0f && rig)
						{
							rig.AddForce(forceDirection * (massM * move.randomMultiplier * forceMultiplier * move.force * move.forceCurve.Evaluate(c)), ForceMode.Acceleration);
						}
						if (move.torque != 0f && rig)
						{
							rig.AddTorque(forceDirection * (massM * forceMultiplier * move.torque * move.forceCurve.Evaluate(c)), ForceMode.Acceleration);
						}
					}
				}
				var speed = 1f;
				speed *= animationSpeed;
				if (move.forceCurve.Evaluate(c) > 0f)
				{
					speed *= animationSpeedWhenPositiveCurve;
				}
				
				c += Time.fixedDeltaTime * speed;
				yield return new WaitForFixedUpdate();
			}
		}

		private bool CheckConditions()
		{
			var result = !(maxRange != 0f && OwnUnit && OwnUnit.distanceToTarget > maxRange);
			if (minRange != 0f && OwnUnit && OwnUnit.distanceToTarget < minRange)
			{
				result = false;
			}
			return result;
		}

		private Vector3 GetDirection(FakeUnitAnimation move, Rigidbody enemyWeapon, Rigidbody enemyTorso, Rigidbody ownRig, DataHandler targetData)
		{
			if (!enemyTorso && OwnUnit.target)
			{
				enemyTorso = OwnUnit.target.data.mainRig;
			}
			var result = Vector3.zero;
			if (ownRig == null)
			{
				return result;
			}
			if (move.forceDirection == FakeUnitAnimation.ForceDirection.Up)
			{
				result = Vector3.up;
			}
			if (move.forceDirection == FakeUnitAnimation.ForceDirection.TowardsTarget && ownRig && enemyTorso)
			{
				result = enemyTorso.position - ownRig.position;
				if (move.normalize)
				{
					result = result.normalized;
				}
			}
			if (move.forceDirection == FakeUnitAnimation.ForceDirection.TowardsTargetHead && ownRig && targetData && targetData.head)
			{
				result = targetData.head.position + targetData.head.transform.forward * 0.1f + targetData.head.transform.up * 0.15f - ownRig.position;
				if (move.normalize)
				{
					result = result.normalized;
				}
			}
			if (move.forceDirection == FakeUnitAnimation.ForceDirection.AwayFromTargetWeapon)
			{
				if (enemyWeapon)
				{
					result = ownRig.position - (enemyWeapon.worldCenterOfMass + enemyWeapon.velocity * move.predictionAmount);
					if (move.normalize)
					{
						result = result.normalized;
					}
					if (move.ignoreY)
					{
						result = new Vector3(result.x, 0f, result.y);
					}
				}
				else if (enemyTorso)
				{
					result = -(enemyTorso.position - ownRig.position);
					if (move.normalize)
					{
						result = result.normalized;
					}
				}
			}
			if (move.forceDirection == FakeUnitAnimation.ForceDirection.CharacterForward)
			{
				result = OwnUnit.movementDirection.forward;
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.CharacterRight)
			{
				result = OwnUnit.movementDirection.right;
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.CrossUpAndAwayFromAttacker && ownRig && enemyTorso)
			{
				result = Vector3.Cross(Vector3.up, ownRig.position - enemyTorso.position);
				if (move.normalize)
				{
					result = result.normalized;
				}
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.CrossUpAndTowardsUnitTarget && ownRig && OwnUnit.target)
			{
				result = Vector3.Cross(Vector3.up, ownRig.position - OwnUnit.target.data.mainRig.position);
				if (move.normalize)
				{
					result = result.normalized;
				}
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.RigUp && ownRig)
			{
				result = ownRig.transform.up;
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.RotateTowardsTarget && enemyTorso && ownRig)
			{
				result = -Vector3.Cross(enemyTorso.position - ownRig.position, ownRig.transform.forward).normalized * Vector3.Angle(enemyTorso.position - ownRig.position, ownRig.transform.forward);
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.RotateTowardsTargetHead && targetData && targetData.head && ownRig)
			{
				result = -Vector3.Cross(targetData.head.position + targetData.head.transform.forward * 0.1f + targetData.head.transform.up * 0.15f - ownRig.position, ownRig.transform.forward).normalized * Vector3.Angle(targetData.head.position + targetData.head.transform.forward * 0.1f + targetData.head.transform.up * 0.15f - ownRig.position, ownRig.transform.forward);
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.AwayFromTargetObject && targetObject && ownRig)
			{
				result = ownRig.transform.position - targetObject.transform.position;
				if (move.normalize)
				{
					result = result.normalized;
				}
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.CrossUpAndAwayFromTargetObject && targetObject && ownRig)
			{
				result = Vector3.Cross(Vector3.up, ownRig.position - targetObject.position);
				if (move.normalize)
				{
					result = result.normalized;
				}
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.InWalkDirection)
			{
				result = OwnUnit.movementDirection.forward;
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.RotateTowardsWalkDirection)
			{
				result = Vector3.Cross(ownRig.transform.forward, OwnUnit.movementDirection.forward).normalized * Vector3.Angle(ownRig.transform.forward, OwnUnit.movementDirection.forward);
			}
			else if (move.randomizeDirection && Random.value > 0.5f)
			{
				result *= -1f;
			}
			else if (move.forceDirection == FakeUnitAnimation.ForceDirection.TowardTargetWithoutY && ownRig && enemyTorso)
			{
				result = new Vector3(enemyTorso.position.x - ownRig.position.x, 0f, enemyTorso.position.z - ownRig.position.z);
			}
			return result;
		}

		private FakeUnit OwnUnit;
		private FakeUnitAnimation[] Moves;
		
		[HideInInspector]
		public Transform targetObject;

		public float animationSpeed = 1f;
		public float animationSpeedWhenPositiveCurve = 1f;

		public float forceMultiplier = 1f;

		public bool divideForceByMass;

		public bool cancelSelf;

		public float minRange;
		public float maxRange;
	}
}
