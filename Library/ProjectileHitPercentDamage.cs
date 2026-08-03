using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class ProjectileHitPercentDamage : ProjectileHitEffect
{
    private Unit OwnUnit;

    private void Start()
    {
        var rootUnit = transform.root.GetComponent<Unit>();
        var teamHolder = GetComponent<TeamHolder>();
        OwnUnit = teamHolder ? teamHolder.spawner.transform.root.GetComponent<Unit>() : rootUnit;
    }
    
    public override bool DoEffect(HitData hit)
    {
        var hitUnit = hit.transform.root.GetComponent<Unit>();
        if (!hitUnit) return false;

        var enemyHealth = Mathf.Clamp(hitUnit.data.maxHealth, 0f, maxDamage / hpPercent) * hpPercent;
        
        hitUnit.data.healthHandler.TakeDamage(enemyHealth, Vector3.zero, OwnUnit);
        
        return true;
    }

    public float maxDamage = 500f;
    public float hpPercent = 0.1f;
}
