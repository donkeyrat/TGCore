using Landfall.TABS;
using HarmonyLib;
using TFBGames;
using TGCore.Library;
using UnityEngine;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(Unit), "SetHighlight")]
    internal class CustomOutlinePatch
    {
        [HarmonyPostfix]
        public static void Postfix(Unit __instance, ref Color highlightColor)
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
