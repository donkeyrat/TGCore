using Landfall.TABS;
using HarmonyLib;
using TGCore.Library;
using UnityEngine;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(ProjectileStick), "Stick")]
    internal class ProjectileStickPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ProjectileStick __instance, Rigidbody rig)
        {
            if (rig) 
            {
                var stuckProjectiles = rig.transform.root.gameObject.GetComponent<StuckProjectilesList>();
                if (!stuckProjectiles) stuckProjectiles = rig.transform.root.gameObject.AddComponent<StuckProjectilesList>();
                stuckProjectiles.sticks.Add(__instance);
            }
        }
    }
}