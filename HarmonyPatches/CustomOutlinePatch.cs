/*
using Landfall.TABS;
using HarmonyLib;
using TFBGames;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(Unit), "SetHighlight", MethodType.Setter)]
    internal class CustomOutlinePatch
    {
        [HarmonyPrefix]
        public static void Postfix(Unit __instance)
        {
            if (__instance.GetComponentInChildren<ChangeOutline>() != null)
            {
                var highlighter = (IHighlight)__instance.GetField("m_highlighter");
                highlighter.BeginHighlight();
                highlighter.SetHighlightColor(__instance.GetComponentInChildren<ChangeOutline>().outlineColor);
            }
        }
    }
}
*/