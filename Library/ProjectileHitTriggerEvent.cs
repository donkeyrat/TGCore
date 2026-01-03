namespace TGCore.Library
{
    public class ProjectileHitAbilityEvent : ProjectileHitEffect
    {
        private TeamHolder TeamHolder;
        private void Start()
        {
            TeamHolder = GetComponent<TeamHolder>();
        }
    
        public override bool DoEffect(HitData hit)
        {
            if (!TeamHolder || !TeamHolder.spawner) return false;

            foreach (var hitEvent in TeamHolder.spawner.transform.root.GetComponentsInChildren<EventOnProjectileHit>())
            {
                hitEvent.TriggerEvent();
            }
            return false;
        }
    }
}