using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Landfall.TABS;
using Landfall.TABS.AI.Components;
using Landfall.TABS.AI.Components.Tags;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.GameMode;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Team = Landfall.TABS.Team;

namespace TGCore.Library
{
    public class Effect_Zombie : UnitEffectBase
    {
        public enum ZombificationType
        {
            Standard,
            Virus,
            Support
        }
    
        private Unit Unit;
        private UnitColorHandler ColorHandler;
        private GameObject Weapon1;
        private GameObject Weapon2;
    
        private bool Done;
        private float CurrentProgress;
        private float LerpProgress;
        
        [Header("Zombification Settings")]
        
        public ZombificationType zombieType;
    
        public float progressToAdd = 100f;
    
        [Header("Revive Settings")] 
        
        public UnityEvent killEvent;
        public UnityEvent reviveEvent;
        
        public float reviveDelay;
    
        [Range(0f, 1f)]
        public float reviveHealthMultiplier = 0.5f;
    
        public float reviveTargetingPriority = 0.2f;
    
        public GameObject reviveWeapon;
    
        public List<GameObject> reviveAbilities = new List<GameObject>();
        
        public GameObject zombieStats;
        
        [Header("Effect Settings")] 
        
        public UnityEvent doEffectEvent;
        public UnityEvent pingEvent;
    
        [Header("Color Settings")] 
        
        public UnitColorInstance color = new UnitColorInstance();
    
        public float lerpSpeed = 1f;
        
        private void Start()
        {
            Unit = transform.root.GetComponent<Unit>();
            ColorHandler = Unit.data.GetComponent<UnitColorHandler>();
        }
        
        public override void DoEffect()
        {
            Unit = transform.root.GetComponent<Unit>();
            ColorHandler = Unit.data.GetComponent<UnitColorHandler>();
            
            if (Unit.holdingHandler)
            {
                Weapon1 = Unit.holdingHandler.rightObject ? Unit.holdingHandler.rightObject.gameObject : null;
                Weapon2 = Unit.holdingHandler.leftObject ? Unit.holdingHandler.leftObject.gameObject : null;
                if (zombieType != ZombificationType.Virus)
                {
                    if (Weapon1 && Weapon1.GetComponent<Holdable>()) Weapon1.GetComponent<Holdable>().ignoreDissarm = true;
                    if (Weapon2 && Weapon2.GetComponent<Holdable>()) Weapon2.GetComponent<Holdable>().ignoreDissarm = true;
                }
            }
            var effect = Unit.GetComponentsInChildren<UnitEffectBase>().ToList().Find(x => x.effectID == 1984);
            if ((effect && effect != this) || Unit.unitType == Unit.UnitType.Warmachine)
            {
                Destroy(gameObject);
                Unit.data.healthHandler.RemoveDieAction(Revive);
            }
            else
            {
                Unit.data.healthHandler.AddDieAction(Revive);
                ApplyEffect();
                doEffectEvent.Invoke();
            }
        }
        
        public override void Ping()
        {
            ApplyEffect();
            
            pingEvent.Invoke();
        }
    
        public void ApplyEffect()
        {
            if (Done || !Unit) return;
            
            CurrentProgress += Mathf.Clamp(progressToAdd / Unit.data.health, 0f, 1f);
            if (zombieType != ZombificationType.Support) AddLerpProgress();
            else StartCoroutine(DoZombieChecks());
        }
    
        public void AddLerpProgress()
        {
            if (Done && zombieType != ZombificationType.Support)
            {
                return;
            }
            
            StopCoroutine(DoLerp());
            StartCoroutine(DoLerp());
        }
    
        private IEnumerator DoLerp()
        {
            var c = 0f;
            var startProgress = LerpProgress;
            while (c < 1f)
            {
                c += Mathf.Clamp(Time.deltaTime * lerpSpeed, 0f, 1f);
                LerpProgress = Mathf.Lerp(startProgress, CurrentProgress, c);
                yield return null;
            }
    
            StartCoroutine(DoZombieChecks());
        }
    
