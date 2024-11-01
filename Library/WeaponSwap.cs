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
            Unit = transform.root.GetComponent<Unit>();
            
            if (useOriginalWeapons && Unit && Unit.unitBlueprint)
            {
                weaponL = Unit.unitBlueprint.LeftWeapon;
                weaponR = Unit.unitBlueprint.RightWeapon;
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
            if (!Unit || !Unit.holdingHandler || !Unit.WeaponHandler)
            {
                yield break;
            }
            
            beforeSwapEvent.Invoke();

            var weapons = new List<Weapon>();
            if (Unit.WeaponHandler.leftWeapon) weapons.Add(Unit.WeaponHandler.leftWeapon);
            if (Unit.WeaponHandler.rightWeapon) weapons.Add(Unit.WeaponHandler.rightWeapon);
            foreach (var weapon in weapons.Where(x => x.GetComponent<WeaponSwapEvent>()))
            {
                weapon.GetComponent<WeaponSwapEvent>().swapEvent.Invoke();
            }
            
            yield return new WaitForSeconds(swapDelay);

            if (!Unit || !Unit.holdingHandler || !Unit.WeaponHandler)
            {
                yield break;
            }

            var left = false;
            var right = false;

            Unit.WeaponHandler.fistRefernce = null;
            
            if (weaponToSwap == SwapType.Right || weaponToSwap == SwapType.Both) 
            {
                if (Unit.holdingHandler.rightObject) 
                {
                    var dropped = Unit.holdingHandler.rightObject.gameObject;
                    Unit.holdingHandler.LetGoOfWeapon(dropped);
                    foreach (var mono in dropped.GetComponentsInChildren<MonoBehaviour>()) mono.StopAllCoroutines();
                    Destroy(dropped);
                }
                if (weaponR)
                {
                    var weaponRSpawned = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, weaponR, new PropItemData(), HoldingHandler.HandType.Right, Unit.data.mainRig.rotation, new List<GameObject>()).gameObject;
                    weaponRSpawned.GetComponent<Rigidbody>().mass *= Unit.unitBlueprint.massMultiplier;
                    right = true;
                }
            }
            if (weaponToSwap == SwapType.Left || weaponToSwap == SwapType.Both) 
            {
                if (Unit.holdingHandler.leftObject) 
                {
                    var dropped = Unit.holdingHandler.leftObject.gameObject;
                    Unit.holdingHandler.LetGoOfWeapon(dropped);
                    foreach (var mono in dropped.GetComponentsInChildren<MonoBehaviour>()) mono.StopAllCoroutines();
                    Destroy(dropped);
                }
                if (weaponL)
                {
                    var weaponLSpawned = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, weaponL, new PropItemData(), HoldingHandler.HandType.Left, Unit.data.mainRig.rotation, new List<GameObject>()).gameObject;
                    weaponLSpawned.GetComponent<Rigidbody>().mass *= Unit.unitBlueprint.massMultiplier;
                    left = true;
                }
                    
                else if (Unit.unitBlueprint.holdinigWithTwoHands) Unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
            }

            if ((left && right) || right)
            {
                Unit.m_AttackDistance = Unit.WeaponHandler.rightWeapon.maxRange;
                Unit.m_PreferedDistance = Unit.WeaponHandler.rightWeapon.maxRange - 0.3f;
            }
            else if (left)
            {
                Unit.m_AttackDistance = Unit.WeaponHandler.leftWeapon.maxRange;
                Unit.m_PreferedDistance = Unit.WeaponHandler.leftWeapon.maxRange - 0.3f;
            }
            
            Unit.api.UpdateECSValues();
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

        private Unit Unit;
        
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