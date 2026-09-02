using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Landfall.TABS;
using Landfall.TABS.AI;
using Landfall.TABS.GameMode;
using Unity.Entities;
using Unity.Mathematics;
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
                    //Destroy(script);
                }
            }
            
            var sinkOnDeath = Unit.GetComponentsInChildren<SinkOnDeath>();
            if (sinkOnDeath.Length > 0)
            {
                foreach (var script in sinkOnDeath)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.Sink);
                    //Destroy(script);
                }
            }
            
            var removeJointsOnDeath = Unit.GetComponentsInChildren<RemoveJointsOnDeath>();
            if (removeJointsOnDeath.Length > 0)
            {
                foreach (var script in removeJointsOnDeath)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.Die);
                    //Destroy(script);
                }
            }
            
            var deathEvents = Unit.GetComponentsInChildren<DeathEvent>();
            if (deathEvents.Length > 0)
            {
                foreach (var script in deathEvents)
                {
                    Unit.data.healthHandler.RemoveDieAction(script.Die);
                    //Destroy(script);
                }
            }
            
            var skeletonDeathAction = Unit.GetComponentsInChildren<SkeletonDeathAction>();
            if (skeletonDeathAction.Length > 0)
            {
                foreach (var script in skeletonDeathAction)
                {
                    MethodInfo method = typeof(SkeletonDeathAction).GetMethod(
                        "OnDeathAction",
                        BindingFlags.NonPublic | BindingFlags.Instance);


                    if (method != null)
                    {
                        Action handler = (Action)Delegate.CreateDelegate(
                            typeof(Action),
                            script,
                            method);
                        
                        Unit.data.healthHandler.RemoveDieAction(handler);
                    }
                    //Destroy(script);
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
            
            Unit.data.ragdollControl = 1f;
            Unit.data.muscleControl = 1f;
            
            Unit.data.health = Unit.data.maxHealth * reviveHealthMultiplier;
            
            var addRigidbodyOnDeath = Unit.GetComponentsInChildren<AddRigidbodyOnDeath>();
            if (addRigidbodyOnDeath.Length > 0)
            {
                foreach (var script in addRigidbodyOnDeath)
                {
                    Unit.data.healthHandler.AddDieAction(script.Die);
                }
            }

            if (Unit.WeaponHandler && letGoOfWeapons)
            {
                if (rightWeaponToSpawn)
                {
                    var weapon = Unit.unitBlueprint.SetWeapon(Unit, Unit.Team, rightWeaponToSpawn, new PropItemData(), HoldingHandler.HandType.Right, Unit.data.mainRig.rotation, new List<GameObject>());
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
                    else Destroy(RightWeaponOriginal);
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
                    else Destroy(LeftWeaponOriginal);
                }
            }

            if (!disableAbilitiesAfterRevive)
            {
                var conditionalEvents = Unit.GetComponentsInChildren<ConditionalEvent>();
                if (conditionalEvents.Length > 0)
                {
                    foreach (var ability in conditionalEvents.Where(x =>
                                 x.events.Length > 0 && x.events[0].conditions
                                     .Where(x => x.conditionType == EventCondition.ConditionType.UnitDeath).ToArray()
                                     .Length <= 0))
                    {
                        var field = typeof(ConditionalEvent).GetField("done", (BindingFlags)(-1));
                        field?.SetValue(ability, false);
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
            
            Unit.data.Dead = false;
            Unit.dead = false;
            Unit.data.hasBeenRevived = true;
            Unit.data.healthHandler.willBeRewived = false;
            
            var sinkOnDeath = Unit.GetComponentsInChildren<SinkOnDeath>();
            if (sinkOnDeath.Length > 0)
            {
                foreach (var script in sinkOnDeath)
                {
                    Unit.data.healthHandler.AddDieAction(script.Sink);
                }
            }
            
            var removeJointsOnDeath = Unit.GetComponentsInChildren<RemoveJointsOnDeath>();
            if (removeJointsOnDeath.Length > 0)
            {
                foreach (var script in removeJointsOnDeath)
                {
                    Unit.data.healthHandler.AddDieAction(script.Die);
                }
            }
            
            var skeletonDeathAction = Unit.GetComponentsInChildren<SkeletonDeathAction>();
            if (skeletonDeathAction.Length > 0)
            {
                foreach (var script in skeletonDeathAction)
                {
                    MethodInfo method = typeof(SkeletonDeathAction).GetMethod(
                        "OnDeathAction",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    if (method != null)
                    {
                        Action handler = (Action)Delegate.CreateDelegate(
                            typeof(Action),
                            script,
                            method);
                        Unit.data.healthHandler.AddDieAction(handler);
                    }
                }
            }
            
            var floatingProjectiles = Unit.GetComponentsInChildren<FloatingProjectiles>();
            if (floatingProjectiles.Length > 0)
            {
                foreach (var script in floatingProjectiles)
                {
                    script.SetField("Done", false);
                    var swordPoints = (ShootPosition[])script.GetField("SwordPoints");
                    var swords = new List<SpookySword>();
                    foreach (var point in swordPoints)
                    {
                        swords.Add((SpookySword)script.InvokeMethod("CreateNewSword", point.transform.position, point.transform.rotation, true));
                    }
                    script.SetField("Swords", swords);
                }
            }
            
            var swordCasting = Unit.GetComponentsInChildren<SpookySwords>();
            if (swordCasting.Length > 0)
            {
                foreach (var script in swordCasting)
                {
                    script.SetField("done", false);
                    var swordPoints = (ShootPosition[])script.GetField("swordPoints");
                    var swords = new List<SpookySword>();
                    foreach (var point in swordPoints)
                    {
                        swords.Add((SpookySword)script.InvokeMethod("CreateNewSword", point.transform.position, (quaternion)point.transform.rotation));
                    }
                    script.SetField("swords", swords);
                }
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
        
        public bool disableAbilitiesAfterRevive;
        
        public List<GameObject> reviveAbilities;
        
        public bool letGoOfWeapons;
        
        public GameObject rightWeaponToSpawn;
        
        public GameObject leftWeaponToSpawn;

        private GameObject RightWeaponOriginal;
        
        private GameObject LeftWeaponOriginal;

        public bool holdWithTwoHands;

        public float removeWeaponsAfterSeconds;
    }
}
