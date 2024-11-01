using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using Landfall.TABS.AI.Components;
using Landfall.TABS.AI.Components.Tags;
using Landfall.TABS.AI.Systems;
using Landfall.TABS.GameMode;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Effect_Zombie : UnitEffectBase
    {
        private void Awake()
        {
            Unit = transform.root.GetComponent<Unit>();
        }
        
        public override void DoEffect()
        {
            Unit = transform.root.GetComponent<Unit>();
            
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
            }
            
            ApplyEffect();
            
            doEffectEvent.Invoke();
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
            if (Done && zombieType != ZombificationType.Support) return;
            
            StopCoroutine(DoLerp());
            StartCoroutine(DoLerp());
        }
    
        private IEnumerator DoLerp()
        {
            if (Done && zombieType != ZombificationType.Support) yield break;
            
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

            yield return new WaitForSeconds(0.05f);
            
            if (Done) yield break;
    
            if (CurrentProgress >= 0.5f)
            {
                Unit.data.healthHandler.willBeRewived = true;
                if (Unit.GetComponentInChildren<AddRigidbodyOnDeath>())
                    foreach (var script in Unit.GetComponentsInChildren<AddRigidbodyOnDeath>())
                    {
                        Unit.data.healthHandler.RemoveDieAction(script.Die); 
                        Destroy(script);
                    }
                if (Unit.GetComponentInChildren<SinkOnDeath>())
                    foreach (var script in Unit.GetComponentsInChildren<SinkOnDeath>())
                    {
                        Unit.data.healthHandler.RemoveDieAction(script.Sink); 
                        Destroy(script);
                    }
                if (Unit.GetComponentInChildren<RemoveJointsOnDeath>())
                    foreach (var script in Unit.GetComponentsInChildren<RemoveJointsOnDeath>())
                    {
                        Unit.data.healthHandler.RemoveDieAction(script.Die); 
                        Destroy(script);
                    }
                if (Unit.GetComponentInChildren<DisableAllSkinnedClothes>())
                    foreach (var script in Unit.GetComponentsInChildren<DisableAllSkinnedClothes>())
                    {
                        Destroy(script);
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
    
        public IEnumerator DoRevive()
        {
            ServiceLocator.GetService<GameModeService>().CurrentGameMode.OnUnitDied(Unit);
            
            Landfall.TABS.Team newTeam;
            if (zombieType == ZombificationType.Support) newTeam = Unit.data.team;
            else newTeam = Unit.data.team == Landfall.TABS.Team.Red ? Landfall.TABS.Team.Blue : Landfall.TABS.Team.Red;
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
    
            Unit.data.Dead = false;
            Unit.dead = false;
            Unit.data.hasBeenRevived = true;
            Unit.data.healthHandler.willBeRewived = false;
    
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
                    Unit.unitBlueprint.SetWeapon(Unit, newTeam, reviveWeapon, new PropItemData(), HoldingHandler.HandType.Right, Unit.data.mainRig.rotation, new List<GameObject>());
                    Unit.unitBlueprint.SetWeapon(Unit, newTeam, reviveWeapon, new PropItemData(), HoldingHandler.HandType.Left, Unit.data.mainRig.rotation, new List<GameObject>());
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
            
            if (Unit.data.GetComponent<StandingHandler>() && (Unit.name.Contains("Humanoid") || Unit.name.Contains("Stiffy") || Unit.name.Contains("Blackbeard") || Unit.name.Contains("Halfling")))
            {
                var ran = Unit.data.gameObject.AddComponent<RandomCharacterStats>();
                ran.minStandingOffset = zombieStats.GetComponent<RandomCharacterStats>().minStandingOffset;
                ran.maxStandingOffset = zombieStats.GetComponent<RandomCharacterStats>().maxStandingOffset;
                ran.minMovement = zombieStats.GetComponent<RandomCharacterStats>().minMovement;
                ran.maxMovemenmt = zombieStats.GetComponent<RandomCharacterStats>().maxMovemenmt;
                ran.randomCurve = zombieStats.GetComponent<RandomCharacterStats>().randomCurve;
            }
            
            Unit.api.SetTargetingType(Unit.unitBlueprint.TargetingComponent);
            ServiceLocator.GetService<UnitHealthbars>().HandleUnitSpawned(Unit);
            Unit.api.UpdateECSValues();
            Unit.InitializeUnit(newTeam);
    
            reviveEvent.Invoke();
        }
        
        public void Update()
        {
            if (Unit)
            {
                Unit.data.GetComponent<UnitColorHandler>().SetColor(color, LerpProgress);
            }
        }
    
        public enum ZombificationType
        {
            Standard,
            Virus,
            Support
        }
    
        private Unit Unit;
        private GameObject Weapon1;
        private GameObject Weapon2;
    
        private bool Done;
        
        [Header("Zombification Settings")]
        
        public ZombificationType zombieType;
        
        private float CurrentProgress;
    
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
        
        private float LerpProgress;
    
        public float lerpSpeed = 1f;
    }
}

