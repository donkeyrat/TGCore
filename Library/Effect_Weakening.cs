using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class Effect_Weakening : UnitEffectBase
    {
        private float Counter;
        private int WeakenCount;
        private Unit OwnUnit;
        private DragHandler DragHandler;
        private UnitColorHandler ColorHandler;
        private RigidbodyHolder RigHolder;
        private float CurrentColorValue;
        private Dictionary<Weapon, float> WeaponCooldownDictionary = new();
        
        public float maxDuration = 10f;
        
        public float dragPerStack = 5f;
        public float dragDecay = 1f;
        public float attackSlowPerStack = 1.25f;
        public float attackSlowDecay = 0.03f;

        public UnitColorInstance color;
        public float colorMultiplier = 0.4f;
        public float colorDecay = 0.05f;
        
        public int weakenLimit = 6;
        
        public override void DoEffect()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
            DragHandler = OwnUnit?.data.GetComponent<DragHandler>();
            ColorHandler = OwnUnit?.data.GetComponent<UnitColorHandler>();
            RigHolder = OwnUnit?.data.GetComponent<RigidbodyHolder>();
            
            if (!OwnUnit || !DragHandler || !RigHolder || OwnUnit.data.immunityForSeconds > 0f)
            {
                Destroy(gameObject);
                return;
            }

            AddWeaponsToDict();
            TriggerEffect();
        }

        public override void Ping()
        {
            if (Counter > maxDuration) return;
            AddWeaponsToDict();
            TriggerEffect();
        }

        private void TriggerEffect()
        {
            if (WeakenCount < weakenLimit)
            {
                WeakenCount++;
            }
            
            var drag = RigHolder.AllDrags;
            for (var i = 0; i < drag.Length; i++)
            {
                drag[i].x = RigHolder.defaultDrags[i].x + dragPerStack * WeakenCount;
                drag[i].y = RigHolder.defaultDrags[i].y + dragPerStack * WeakenCount;
            }
            DragHandler.UpdateDrag();

            foreach (var weapon in WeaponCooldownDictionary)
            {
                weapon.Key.internalCooldown = weapon.Value * (1 + attackSlowPerStack * WeakenCount);
            }

            CurrentColorValue = (float)WeakenCount / weakenLimit;
        }

        private void Update()
        {
            Counter += Time.deltaTime;
            if (OwnUnit.data.Dead) return;
            
            for (var i = 0; i < RigHolder.AllDrags.Length; i++)
            {
                if (RigHolder.AllDrags[i].x > RigHolder.defaultDrags[i].x)
                {
                    RigHolder.AllDrags[i].x -= Time.deltaTime * dragDecay;
                }
                if (RigHolder.AllDrags[i].y > RigHolder.defaultDrags[i].y)
                {
                    RigHolder.AllDrags[i].y -= Time.deltaTime * dragDecay;
                }
            }
            DragHandler.UpdateDrag();

            foreach (var weapon in WeaponCooldownDictionary)
            {
                if (weapon.Key.internalCooldown > weapon.Value)
                {
                    weapon.Key.internalCooldown -= Time.deltaTime * attackSlowDecay;
                }
            }
            
            if (color.colorName != "")
            {
                CurrentColorValue = Mathf.Clamp(CurrentColorValue - colorDecay * Time.deltaTime, 0f, 1f);
                ColorHandler.SetColor(color, CurrentColorValue * colorMultiplier);
            }
        }

        private void AddWeaponsToDict()
        {
            if (OwnUnit.WeaponHandler)
            {
                var rightWeapon = OwnUnit.WeaponHandler.rightWeapon;
                if (rightWeapon && !WeaponCooldownDictionary.ContainsKey(rightWeapon))
                {
                    WeaponCooldownDictionary.Add(rightWeapon, rightWeapon.internalCooldown);
                    rightWeapon.internalCooldown *= 1 + attackSlowPerStack * WeakenCount;
                }
                var leftWeapon = OwnUnit.WeaponHandler.leftWeapon;
                if (leftWeapon && !WeaponCooldownDictionary.ContainsKey(leftWeapon))
                {
                    WeaponCooldownDictionary.Add(leftWeapon, leftWeapon.internalCooldown);
                    rightWeapon.internalCooldown *= 1 + attackSlowPerStack * WeakenCount;
                }
            }
            else if (OwnUnit.data.GetComponent<HoldingHandlerMulti>())
            {
                foreach (var weaponObject in OwnUnit.data.GetComponent<HoldingHandlerMulti>().spawnedWeapons.Where(x => x && !WeaponCooldownDictionary.ContainsKey(x.GetComponent<Weapon>())))
                {
                    var weapon = weaponObject.GetComponent<Weapon>();
                    WeaponCooldownDictionary.Add(weapon, weapon.internalCooldown);
                    weapon.internalCooldown *= 1 + attackSlowPerStack * WeakenCount;
                }
            }
        }
    }
}
