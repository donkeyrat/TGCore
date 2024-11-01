using System.Collections;
using TFBGames;
using UnityEngine;

namespace TGCore.Library
{
	public class WeaponForceAnimationToggleable : AttackEffect
	{
		private void Start()
		{
			Rig = GetComponentInParent<Rigidbody>();
		}

		public override void DoEffect(Rigidbody target, Vector3 targetDir)
		{
			if (!toggled) return;
			
			if (!Data)
			{
				Data = GetComponentInParent<Weapon>().connectedData;
			}
			if (!Holdable)
			{
				Holdable = GetComponentInParent<Holdable>();
			}
			if (chance == 0f || !(chance < Random.value))
			{
				foreach (var anim in animations)
				{
					StartCoroutine(PlayAnimationAfterDelay(anim, target ? target.position : (transform.position + targetDir), target));
				}
			}
		}

		private IEnumerator PlayAnimationAfterDelay(SpellAnimation animation, Vector3 position, Rigidbody targetRig = null)
		{
			if (upwardsModifier != 0f)
			{
				position += Vector3.up * upwardsModifier;
			}
			if (Holdable && Holdable.hl && animation.invertForceIfLeft)
			{
				animation.rigAnimationForce *= -1f;
			}
			var usedRig = new[] { Rig };
			switch (animation.animationRig)
			{
				case SpellAnimation.AnimationRig.All:
					usedRig = Data.allRigs.AllRigs;
					break;
				case SpellAnimation.AnimationRig.Torso:
					usedRig[0] = Data.mainRig;
					break;
				case SpellAnimation.AnimationRig.Hip:
					usedRig[0] = Data.hip;
					break;
				case SpellAnimation.AnimationRig.ThisRig:
					usedRig[0] = GetComponent<Rigidbody>();
					break;
			}

			var animationDirection = SetDirection(position, animation);
			yield return new WaitForSeconds(animation.animationDelay);
			var t = animation.rigAnimationCurve[animation.rigAnimationCurve.length - 1].time;
			var c = 0f;
			var ASM = Mathf.Clamp(Data.unit.attackSpeedMultiplier, 0f, 6f);
			while (c < t && Data.ragdollControl > 0.7f)
			{
				if (animation.setDirectionContinious && targetRig)
				{
					animationDirection = SetDirection(targetRig.position + Vector3.up * upwardsModifier, animation);
				}
				if (Data.sinceGrounded < 0.3f)
				{
					foreach (var rig in usedRig)
					{
						rig.AddForce(FixedTimeStepService.SmallForceCoefficient * 100f * animation.rigAnimationCurve.Evaluate(c) * animation.rigAnimationForce * ASM * Time.deltaTime * animationDirection, ForceMode.Acceleration);
					}
				}
				c += Time.deltaTime * ASM;
				yield return null;
			}
		}

		private Vector3 SetDirection(Vector3 position, SpellAnimation animation)
		{
			var result = (position - transform.position).normalized;
			switch (animation.animationDirection)
			{
				case RangeWeapon.SpawnRotation.TowardsTargetWithoutY:
					result = new Vector3(result.x, 0f, result.z).normalized;
					break;
				case RangeWeapon.SpawnRotation.Up:
					result = Vector3.up;
					break;
				case RangeWeapon.SpawnRotation.identity:
					result = Vector3.forward;
					break;
				case RangeWeapon.SpawnRotation.CharacterForward:
					result = Data.characterForwardObject.forward;
					break;
			}
			if (animation.rangeMultiplierCurve.length > 0)
			{
				result *= animation.rangeMultiplierCurve.Evaluate(Vector3.Distance(transform.position, position));
			}
			return result;
		}

		public void Toggle(bool onOff)
		{
			toggled = onOff;
		}
		
		private DataHandler Data;
		private Holdable Holdable;
		private Rigidbody Rig;
		
		public SpellAnimation[] animations;
		public float upwardsModifier;
		public float chance;
		public bool toggled;
	}
}
