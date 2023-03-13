using System.Collections;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class Effect_Disintegrate : UnitEffectBase
    {
        private void Awake()
        {
            ownUnit = transform.root.GetComponent<Unit>();
            colorHandler = ownUnit.data.GetComponent<UnitColorHandler>();
        }
        
        public override void DoEffect()
        {
            ownUnit = transform.root.GetComponent<Unit>();
            colorHandler = ownUnit.data.GetComponent<UnitColorHandler>();
            
            StartCoroutine(AddEffect());
        }

        public override void Ping()
        {
            StartCoroutine(AddEffect());
        }

        private IEnumerator AddEffect()
        {
            yield return new WaitForSeconds(effectDelay);
            
            if (!ownUnit) yield break;
            
            ownUnit.data.healthHandler.TakeDamage(damage, Vector3.zero);
            
            if (ownUnit.data.Dead || alwaysDestroy) StartCoroutine(DestroyUnit());
        }

        private IEnumerator DestroyUnit()
        {
            if (!ownUnit || destroying) yield break;
            destroying = true;
            
            destroyEvent.Invoke();
                
            yield return new WaitForSeconds(destroyDelay);
                
            colorHandler.SetMaterial(mat);

            if (destroyRoot)
            {
                if (ownUnit && ownUnit.GetComponent<GameObjectEntity>() && World.Active.GetOrCreateManager<TeamSystem>().GetTeamUnits(ownUnit.Team).Contains(ownUnit))
                {
                    World.Active.GetOrCreateManager<TeamSystem>().RemoveEntity(ownUnit.GetComponent<GameObjectEntity>().Entity, ownUnit.Team, ownUnit);
                }
                ownUnit.DestroyUnit();
            }
        }

        private Unit ownUnit;

        private UnitColorHandler colorHandler;

        private bool destroying;

        public float damage;

        public UnityEvent destroyEvent = new UnityEvent();
        
        public bool destroyRoot = true;

        public bool alwaysDestroy;

        public float destroyDelay;

        public float effectDelay = 0.01f;
        
        public Material mat;
    }
}
