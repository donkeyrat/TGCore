using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TGCore.Library
{
    public class EnableDamageOnAllWeapons : MonoBehaviour
    {
        private List<MeleeWeapon> meleeWeapons = new List<MeleeWeapon>();

        private void Start()
        {
            meleeWeapons = transform.root.GetComponentsInChildren<MeleeWeapon>().ToList();
        }

        public void EnableDamage()
        {
            if (meleeWeapons.Count > 0)
            {
                foreach (var weapon in meleeWeapons.Where(weapon => weapon != null))
                {
                    weapon.EnableDamageOutOfSwing();
                }
            }
        }

        public void DisableDamage()
        {
            if (meleeWeapons.Count > 0)
            {
                foreach (var weapon in meleeWeapons.Where(weapon => weapon != null))
                {
                    weapon.DisableDamageOutOfSwing();
                }
            }
        }
    }

}