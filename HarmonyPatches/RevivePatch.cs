using Landfall.TABS.GameMode;
using UnityEngine;
using HarmonyLib;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(DataHandler), "Dead", MethodType.Setter)]
    internal class RevivePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(DataHandler __instance, ref bool value)
        {
            if (value && !(bool)__instance.GetField("dead"))
            {
                GameModeService service = ServiceLocator.GetService<GameModeService>();
                if (service.CurrentGameMode == null)
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