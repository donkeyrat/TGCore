/*
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(Mount), "Fall")]
    internal class MountForcePatch
    {
        [HarmonyPrefix]
        public static void Postfix(Mount __instance, ref MountPos ___myMountPos, ref DataHandler ___otherData, ref MovementHandler ___mountMove, ref bool ___isMounted, ref DataHandler ___data, ref Landfall.TABS.AI.UnitAPI ___m_unitApi, ref RigidbodyHolder ___rigHolder, ref ConfigurableJoint[] ___joints)
		{
			if (___myMountPos.GetComponent<SidewaysForce>())
			{
				___data.mainRig.AddForce(___data.mainRig.transform.right * ___myMountPos.GetComponent<SidewaysForce>().sidewaysForce, ForceMode.VelocityChange);
			}
        }
    }
}
*/