using System.Linq;
using Landfall.TABS;
using Landfall.TABS.AI.Systems;
using Unity.Entities;
using UnityEngine;

namespace TGCore.Library
{
    public class AddEffectToTargetOnce : MonoBehaviour 
    {
        private void Start()
        {
            ownUnit = transform.root.GetComponent<Unit>();
        }
        
        public void Go() 
        {
            AddEffect(ownUnit.data.targetData);
        }
        
        public void GoWithRandomTarget(float radius) 
        {
            var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0.1f, LayerMask.GetMask("MainRig"));
            var foundUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != ownUnit.Team)
                .Distinct()
                .ToArray();
            
            if (foundUnits.Length > 0) AddEffect(foundUnits[Random.Range(0, foundUnits.Length)].data);
        }

        public void AddEffect(DataHandler targetData)
        {
            if (!ownUnit || !targetData || targetData.Dead) return;
            
            var existingEffect = UnitEffectBase.AddEffectToTarget(targetData.unit.gameObject, effectPrefab);
            if (!existingEffect) 
            {
                var newEffect = Instantiate(effectPrefab.gameObject, targetData.unit.transform);
                newEffect.transform.position = targetData.unit.transform.position;
                newEffect.transform.rotation = Quaternion.LookRotation(targetData.mainRig.position - ownUnit.data.mainRig.position);
                    
                TeamHolder.AddTeamHolder(newEffect, transform.root.gameObject);
                    
                existingEffect = newEffect.GetComponent<UnitEffectBase>();
                existingEffect.DoEffect();
            }
            else if (!onlyOnce) 
            {
                existingEffect.transform.rotation = Quaternion.LookRotation(targetData.mainRig.position - ownUnit.data.mainRig.position);
                existingEffect.Ping();
            }
        }

        private Unit ownUnit;
        
        public UnitEffectBase effectPrefab;

        public bool onlyOnce;
    }
}