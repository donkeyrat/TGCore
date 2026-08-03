using HarmonyLib;
using Landfall.TABS;
using TGCore.Library;
using UnityEngine;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(CollisionWeapon), "OnCollisionEnter")]
    class ArmorWeaponPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(CollisionWeapon __instance, Collision collision, ref MeleeWeapon ___meleeWeapon, ref Rigidbody ___rig, ref DataHandler ___connectedData)
        {
            var armoredUnit = collision.transform.root.GetComponent<AchillesArmor.UnitIsArmored>();
            if (collision.transform && armoredUnit && armoredUnit.armorActive && collision.rigidbody && ___rig && ___meleeWeapon && ___meleeWeapon.isSwinging && ___connectedData && armoredUnit.GetComponent<Unit>().Team != ___connectedData.unit.Team && armoredUnit.parryPower > ___meleeWeapon.requiredPowerToParry)
            {
                ___meleeWeapon.StopSwing();
                ___rig.AddForce(collision.contacts[0].point.normalized * -armoredUnit.parryForce, ForceMode.VelocityChange);
                
                Object.Instantiate(armoredUnit.weaponHitEffect, collision.contacts[0].point, Quaternion.identity);
            }
            return true;
        }
    }
}