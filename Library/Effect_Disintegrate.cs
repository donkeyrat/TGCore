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
        public override void DoEffect()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
            HideRenderers = GetComponent<HideRenderers>();
            
            AddEffect();
        }

        public override void Ping()
        {
            AddEffect();
        }

        private void AddEffect()
        {
            OwnUnit.data.healthHandler.TakeDamage(damage, Vector3.zero);
            
            if (OwnUnit.data.Dead || alwaysDestroy) StartCoroutine(DestroyUnit());
        }

        private IEnumerator DestroyUnit()
        {
            if (!OwnUnit || Destroying) yield break;
            Destroying = true;
            
            destroyEvent.Invoke();
                
            yield return new WaitForSeconds(destroyDelay);
                
            HideRenderers.HideAll();

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
        private bool Destroying;
        private HideRenderers HideRenderers;

        public float damage;

        public UnityEvent destroyEvent = new UnityEvent();
        
        public bool destroyRoot = true;

        public bool alwaysDestroy;

        public float destroyDelay;
    }
}
