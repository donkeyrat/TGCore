using HarmonyLib;
using Landfall.TABS;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(HealthHandler), "Die", typeof(Unit))]
    internal class UnKillablePatch 
    {
        [HarmonyPrefix]
        public static bool Prefix(HealthHandler __instance, ref Unit damager)
        {
            return !__instance.transform.root.GetComponentInChildren<UnKillable>();
        }
    }
}
