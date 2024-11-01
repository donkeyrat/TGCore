using UnityEngine;

namespace TGCore.Library
{
    public class ExplosionAddEffectOnce : ExplosionEffect
    {
        public override void DoEffect(GameObject target)
        {
            var existingEffect = UnitEffectBase.AddEffectToTarget(target.gameObject, effectPrefab);
            if (!existingEffect && chance > Random.value)
            {
                var newEffect = Instantiate(effectPrefab.gameObject, target.transform.root);
                newEffect.transform.position = target.transform.root.position;
                newEffect.transform.rotation = Quaternion.LookRotation(target.transform.root.position - transform.position);
                
                TeamHolder.AddTeamHolder(newEffect, transform.root.gameObject);
                
                existingEffect = newEffect.GetComponent<UnitEffectBase>();
                existingEffect.DoEffect();
                
                var targetableEffects = existingEffect.gameObject.GetComponentsInChildren<TargetableEffect>();
                foreach (var targetableEffect in targetableEffects)
                {
                    targetableEffect.DoEffect(transform, target.transform);
                }
            }
            else if (!onlyOnce)
            {
                existingEffect.Ping();
            }
        }
        
        public UnitEffectBase effectPrefab;

        public bool onlyOnce;
        public bool onlyVampire;

        public float chance = 1f;
    }
}