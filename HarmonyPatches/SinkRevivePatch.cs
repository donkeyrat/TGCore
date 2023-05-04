using HarmonyLib;

namespace TGCore.HarmonyPatches 
{
    [HarmonyPatch(typeof(SinkOnDeath), "Sink")]
    internal class ReviveFixer 
    {
        [HarmonyPrefix]
        public static bool Prefix(SinkOnDeath __instance)
        {
            var data = __instance.GetComponentInChildren<DataHandler>();;
            return !data.healthHandler.willBeRewived;
        }
    }
}