using Landfall.TABS;
using HarmonyLib;
using TFBGames;
using TGCore.Library;
using UnityEngine;

namespace TGCore.HarmonyPatches;

[HarmonyPatch(typeof(AttackSpeedOverTimeEffect), "AddAttackSpeed")]
internal class CheerleaderPatch
{
    [HarmonyPostfix]
    public static void Postfix(AttackSpeedOverTimeEffect __instance, ref Unit ___unit)
    {
        if (___unit)
        {
            foreach (var boost in ___unit.GetComponentsInChildren<BoostAbilitySpeed>())
            {
                var conditional = boost.GetComponent<ConditionalEvent>();
                if (conditional)
                {
                    var cooldownCondition = conditional.events[0].conditions[0];
                    cooldownCondition.value -= __instance.attackSpeedToAdd * boost.multiplier;
                    cooldownCondition.value = Mathf.Clamp(cooldownCondition.value, boost.minimumCooldown, 25f);
                }
            }
        }
    }
}