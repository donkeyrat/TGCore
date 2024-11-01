using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Effect_Transformation : UnitEffectBase
    {
        private void Awake()
        {
            Unit = transform.root.GetComponent<Unit>();
            DragHandler = Unit.GetComponentInChildren<DragHandler>();
            RigHolder = Unit.data.allRigs;
            ColorHandler = Unit.GetComponentInChildren<UnitColorHandler>();
            Possess = MainCam.instance.GetComponentInParent<CameraAbilityPossess>();
            Unparent = GetComponent<Unparent>();

            OriginalDrags = new List<Vector2>(RigHolder.AllDrags);
        }
        
        public override void DoEffect()
        {
            if (!Unit.GetComponent<SpawnerBlueprintHolder>()) StartCoroutine(BeginTransformation());
        }

        public override void Ping()
        {
        }

        private IEnumerator BeginTransformation()
        {
            yield return new WaitForSeconds(SetState(TransformState.Transforming, false));
            
            if (!Unit || Unit.data.Dead || Unit.data.immunityForSeconds > 0 || Unit.GetComponentInChildren<UnKillable>()) yield break;
            
            var t = 0f;
            while (t < transformDelay && Unit && !Unit.data.Dead)
            {
                EffectDealt += effectOverTime * Time.deltaTime;
                
                Unit.data.healthHandler.TakeDamage(damageOverTime * Time.deltaTime, Vector3.zero);
                for (var i = 0; i < RigHolder.AllDrags.Length; i++)
                {
                    RigHolder.AllDrags[i].x += dragOverTime * Time.deltaTime;
                    RigHolder.AllDrags[i].y += dragOverTime * Time.deltaTime;
                }
                if (DragHandler) DragHandler.UpdateDrag();
                ColorHandler.SetColor(color, EffectDealt / (Unit.data.maxHealth * percentAffectedThreshold));
                
                t += Time.deltaTime;
                yield return null;
            }

            if (!Unit) yield break;

            if (EffectDealt / Unit.data.maxHealth >= percentAffectedThreshold)
            {
                StoredHealth = Unit.data.health;
                StartCoroutine(DoTransform());
            }
            else
            {
                StartCoroutine(DontTransform());
            }
        }

        private IEnumerator DoTransform()
        {
            if (!Unit) yield break;
            
            yield return new WaitForSeconds(SetState(TransformState.Transformed));
            
            if (!Unit) yield break;
            
            var newUnit = unitToTransformInto.Spawn(Unit.data.mainRig.position, Unit.data.mainRig.rotation, Unit.Team)[0].GetComponent<Unit>();
            newUnit.gameObject.AddComponent<SpawnerBlueprintHolder>().unitBlueprint = Unit.unitBlueprint;
            
            if (Possess && Possess.currentUnit == Unit) Possess.EnterUnit(newUnit);
            foreach (var rig in newUnit.data.allRigs.AllRigs) rig.velocity = Unit.data.mainRig.velocity;

            transform.SetParent(newUnit.data.mainRig.transform);
            transform.localPosition = Vector3.zero;

            StoredAlive = !Unit.data.Dead;
            
            Unit.DestroyUnit();

            yield return new WaitForSeconds(revertDelay);

            if (!StoredAlive)
            {
                newUnit.data.healthHandler.Die();
            }
            
            if (!newUnit) yield break;

            yield return new WaitForSeconds(SetState(TransformState.Reverted));

            if (!newUnit) yield break;

            if (StoredAlive)
            {
                Unit = newUnit.GetComponent<SpawnerBlueprintHolder>().unitBlueprint.Spawn(newUnit.data.mainRig.position, newUnit.data.mainRig.rotation, Unit.Team)[0].GetComponent<Unit>();

                if (Possess && Possess.currentUnit == Unit) Possess.EnterUnit(Unit);
                foreach (var rig in Unit.data.allRigs.AllRigs) rig.velocity = newUnit.data.mainRig.velocity;
            }
            
            if (Unparent) Unparent.Go();
            newUnit.DestroyUnit();

            yield return new WaitUntil(() => !Unit || Unit.data.lifeTime > 0.3f || Unit.data.Dead);

            if (Unit && !Unit.data.Dead && keepOldHealth)
            {
                Unit.data.healthHandler.TakeDamage(Unit.data.maxHealth - StoredHealth, Vector3.zero);
            }
        }

        private IEnumerator DontTransform()
        {
            yield return new WaitForSeconds(SetState(TransformState.None, false));
            
            if (!Unit || Unit.data.Dead) yield break;
            
            var t = 0f;
            while (t < 1f)
            {
                for (var i = 0; i < RigHolder.AllDrags.Length; i++)
                {
                    RigHolder.AllDrags[i].x = Mathf.Lerp(RigHolder.AllDrags[i].x, OriginalDrags[i].x, t);
                    RigHolder.AllDrags[i].y = Mathf.Lerp(RigHolder.AllDrags[i].y, OriginalDrags[i].y, t);
                }
                if (DragHandler) DragHandler.UpdateDrag();
                
                ColorHandler.SetColor(color, 1f - t);
                
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        public float SetState(TransformState state, bool doEvent = true)
        {
            CurrentState = state;
            if (doEvent) globalEvent.Invoke();
            return globalDelay;
        }

        public enum TransformState
        {
            None,
            Transforming,
            Transformed,
            Reverted
        }
        
        private TransformState CurrentState;
        private float EffectDealt;
        private float StoredHealth;
        private bool StoredAlive;
        private Unit Unit;
        private CameraAbilityPossess Possess;
        private DragHandler DragHandler;
        private RigidbodyHolder RigHolder;
        private List<Vector2> OriginalDrags;
        private UnitColorHandler ColorHandler;
        private Unparent Unparent;

        public UnityEvent globalEvent = new UnityEvent();

        public float globalDelay = 0.3f;
        
        [Header("Pre-Transform Settings")]

        [Range(0f, 1f)] 
        public float percentAffectedThreshold;
        
        public float transformDelay = 3f;

        public float damageOverTime = 100f;
        
        public float effectOverTime = 100f;

        public float dragOverTime = 1f;
        
        [Header("Transform Settings")]
        
        public UnitBlueprint unitToTransformInto;

        public UnitColorInstance color = new UnitColorInstance();
        
        [Header("Revert Settings")]

        public float revertDelay = 6f;

        public bool keepOldHealth = true;
    }
}