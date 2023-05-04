using HarmonyLib;

namespace TGCore.HarmonyPatches 
{
    [HarmonyPatch(typeof(SinkOnDeath), "Sink")]
    internal class ReviveFixer 
    {
        
        [HarmonyPrefix]
        public static bool Prefix(SinkOnDeath __instance, ref DataHandler ___data)
        {
            return !___data.healthHandler.willBeRewived;
        }
    }
}