using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class Effect_Weakening : UnitEffectBase
    {
        public override void DoEffect()
        {
            Ping();
        }

        public override void Ping()
        {
            if (transform.root.GetComponent<Unit>().unitBlueprint.Name.Contains("One Punch Man") || transform.root.GetComponent<Unit>().unitBlueprint.Name.Contains("Seraphim"))
            {
                Destroy(gameObject);
            }
            if (transform.root.name == "AFTERIMAGE")
            {
                Destroy(gameObject);
            }
            else
            {
                var drag = transform.root.GetComponentInChildren<RigidbodyHolder>().AllDrags;
                for (var i = 0; i < drag.Length; i++)
                {
                    drag[i].x *= dragMultiplier;
                    drag[i].y *= dragMultiplier;
                }
                
                if (transform.root.GetComponentInChildren<DragHandler>()) transform.root.GetComponentInChildren<DragHandler>().UpdateDrag();
                
                if (transform.root.GetComponent<Unit>().WeaponHandler && transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon)
                {
                    transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon.internalCooldown /= speedMultiplier;
                    transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon.levelMultiplier *= damageMultiplier;
                    transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon.gameObject.FetchComponent<Level>().levelMultiplier *= damageMultiplier;
                }
                if (transform.root.GetComponent<Unit>().WeaponHandler && transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon)
                {
                    transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon.internalCooldown /= speedMultiplier;
                    transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon.levelMultiplier *= damageMultiplier;
                    transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon.gameObject.FetchComponent<Level>().levelMultiplier *= damageMultiplier;
                }
                if (transform.root.GetComponentInChildren<HoldingHandlerMulti>())
                {
                    foreach (var weapon in transform.root.GetComponentInChildren<HoldingHandlerMulti>().spawnedWeapons)
                    {
                        weapon.GetComponent<Weapon>().internalCooldown /= speedMultiplier;
                        weapon.GetComponent<Weapon>().levelMultiplier *= damageMultiplier;
                        weapon.FetchComponent<Level>().levelMultiplier *= damageMultiplier;
                    }
                }
                
                if (mat) transform.root.GetComponentInChildren<UnitColorHandler>().SetMaterial(mat);
                
                if (color.colorName != "") transform.root.GetComponentInChildren<UnitColorHandler>().SetColor(color, 1f);
            }
        }

        public void OnDestroy()
        {
            var drag = transform.root.GetComponentInChildren<RigidbodyHolder>().AllDrags;
            for (var i = 0; i < drag.Length; i++) 
            {
                drag[i].x /= dragMultiplier;
                drag[i].y /= dragMultiplier;
            }
            if (transform.root.GetComponentInChildren<DragHandler>()) {
                
                transform.root.GetComponentInChildren<DragHandler>().UpdateDrag();
            }
            if (transform.root.GetComponent<Unit>().WeaponHandler && transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon) {
                
                transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon.internalCooldown *= speedMultiplier;
                transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon.levelMultiplier /= damageMultiplier;
                transform.root.GetComponent<Unit>().WeaponHandler.rightWeapon.gameObject.FetchComponent<Level>().levelMultiplier /= damageMultiplier;
            }
            if (transform.root.GetComponent<Unit>().WeaponHandler && transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon) {
                
                transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon.internalCooldown *= speedMultiplier;
                transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon.levelMultiplier /= damageMultiplier;
                transform.root.GetComponent<Unit>().WeaponHandler.leftWeapon.gameObject.FetchComponent<Level>().levelMultiplier /= damageMultiplier;
            }
            if (transform.root.GetComponentInChildren<HoldingHandlerMulti>()) {
                
                foreach (var weapon in transform.root.GetComponentInChildren<HoldingHandlerMulti>().spawnedWeapons)
                {
                    weapon.GetComponent<Weapon>().internalCooldown *= speedMultiplier;
                    weapon.GetComponent<Weapon>().levelMultiplier /= damageMultiplier;
                    weapon.FetchComponent<Level>().levelMultiplier /= damageMultiplier;
                }
            }
        }

        public float dragMultiplier;

        public float speedMultiplier;

        public Material mat;

        public UnitColorInstance color;
    }
}
