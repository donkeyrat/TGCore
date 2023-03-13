using UnityEngine;
using HarmonyLib;

namespace TGCore.HarmonyPatches
{
    
    [HarmonyPatch(typeof(TABSBooter), "Start")]
    internal class SplashScreenPatch 
    {
        [HarmonyPrefix]
        public static bool Prefix(TABSBooter __instance)
        {
            if (DisplaySplashScreen)
            {
                TABSSceneManager.LoadScene("TGCoreScene", true);
            
                DisplaySplashScreen = false;
                __instance.enabled = false;
                return false;
            }
            return true;
        }

        private static bool DisplaySplashScreen = true;
    }
}