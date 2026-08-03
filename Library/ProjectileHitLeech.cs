using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class ProjectileHitLeech : ProjectileHitEffect
{
    private ProjectileHit ProjectileHit;
    private TeamHolder TeamHolder;

    public float healToDamageMultiplier = 0.7f;
    
    private void Start()
    {
        ProjectileHit = GetComponent<ProjectileHit>();
        TeamHolder = GetComponent<TeamHolder>();
    }
    
    public override bool DoEffect(HitData hit)
    {
        if (hit.transform.root.GetComponent<Unit>() && TeamHolder && TeamHolder.spawner)
        {
            TeamHolder.spawner.transform.root.GetComponent<Unit>().data.healthHandler.TakeDamage(-ProjectileHit.damage * healToDamageMultiplier, Vector3.zero);
        }
        return false;
    }
}