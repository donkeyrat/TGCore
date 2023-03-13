/*
using UnityEngine;
using HarmonyLib;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(Water), "OnTriggerStay")]
    internal class WaterDamagePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Water __instance, Collider other)
        {
			var componentInParent = other.GetComponentInParent<Rigidbody>();
			if (!componentInParent)
			{
				return false;
			}
			
			var componentInChildren = other.transform.root.GetComponentInChildren<DataHandler>();
			var drown = componentInChildren.unit.GetComponentInChildren<CannotDrown>();
			
			if (componentInChildren && !drown)
			{
				if (__instance.damageOverTime != 0f && !componentInChildren.Dead && other.gameObject.layer == LayerMask.NameToLayer("MainRig"))
				{
					componentInChildren.healthHandler.TakeDamage(__instance.damageOverTime * Time.deltaTime * (0.01f * componentInChildren.maxHealth), Vector3.zero, null, DamageType.Default);
				}
				if (componentInChildren.Dead && __instance.deadForce != 0f)
				{
					componentInParent.AddForce(Vector3.up * Mathf.Clamp((__instance.transform.position.y + __instance.offet - other.transform.position.y) * 10f, 0f, 10f) * __instance.deadForce * Time.deltaTime, ForceMode.Acceleration);
				}
				componentInChildren.sinceGrounded = Mathf.Clamp(componentInChildren.sinceGrounded, float.NegativeInfinity, 1f);
				componentInParent.AddForce(Vector3.up * Mathf.Clamp((__instance.transform.position.y + __instance.offet - other.transform.position.y) * 10f, 0f, 10f) * __instance.force * Time.deltaTime, ForceMode.Acceleration);
				componentInParent.AddForce(__instance.transform.forward * Mathf.Clamp((__instance.transform.position.y + __instance.offet - other.transform.position.y) * 10f, 0f, 10f) * __instance.streamForce * Time.deltaTime, ForceMode.Acceleration);
			}
			else if (componentInChildren && drown) 
			{
				if (componentInChildren.Dead && __instance.deadForce != 0f) 
				{
					componentInParent.AddForce(Vector3.up * Mathf.Clamp((__instance.transform.position.y + __instance.offet - other.transform.position.y) * 10f, 0f, 10f) * __instance.deadForce * Time.deltaTime, ForceMode.Acceleration);
				}
				else componentInParent.AddForce(Vector3.up * 1000f * drown.upForceMultiplier * Time.deltaTime, ForceMode.Acceleration);
				return false;
			}
			
 			componentInParent.angularVelocity *= 0.9f;
			componentInParent.velocity *= 0.9f;
			return false;
        }
    }
}
*/