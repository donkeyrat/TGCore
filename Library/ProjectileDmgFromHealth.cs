using Landfall.TABS;

namespace TGCore.Library;

public class ProjectileDmgFromHealth : ProjectileHitEffect
{
    private TeamHolder TeamHolder;
    
    public float damage;
    public float healthThreshold;

    private void Start()
    {
        TeamHolder = GetComponent<TeamHolder>();
    }

    public override bool DoEffect(HitData hit)
    {
        var rootUnit = hit.transform.root.GetComponent<Unit>();
        
        if (rootUnit && rootUnit.data.health / rootUnit.data.maxHealth <= healthThreshold)
        {
            rootUnit.data.healthHandler.TakeDamage(damage, transform.forward, TeamHolder.spawner.GetComponentInParent<Unit>());
        }

        return false;
    }
}