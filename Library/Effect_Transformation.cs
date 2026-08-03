using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Effect_Transformation : UnitEffectBase
    {
        private float EffectDealt;
        private bool StoredAlive;
        private Unit OriginalUnit;
        private CameraAbilityPossess Possess;
        private DragHandler DragHandler;
        private RigidbodyHolder RigHolder;
        private List<Vector2> OriginalDrags;
        private UnitColorHandler ColorHandler;
        private PossesionCamera PossesionCamera;

        public UnityEvent globalEvent;

        public float globalDelay = 0.3f;
        
        [Header("Pre-Transform")]

        [Range(0f, 1f)] 
        public float percentAffectedThreshold;
        
        public float transformDelay = 3f;

        public float damageOverTime = 100f;
        
        public float effectOverTime = 100f;

        public float dragOverTime = 1f;
        
        [Header("Transform")]
        
        public UnityEvent transformEvent;
        public UnitBlueprint unitToTransformInto;
        public UnitColorInstance color = new UnitColorInstance();
        
        [Header("Revert")]
        
        public UnityEvent revertEvent;
        public float revertDelay = 6f;
        
        [Header("Hiding")]
        
        public UnityEvent hideUnitEvent;
        public UnityEvent unHideUnitEvent;
        
        public override void DoEffect()
        {
            OriginalUnit = transform.root.GetComponent<Unit>();
            DragHandler = OriginalUnit.GetComponentInChildren<DragHandler>();
            RigHolder = OriginalUnit.data.allRigs;
            ColorHandler = OriginalUnit.GetComponentInChildren<UnitColorHandler>();
            Possess = MainCam.instance.GetComponentInParent<CameraAbilityPossess>();
            PossesionCamera = OriginalUnit.GetComponentInChildren<PossesionCamera>();

            OriginalDrags = new List<Vector2>(RigHolder.AllDrags);
            
            if (!OriginalUnit.GetComponent<SpawnerBlueprintHolder>() && !OriginalUnit.data.Dead && OriginalUnit.data.immunityForSeconds <= 0 
                && !OriginalUnit.GetComponentInChildren<UnKillable>() 
                && OriginalUnit.unitType == Unit.UnitType.Meat)
            {
                StartCoroutine(BeginTransformation());
            }
        }

        public override void Ping()
        {
        }

        private IEnumerator BeginTransformation()
        {
            var t = 0f;
            while (t < transformDelay && !OriginalUnit.data.Dead)
            {
                EffectDealt += effectOverTime * Time.deltaTime;
                
                OriginalUnit.data.healthHandler.TakeDamage(damageOverTime * Time.deltaTime, Vector3.zero);
                for (var i = 0; i < RigHolder.AllDrags.Length; i++)
                {
                    RigHolder.AllDrags[i].x += dragOverTime * Time.deltaTime;
                    RigHolder.AllDrags[i].y += dragOverTime * Time.deltaTime;
                }
                if (DragHandler) DragHandler.UpdateDrag();
                ColorHandler.SetColor(color, EffectDealt / (OriginalUnit.data.maxHealth * percentAffectedThreshold));
                
                t += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(EffectDealt / OriginalUnit.data.maxHealth >= percentAffectedThreshold
                ? DoTransform()
                : DontTransform());
        }

        private IEnumerator DoTransform()
        {
            globalEvent.Invoke();
            transformEvent.Invoke();
            yield return new WaitForSeconds(globalDelay);
            
            var transformedUnit = unitToTransformInto.Spawn(OriginalUnit.data.mainRig.position, OriginalUnit.data.mainRig.rotation, OriginalUnit.Team == Team.Red ? Team.Blue : Team.Red)[0].GetComponent<Unit>();
            transformedUnit.gameObject.AddComponent<SpawnerBlueprintHolder>().unitBlueprint = OriginalUnit.unitBlueprint;
            
            if (Possess && Possess.currentUnit == OriginalUnit) Possess.EnterUnit(transformedUnit);
            foreach (var rig in transformedUnit.data.allRigs.AllRigs)
            {
                rig.velocity = OriginalUnit.data.mainRig.velocity;
            }

            StoredAlive = !OriginalUnit.data.Dead;
            
            hideUnitEvent.Invoke();
            if (PossesionCamera) PossesionCamera.gameObject.SetActive(false);
            OriginalUnit.SetHealthBarActive(false);
            
            foreach (var rig in OriginalUnit.GetComponentsInChildren<Rigidbody>())
            {
                rig.position += Vector3.up * 100f;
            }
            
            transform.SetParent(transformedUnit.data.mainRig.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            yield return new WaitForSeconds(revertDelay);
            
            if (!StoredAlive || OriginalUnit == null)
            {
                transformedUnit.data.healthHandler.Die();
            }

            globalEvent.Invoke();
            revertEvent.Invoke();
            yield return new WaitForSeconds(globalDelay);

            if (OriginalUnit == null)
            {
                transformedUnit.data.healthHandler.Die();
                transformedUnit.DestroyUnit();
                Destroy(gameObject);
                yield break;
            }

            if (Possess && Possess.currentUnit == transformedUnit) Possess.EnterUnit(OriginalUnit);
            foreach (var rig in OriginalUnit.data.allRigs.AllRigs)
            {
                if (rig) rig.velocity = transformedUnit.data.mainRig.velocity;
            }
            
            transform.SetParent(OriginalUnit.data.mainRig.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            OriginalUnit.transform.rotation = transformedUnit.data.mainRig.rotation;
            
            var werewolfVector = transformedUnit.data.mainRig.position - OriginalUnit.data.mainRig.transform.position;
            foreach (var rig in OriginalUnit.GetComponentsInChildren<Rigidbody>())
            {
                rig.position += werewolfVector;
            }

            OriginalUnit.data.sinceGrounded = 0f;
            OriginalUnit.data.fallTime = 0f;
            
            ResetTransformEffect();
            
            unHideUnitEvent.Invoke();
            if (PossesionCamera) PossesionCamera.gameObject.SetActive(true);
            if (!OriginalUnit.data.Dead || OriginalUnit.data.healthHandler.willBeRewived) OriginalUnit.SetHealthBarActive(true);

            transformedUnit.data.healthHandler.Die();
            transformedUnit.DestroyUnit();
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        private IEnumerator DontTransform()
        {
            var startingAmount = Mathf.Clamp(EffectDealt / (OriginalUnit.data.maxHealth * percentAffectedThreshold), 0f, 1f);
            var t = 0f;
            while (t < 1f)
            {
                for (var i = 0; i < RigHolder.AllDrags.Length; i++)
                {
                    RigHolder.AllDrags[i].x = Mathf.Lerp(RigHolder.AllDrags[i].x, OriginalDrags[i].x, t);
                    RigHolder.AllDrags[i].y = Mathf.Lerp(RigHolder.AllDrags[i].y, OriginalDrags[i].y, t);
                }
                if (DragHandler) DragHandler.UpdateDrag();
                
                ColorHandler.SetColor(color, startingAmount - t*startingAmount);
                
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        private void ResetTransformEffect()
        {
            for (var i = 0; i < RigHolder.AllDrags.Length; i++)
            {
                RigHolder.AllDrags[i].x = OriginalDrags[i].x;
                RigHolder.AllDrags[i].y = OriginalDrags[i].y;
            }
            if (DragHandler) DragHandler.UpdateDrag();
                
            ColorHandler.SetColor(color, 0f);
        }
    }
}