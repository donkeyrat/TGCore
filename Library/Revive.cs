using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Landfall.TABS;
using Landfall.TABS.AI;
using Landfall.TABS.GameMode;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Revive : MonoBehaviour
    {
        public void Start()
        {
            Unit = transform.root.GetComponent<Unit>();
            EyeSpawner = Unit.GetComponentInChildren<EyeSpawner>();
            Unit.data.healthHandler.willBeRewived = true;
            
            if (Unit.data.weaponHandler.rightWeapon != null && Unit.data.weaponHandler.rightWeapon.GetComponent<Holdable>())
            {
                RightWeaponOriginal = Unit.data.weaponHandler.rightWeapon.gameObject;
                if (!letGoOfWeapons) Unit.data.weaponHandler.rightWeapon.GetComponent<Holdable>().ignoreDissarm = true;
            }
            if (Unit.data.weaponHandler.leftWeapon != null && Unit.data.weaponHandler.leftWeapon.GetComponent<Holdable>())
            {
                LeftWeaponOriginal = Unit.data.weaponHandler.leftWeapon.gameObject;
                if (!letGoOfWeapons) Unit.data.weaponHandler.leftWeapon.GetComponent<Holdable>().ignoreDissarm = true;
            }
            
            var addRigidbodyOnDeath = Unit.GetComponentsInChildren<AddRigidbodyOnDeath>();
            if (addRigidbodyOnDeath.Length > 0)
            {
                foreach (var script in addRigidbodyOnDeath)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.Die);
                    Destroy(script);
                }
            }
            
            var sinkOnDeath = Unit.GetComponentsInChildren<SinkOnDeath>();
            if (sinkOnDeath.Length > 0)
            {
                foreach (var script in sinkOnDeath)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.Sink);
                    Destroy(script);
                }
            }
            
            var removeJointsOnDeath = Unit.GetComponentsInChildren<RemoveJointsOnDeath>();
            if (removeJointsOnDeath.Length > 0)
            {
                foreach (var script in removeJointsOnDeath)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.Die);
                    Destroy(script);
                }
            }
            
            var disableAllSkinnedClothes = Unit.GetComponentsInChildren<DisableAllSkinnedClothes>();
            if (disableAllSkinnedClothes.Length > 0)
            {
                foreach (var script in disableAllSkinnedClothes)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.DoIt);
                    Destroy(script);
                }
            }
        }

        public void DoRevive()
        {
            StartCoroutine(Revival());
        }

        public IEnumerator Revival()
        {
            var effect = Unit.GetComponentsInChildren<UnitEffectBase>().ToList().Find(x => x.effectID == 1984 || x.effectID == 1987);
            if (Unit.data.health > 0f || effect)
            {
                Unit.data.healthHandler.willBeRewived = false;
                ServiceLocator.GetService<GameModeService>().CurrentGameMode.OnUnitDied(Unit);
                Destroy(this);
                yield break;
            }
            
            preReviveEvent.Invoke();
            
            yield return new WaitForSeconds(reviveDelay);
            
            Unit.data.Dead = false;
            Unit.dead = false;
            Unit.data.hasBeenRevived = true;
            Unit.data.healthHandler.willBeRewived = false;
            
            Unit.data.ragdollControl = 1f;
            Unit.data.muscleControl = 1f;
            
            Unit.data.health = Unit.data.maxHealth * reviveHealthMultiplier;

            if (Unit.WeaponHandler && letGoOfWeapons)
            {
                if (rightWeaponToSpawn)
                {
                    var weapon = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, rightWeaponToSpawn, new PropItemData(), HoldingHandler.HandType.Right, Unit.data.mainRig.rotation, new List<GameObject>());
                    weapon.rigidbody.mass *= Unit.unitBlueprint.massMultiplier;
                    
                    if (holdWithTwoHands) Unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
                }
                else if (useWeaponsAfterRevive && RightWeaponOriginal)
                {
                    var weapon = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, RightWeaponOriginal, new PropItemData(), HoldingHandler.HandType.Right, Unit.data.mainRig.rotation, new List<GameObject>());
                    weapon.rigidbody.mass *= Unit.unitBlueprint.massMultiplier;
                    
                    if (holdWithTwoHands) Unit.holdingHandler.leftHandActivity = HoldingHandler.HandActivity.HoldingRightObject;
                }
                if (!holdWithTwoHands)
                {
                    if (leftWeaponToSpawn)
                    {
                        var weapon = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, leftWeaponToSpawn, new PropItemData(), HoldingHandler.HandType.Left, Unit.data.mainRig.rotation, new List<GameObject>());
                        weapon.rigidbody.mass *= Unit.unitBlueprint.massMultiplier;
                    }
                    else if (useWeaponsAfterRevive && LeftWeaponOriginal)
                    {
                        var weapon = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, LeftWeaponOriginal, new PropItemData(), HoldingHandler.HandType.Left, Unit.data.mainRig.rotation, new List<GameObject>());
                        weapon.rigidbody.mass *= Unit.unitBlueprint.massMultiplier;
                    }
                }

                if (RightWeaponOriginal)
                {
                    RightWeaponOriginal.transform.SetParent(null);
                    if (removeWeaponsAfterSeconds > 0f)
                    {
                        var sec = RightWeaponOriginal.AddComponent<RemoveAfterSeconds>();
                        sec.shrink = true;
                        sec.seconds = removeWeaponsAfterSeconds;
                    }
                    else if (removeWeaponsAfterSeconds < 0f) Destroy(RightWeaponOriginal);
                }
                if (LeftWeaponOriginal)
                {
                    LeftWeaponOriginal.transform.SetParent(null);
                    if (removeWeaponsAfterSeconds > 0f)
                    {
                        var sec = LeftWeaponOriginal.AddComponent<RemoveAfterSeconds>();
                        sec.shrink = true;
                        sec.seconds = removeWeaponsAfterSeconds;
                    }
                    else if (removeWeaponsAfterSeconds < 0f) Destroy(LeftWeaponOriginal);
                }
            }

            var conditionalEvents = Unit.GetComponentsInChildren<ConditionalEvent>();
            if (conditionalEvents.Length > 0)
            {
                foreach (var ability in conditionalEvents)
                {
                    var field = typeof(ConditionalEvent).GetField("done", (BindingFlags)(-1));
                    if (field != null)
                    {
                        field.SetValue(ability, false);
                    }
                }
            }
            
            foreach (var ability in reviveAbilities)
            {
                Instantiate(ability, Unit.transform.position, Unit.transform.rotation, Unit.transform);
            }
            
            if (openEyes && EyeSpawner && EyeSpawner.spawnedEyes != null) 
            {
                foreach (var eye in EyeSpawner.spawnedEyes) 
                {
                    eye.dead.SetActive(false);
                    eye.currentEyeState = GooglyEye.EyeState.Open;
                    eye.SetState(GooglyEye.EyeState.Open);
                    GooglyEyes.instance.AddEye(eye);
                }
            }
            
            if (Unit.unitBlueprint.MovementComponents != null && Unit.unitBlueprint.MovementComponents.Count > 0)
            {
                foreach (var mov in Unit.unitBlueprint.MovementComponents)
                {
                    var mi = (MethodInfo)typeof(UnitAPI).GetMethod("CreateGenericRemoveComponentData", (BindingFlags)(-1)).Invoke(Unit.api, new object[] { mov.GetType() });
                    mi.Invoke(Unit.GetComponent<GameObjectEntity>().EntityManager, new object[] { Unit.GetComponent<GameObjectEntity>().Entity });
                }
            }
            
            Unit.data.healthHandler.deathEvent.RemoveAllListeners();
            foreach (var rigidbodyOnDeath in Unit.GetComponentsInChildren<AddRigidbodyOnDeath>()) {

                Unit.data.healthHandler.RemoveDieAction(rigidbodyOnDeath.Die);
            }
            foreach (var deathEvent in Unit.GetComponentsInChildren<DeathEvent>()) {

                Unit.data.healthHandler.RemoveDieAction(deathEvent.Die);
            }
            
            ServiceLocator.GetService<UnitHealthbars>().HandleUnitSpawned(Unit);
            Unit.api.SetTargetingType(Unit.unitBlueprint.TargetingComponent);
            Unit.api.UpdateECSValues();
            Unit.InitializeUnit(Unit.Team);

            reviveEvent.Invoke();
            
            Destroy(this);
        }

        private Unit Unit;

        private EyeSpawner EyeSpawner;
        
        [Header("Revive Settings")]

        public UnityEvent preReviveEvent = new UnityEvent();

        public UnityEvent reviveEvent = new UnityEvent();

        public float reviveDelay = 4f;
        
        [Range(0f, 1f)]
        public float reviveHealthMultiplier = 0.5f;

        public bool openEyes = true;

        [Header("Weapon Settings")] 
        
        public List<GameObject> reviveAbilities;
        
        public bool letGoOfWeapons;

        public bool useWeaponsAfterRevive = true;
        
        public GameObject rightWeaponToSpawn;
        
        public GameObject leftWeaponToSpawn;

        private GameObject RightWeaponOriginal;
        
        private GameObject LeftWeaponOriginal;

        public bool holdWithTwoHands;

        public float removeWeaponsAfterSeconds;
    }
}