        private IEnumerator DoZombieChecks()
        {
            if (Done) yield break;
            //yield return new WaitForSeconds(0.05f);
            //if (Done) yield break;
    
            if (CurrentProgress >= 0.5f)
            {
                Unit.data.healthHandler.willBeRewived = true;
                
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
    
            if (CurrentProgress >= 1f && zombieType != ZombificationType.Support)
            {
                Unit.data.healthHandler.TakeDamage(Unit.data.maxHealth, Vector3.zero, Unit, DamageType.Magic);
            }
        }
    
        public void Revive()
        {
            if (!Done && Unit.data.healthHandler.willBeRewived)
            {
                Done = true;
                StartCoroutine(DoRevive());
            }
        }
    
        private IEnumerator DoRevive()
        {
            ServiceLocator.GetService<GameModeService>().CurrentGameMode.OnUnitDied(Unit);
            
            Team newTeam;
            
            if (zombieType == ZombificationType.Support) newTeam = Unit.data.team;
            else newTeam = Unit.data.team == Team.Red ? Team.Blue : Team.Red;
            
            Unit.data.team = newTeam;
            Unit.Team = newTeam;
            
            Unit.targetingPriorityMultiplier = reviveTargetingPriority;
            
            var goe = Unit.GetComponent<GameObjectEntity>();
            goe.EntityManager.RemoveComponent<IsDead>(goe.Entity);
            goe.EntityManager.AddComponent(goe.Entity, ComponentType.Create<UnitTag>());
            goe.EntityManager.SetSharedComponentData(goe.Entity, new Landfall.TABS.AI.Components.Team
            {
                Value = (int)Unit.Team
            });
            World.Active.GetOrCreateManager<TeamSystem>().AddUnit(goe.Entity, Unit.gameObject, Unit.transform, Unit.data.mainRig, Unit.data, newTeam, Unit, false);
            
            if (zombieType == ZombificationType.Support) AddLerpProgress();
            
            killEvent.Invoke();
            
            yield return new WaitForSeconds(reviveDelay);
    
            Unit.data.ragdollControl = 1f;
            Unit.data.muscleControl = 1f;
    
            Unit.data.health = Unit.data.maxHealth * reviveHealthMultiplier;
    
            if (zombieType == ZombificationType.Virus)
            {
                if (Unit.holdingHandler)
                {
                    if (Weapon1) Weapon1.AddComponent<RemoveAfterSeconds>().shrink = true;
                    if (Weapon2) Weapon1.AddComponent<RemoveAfterSeconds>().shrink = true;
                    Unit.holdingHandler.LetGoOfAll();
                    var weapon1 = Unit.unitBlueprint.SetWeapon(Unit, newTeam, reviveWeapon, new PropItemData(), HoldingHandler.HandType.Right, Unit.data.mainRig.rotation, new List<GameObject>());
                    weapon1.rigidbody.mass *= Unit.unitBlueprint.massMultiplier;
                    var weapon2 = Unit.unitBlueprint.SetWeapon(Unit, newTeam, reviveWeapon, new PropItemData(), HoldingHandler.HandType.Left, Unit.data.mainRig.rotation, new List<GameObject>());
                    weapon2.rigidbody.mass *= Unit.unitBlueprint.massMultiplier;
                }
                else if (Unit.GetComponentInChildren<HoldingHandlerMulti>())
                {
                    var multi = Unit.GetComponentInChildren<HoldingHandlerMulti>();
                    foreach (var w in multi.spawnedWeapons)
                    {
                        w.AddComponent<RemoveAfterSeconds>().shrink = true;
                    }
                    multi.LetGoOfAll();
                    foreach (var left in multi.otherHands)
                    {
                        multi.SetWeapon(left.gameObject, Instantiate(reviveWeapon, left.transform.position, left.transform.rotation, Unit.transform));
                    }
                    foreach (var right in multi.mainHands)
                    {
                        multi.SetWeapon(right.gameObject, Instantiate(reviveWeapon, right.transform.position, right.transform.rotation, Unit.transform));
                    }
                }
            }
            else
            {
                if (Weapon1 && Weapon1.GetComponent<Holdable>()) Weapon1.GetComponent<Holdable>().ignoreDissarm = false;
                if (Weapon2 && Weapon2.GetComponent<Holdable>()) Weapon2.GetComponent<Holdable>().ignoreDissarm = false;
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
            
            if (Unit.GetComponentInChildren<TeamColor>())
            {
                foreach (var tc in Unit.GetComponentsInChildren<TeamColor>())
                {
                    tc.SetTeamColor(newTeam);
                }
            }
            
            if (Unit.data.GetComponent<StandingHandler>() && Unit.data.GetComponent<AnimationHandler>())
            {
                var ran = Unit.data.gameObject.AddComponent<RandomCharacterStats>();
                var zombieStatsToAdd = zombieStats.GetComponent<RandomCharacterStats>();
                ran.minStandingOffset = zombieStatsToAdd.minStandingOffset;
                ran.maxStandingOffset = zombieStatsToAdd.maxStandingOffset;
                ran.minMovement = zombieStatsToAdd.minMovement;
                ran.maxMovemenmt = zombieStatsToAdd.maxMovemenmt;
                ran.randomCurve = zombieStatsToAdd.randomCurve;
            }
            
            Unit.data.Dead = false;
            Unit.dead = false;
            Unit.data.hasBeenRevived = true;
            Unit.data.healthHandler.willBeRewived = false;
            
            var addRigidbodyOnDeath = Unit.GetComponentsInChildren<AddRigidbodyOnDeath>();
            if (addRigidbodyOnDeath.Length > 0)
            {
                foreach (var script in addRigidbodyOnDeath)
                {
                    Unit.data.healthHandler.AddDieAction(script.Die);
                }
            }
            
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
            
            Unit.api.SetTargetingType(Unit.unitBlueprint.TargetingComponent);
            ServiceLocator.GetService<UnitHealthbars>().HandleUnitSpawned(Unit);
            Unit.api.UpdateECSValues();
            Unit.InitializeUnit(newTeam);
    
            reviveEvent.Invoke();
        }
        
        public void Update()
        {
            if (ColorHandler)
            {
                ColorHandler.SetColor(color, LerpProgress);
            }
        }
    }
}

