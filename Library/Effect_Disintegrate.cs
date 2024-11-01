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
            OwnUnit = transform.root.GetComponent<Unit>();
            ColorHandler = OwnUnit.data.GetComponent<UnitColorHandler>();
        }
        
        public override void DoEffect()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
            ColorHandler = OwnUnit.data.GetComponent<UnitColorHandler>();
            
            StartCoroutine(AddEffect());
        }

        public override void Ping()
        {
            StartCoroutine(AddEffect());
        }

        private IEnumerator AddEffect()
        {
            yield return new WaitForSeconds(effectDelay);
            
            if (!OwnUnit) yield break;
            
            OwnUnit.data.healthHandler.TakeDamage(damage, Vector3.zero);
            
            if (OwnUnit.data.Dead || alwaysDestroy) StartCoroutine(DestroyUnit());
        }

        private IEnumerator DestroyUnit()
        {
            if (!OwnUnit || Destroying) yield break;
            Destroying = true;
            
            destroyEvent.Invoke();
                
            yield return new WaitForSeconds(destroyDelay);
                
            ColorHandler.SetMaterial(mat);

            if (destroyRoot)
            {
                if (OwnUnit && OwnUnit.GetComponent<GameObjectEntity>() && World.Active.GetOrCreateManager<TeamSystem>().GetTeamUnits(OwnUnit.Team).Contains(OwnUnit))
                {
                    World.Active.GetOrCreateManager<TeamSystem>().RemoveEntity(OwnUnit.GetComponent<GameObjectEntity>().Entity, OwnUnit.Team, OwnUnit);
                }
                OwnUnit.DestroyUnit();
            }
        }

        private Unit OwnUnit;

        private UnitColorHandler ColorHandler;

        private bool Destroying;

        public float damage;

        public UnityEvent destroyEvent = new UnityEvent();
        
        public bool destroyRoot = true;

        public bool alwaysDestroy;

        public float destroyDelay;

        public float effectDelay = 0.01f;
        
        public Material mat;
    }
}
