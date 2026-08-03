using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class ExplosionAddFewEffects : MonoBehaviour
{
    private Unit Unit;
    private int EffectsAdded;
    
    public enum ForceDirection
    {
        Outward = 0,
        Forward = 1
    }

    public ForceDirection forceDirection;

    public LayerMask layerMask;
    public float damage;
    public float radius = 4f;
    public float force;
    public float minMassCap = 25f;
    
    public bool automatic;
    public bool ignoreTeamMates;
    public bool onlyTeamMates;
    public bool ignoreRoot = true;
    public bool onlyRoot;
    
    [Header("Effect")]
    public UnitEffectBase effect;

    public float freezeAmount;
    public bool ignoreDead;
    public int maxEffectsToAdd = 5;
    
    private void Start()
    {
        Unit = GetComponent<TeamHolder>()
            ? GetComponent<TeamHolder>().spawner.GetComponent<Unit>()
            : transform.root.GetComponent<Unit>();
        
        if (automatic)
        {
            Explode();
        }
    }

    public void Explode()
    {
        var hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, 0.1f, layerMask);
        var rigidbodies = hits
            //.Select(hit => hit.rigidbody)
            //.Select(hit => hit.transform.root.GetComponent<Unit>())
            .Where(x => x.transform && x.rigidbody && x.transform.GetComponentInParent<Unit>())
            .OrderBy(x => (x.transform.position - transform.position).magnitude)
            .Distinct()
            .ToArray();

        foreach (var hit in rigidbodies)
        {
            var hitUnit = hit.transform.GetComponentInParent<Unit>();
            if ((hitUnit.Team == Unit.Team && ignoreTeamMates) ||
                (hitUnit.Team != Unit.Team && onlyTeamMates) ||
                (hitUnit == Unit && ignoreRoot) ||
                (hitUnit != Unit && onlyRoot) ||
                (hitUnit.data.Dead && ignoreDead))
            {
                continue;
            }
            
            var rig = hit.rigidbody;
            var clampedDrag = Mathf.Clamp(rig.drag / 3f, 0.1f, 1f);
            switch (forceDirection)
            {
                case ForceDirection.Outward:
                    WilhelmPhysicsFunctions.AddAxplosionForceWithMinWeight(rig, force * clampedDrag, transform.position, radius, 
                        ForceMode.Impulse, minMassCap);
                    break;
                case ForceDirection.Forward:
                    WilhelmPhysicsFunctions.AddForceWithMinWeight(rig, force * clampedDrag * transform.forward, 
                        ForceMode.Impulse, minMassCap);
                    break;
            }
            rig.velocity *= 0.9f;
                
            var damageMultiplier = Mathf.Clamp(1f - Vector3.Distance(base.transform.position, 
                hit.collider.ClosestPoint(base.transform.position)) / radius, 0f, 1f);
            var direction = hit.collider.transform.position - transform.position;
            hitUnit.data.healthHandler.TakeDamage(damage * damageMultiplier, direction, Unit);
            
            DoEffect(hitUnit.gameObject);
        }
    }

    public void DoEffect(GameObject target)
    {
        if (maxEffectsToAdd <= EffectsAdded) return;
        
        var existingEffect = UnitEffectBase.AddEffectToTarget(target, effect);
        if (existingEffect == null)
        {
            existingEffect = Instantiate(effect, target.transform);
            existingEffect.transform.position = target.transform.position;
            //if (setEffectPosition)
            //{
            //    existingEffect.transform.SetPositionAndRotation(target.transform.position, Quaternion.LookRotation(target.transform.position - transform.position));
            //}
            TeamHolder.AddTeamHolder(existingEffect.gameObject, Unit.gameObject);
            existingEffect.DoEffect();
            EffectsAdded++;
        }
        else
        {
            if (freezeAmount > 0f) existingEffect.GetComponent<Effect_Frostbite>().coldAmount = freezeAmount;
            existingEffect.Ping();
        }
        var targetableEffects = existingEffect.gameObject.GetComponentsInChildren<TargetableEffect>();
        foreach (var targetable in targetableEffects)
        {
            targetable.DoEffect(base.transform, target.transform);
        }
    }
    
    public void ResetCounter()
    {
        EffectsAdded = 0;
    }
}