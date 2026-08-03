using HarmonyLib;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using Landfall.TABS.GameState;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(HealthHandler), "TakeDamage")]
    class ReturnDamagePatch
    {
        [HarmonyPrefix]
        public static void Prefix(HealthHandler __instance, float damage, Unit damager, GameStateManager ___m_gameStateManager, bool ___isInvulnerable, ref DataHandler ___data, SettingsInstance ___m_bugUnitsDying, GameModeService ___gameModeService)
        {
            if ((___m_gameStateManager == null || ___m_gameStateManager.GameState == GameState.BattleState) &&
                !___isInvulnerable && (!(___data.immunityForSeconds > 0f) || !(damage > 0f)) &&
                !(___data.lifeTime < 0.3f) && !___data.unit.WasDamaged(null, null)
                && damager && !___data.Dead)
            {
                var returnDamage = ___data.unit.GetComponentInChildren<Effect_ReturnDamage>();
                if (returnDamage) returnDamage.ReturnDamage(damager, damage, ___m_bugUnitsDying, ___gameModeService);
            }
        }
    }
}