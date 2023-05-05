using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library 
{
    public class WeaponSwap : MonoBehaviour 
    {
        public void Start()
        {
            unit = transform.root.GetComponent<Unit>();
            
            if (useOriginalWeapons && unit && unit.unitBlueprint)
            {
                weaponL = unit.unitBlueprint.LeftWeapon;
                weaponR = unit.unitBlueprint.RightWeapon;
            }
        }
        
        public void Swap()
        {
            if (!canSwap || transform.root.GetComponent<HoldingHandlerMulti>())
            {
                return;
            }
            
            StartCoroutine(DoSwap());
        }

        private IEnumerator DoSwap()
        {
            if (!unit || !unit.holdingHandler || !unit.WeaponHandler)
            {
                yield break;
            }
            
            beforeSwapEvent.Invoke();

            var weapons = new List<Weapon>();
            if (unit.WeaponHandler.leftWeapon) weapons.Add(unit.WeaponHandler.leftWeapon);
            if (unit.WeaponHandler.rightWeapon) weapons.Add(unit.WeaponHandler.rightWeapon);
            foreach (var weapon in weapons.Where(x => x.GetComponent<WeaponSwapEvent>()))
            {
                weapon.GetComponent<WeaponSwapEvent>().swapEvent.Invoke();
            }
            
            yield return new WaitForSeconds(swapDelay);

            if (!unit || !unit.holdingHandler || !unit.WeaponHandler)
            {
                yield break;
            }

            var left = false;
            var right = false;

            unit.WeaponHandler.fistRefernce = null;
            
            if (weaponToSwap == SwapType.Right || weaponToSwap == SwapType.Both) 
            {
                if (unit.holdingHandler.rightObject) 
                {
                    var dropped = unit.holdingHandler.rightObject.gameObject;
                    unit.holdingHandler.LetGoOfWeapon(dropped);
                    foreach (var mono in dropped.GetComponentsInChildren<MonoBehaviour>()) mono.StopAllCoroutines();
                    Destroy(dropped);
                }
                if (weaponR)
                {
                    var weaponRSpawned = unit.unitBlueprint.SetWeapon(unit, unit.Team, weaponR, new PropItemData(), HoldingHandler.HandType.Right, unit.data.mainRig.rotation, new List<GameObject>()).gameObject;
                    weaponRSpawned.GetComponent<Rigidbody>().mass *= unit.unitBlueprint.massMultiplier;
                    right = true;
                }
            }
            if (weaponToSwap == SwapType.Left || weaponToSwap == SwapType.Both) 
            {
                if (unit.holdingHandler.leftObject) 
                {
                    var dropped = unit.holdingHandler.leftObject.gameObject;
                    unit.holdingHandler.LetGoOfWeapon(dropped);
                    foreach (var mono in dropped.GetComponentsInChildren<MonoBehaviour>()) mono.StopAllCoroutines();
                    Destroy(dropped);
                }
                if (weaponL)
                {
                    var weaponLSpawned = unit.unitBlueprint.SetWeapon(unit, unit.Team, weaponL, new PropItemData(), HoldingHandler.HandType.Left, unit.data.mainRig.rotation, new List<GameObject>()).gameObject;
                    weaponLSpawned.GetComponent<Rigidbody>().mass *= unit.unitBlueprint.massMultiplier;
                    left = true;
                }
                    
                else if (unit.unitBlueprint.holdinigWithTwoHands) unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
            }

            if ((left && right) || right)
            {
                unit.m_AttackDistance = unit.WeaponHandler.rightWeapon.maxRange;
                unit.m_PreferedDistance = unit.WeaponHandler.rightWeapon.maxRange - 0.3f;
            }
            else if (left)
            {
                unit.m_AttackDistance = unit.WeaponHandler.leftWeapon.maxRange;
                unit.m_PreferedDistance = unit.WeaponHandler.leftWeapon.maxRange - 0.3f;
            }
            
            unit.api.UpdateECSValues();
            canSwap = false;
            
            swapEvent.Invoke();
        }

        public void Reset()
        {
            canSwap = true;
        }

        public enum SwapType 
        {
            Both,
            Right,
            Left
        }

        private Unit unit;
        
        [Header("Swap Settings")]
        
        public SwapType weaponToSwap;
        public UnityEvent beforeSwapEvent = new UnityEvent();
        public UnityEvent swapEvent = new UnityEvent();
        
        public bool canSwap;
        public float swapDelay = 0.5f;

        [Header("Weapon Settings")]
        
        public GameObject weaponR;
        public GameObject weaponL;
        public bool useOriginalWeapons;
    }
}