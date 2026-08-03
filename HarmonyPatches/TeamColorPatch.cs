using DM;
using UnityEngine;
using HarmonyLib;
using Landfall.TABS;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(TeamColor), "Awake")]
    internal class TeamColorPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(TeamColor __instance)
        {
            var teamColors = ContentDatabase.Instance().GetUnitEditorColorPalette().TeamColors;
            for (var i = 0; i < teamColors.Length; i++)
            {
                if (teamColors[i].m_materialRed.color == __instance.redMaterial.color)
                {
                    __instance.SetField("m_teamColorIndex", i);
                    break;
                }
            }
            __instance.SetField("m_flipColorsSetting", ServiceLocator.GetService<GlobalSettingsHandler>().GetSettingsInstance("GAMEPLAY_FLIP_COLORS"));
            __instance.SetField("meshRenderer", __instance.GetComponent<Renderer>());
			return false;
        }
    }
}