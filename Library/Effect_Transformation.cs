using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Effect_Transformation : UnitEffectBase
    {
        private void Awake()
        {
            unit = transform.root.GetComponent<Unit>();
            dragHandler = unit.GetComponentInChildren<DragHandler>();
            rigHolder = unit.data.allRigs;
            colorHandler = unit.GetComponentInChildren<UnitColorHandler>();
            possess = MainCam.instance.GetComponentInParent<CameraAbilityPossess>();
            unparent = GetComponent<Unparent>();

            originalDrags = new List<Vector2>(rigHolder.AllDrags);
        }
        
        public override void DoEffect()
        {
            if (!unit.GetComponent<SpawnerBlueprintHolder>()) StartCoroutine(BeginTransformation());
        }

        public override void Ping()
        {
        }

        private IEnumerator BeginTransformation()
        {
            yield return new WaitForSeconds(SetState(TransformState.Transforming, false));
            
            if (!unit || unit.data.Dead || unit.data.immunityForSeconds > 0) yield break;
            
            var t = 0f;
            while (t < transformDelay && unit && !unit.data.Dead)
            {
                effectDealt += effectOverTime * Time.deltaTime;
                
                unit.data.healthHandler.TakeDamage(damageOverTime * Time.deltaTime, Vector3.zero);
                for (var i = 0; i < rigHolder.AllDrags.Length; i++)
                {
                    rigHolder.AllDrags[i].x += dragOverTime * Time.deltaTime;
                    rigHolder.AllDrags[i].y += dragOverTime * Time.deltaTime;
                }
                if (dragHandler) dragHandler.UpdateDrag();
                colorHandler.SetColor(color, effectDealt / (unit.data.maxHealth * percentAffectedThreshold));
                
                t += Time.deltaTime;
                yield return null;
            }

            if (!unit || unit.data.Dead) yield break;

            if (effectDealt / unit.data.maxHealth >= percentAffectedThreshold)
            {
                storedHealth = unit.data.health;
                StartCoroutine(DoTransform());
            }
            else
            {
                StartCoroutine(DontTransform());
            }
        }

        private IEnumerator DoTransform()
        {
            if (!unit || unit.data.Dead) yield break;
            
            yield return new WaitForSeconds(SetState(TransformState.Transformed));
            
            if (!unit || unit.data.Dead) yield break;
            
            var newUnit = unitToTransformInto.Spawn(unit.data.mainRig.position, unit.data.mainRig.rotation, unit.Team)[0].GetComponent<Unit>();
            newUnit.gameObject.AddComponent<SpawnerBlueprintHolder>().unitBlueprint = unit.unitBlueprint;
            
            if (possess && possess.currentUnit == unit) possess.EnterUnit(newUnit);
            foreach (var rig in newUnit.data.allRigs.AllRigs) rig.velocity = unit.data.mainRig.velocity;

            transform.SetParent(newUnit.data.mainRig.transform);
            transform.localPosition = Vector3.zero;
            
            unit.DestroyUnit();

            yield return new WaitForSeconds(revertDelay);
            
            if (!newUnit || newUnit.data.Dead) yield break;

            yield return new WaitForSeconds(SetState(TransformState.Reverted));

            if (!newUnit || newUnit.data.Dead) yield break;

            unit = newUnit.GetComponent<SpawnerBlueprintHolder>().unitBlueprint.Spawn(newUnit.data.mainRig.position, newUnit.data.mainRig.rotation, unit.Team)[0].GetComponent<Unit>();
            if (keepOldHealth) unit.data.healthHandler.TakeDamage(unit.data.maxHealth - storedHealth, Vector3.zero);

            if (unparent) unparent.Go();
            newUnit.DestroyUnit();
        }

        private IEnumerator DontTransform()
        {
            yield return new WaitForSeconds(SetState(TransformState.None, false));
            
            if (!unit || unit.data.Dead) yield break;
            
            var t = 0f;
            while (t < 1f)
            {
                for (var i = 0; i < rigHolder.AllDrags.Length; i++)
                {
                    rigHolder.AllDrags[i].x = Mathf.Lerp(rigHolder.AllDrags[i].x, originalDrags[i].x, t);
                    rigHolder.AllDrags[i].y = Mathf.Lerp(rigHolder.AllDrags[i].y, originalDrags[i].y, t);
                }
                if (dragHandler) dragHandler.UpdateDrag();
                
                colorHandler.SetColor(color, 1f - t);
                
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        public float SetState(TransformState state, bool doEvent = true)
        {
            currentState = state;
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
        
        private TransformState currentState;
        private float effectDealt;
        private float storedHealth;
        private Unit unit;
        private CameraAbilityPossess possess;
        private DragHandler dragHandler;
        private RigidbodyHolder rigHolder;
        private List<Vector2> originalDrags;
        private UnitColorHandler colorHandler;
        private Unparent unparent;

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