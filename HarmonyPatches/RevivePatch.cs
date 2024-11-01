using Landfall.TABS.GameMode;
using UnityEngine;
using HarmonyLib;
using TGCore;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(DataHandler), "Dead", MethodType.Setter)]
    internal class RevivePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(DataHandler __instance, ref bool value)
        {
            if (__instance && __instance.healthHandler && value && !(bool)__instance.GetField("dead"))
            {
                var service = ServiceLocator.GetService<GameModeService>();
                if (!service || service.CurrentGameMode == null)
                {
                    Debug.LogError("Could not find CurrentGameMode!");
                }
                else if (!__instance.healthHandler.willBeRewived)
                {
                    service.CurrentGameMode.OnUnitDied(__instance.unit);
                }
            }
            __instance.SetField("dead", value);
            return false;
        }
    }
}