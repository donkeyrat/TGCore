using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TGCore.Library
{
    public class EnableDamageOnAllWeapons : MonoBehaviour
    {
        private List<MeleeWeapon> MeleeWeapons = new List<MeleeWeapon>();

        private void Start()
        {
            MeleeWeapons = transform.root.GetComponentsInChildren<MeleeWeapon>().ToList();
        }

        public void EnableDamage()
        {
            if (MeleeWeapons.Count > 0)
            {
                foreach (var weapon in MeleeWeapons.Where(weapon => weapon != null))
                {
                    weapon.EnableDamageOutOfSwing();
                }
            }
        }

        public void DisableDamage()
        {
            if (MeleeWeapons.Count > 0)
            {
                foreach (var weapon in MeleeWeapons.Where(weapon => weapon != null))
                {
                    weapon.DisableDamageOutOfSwing();
                }
            }
        }
    }

}