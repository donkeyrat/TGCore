using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class SpawnObjectWithLimit : MonoBehaviour
    {
        private void Start()
        {
            Unit = transform.root.GetComponent<Unit>();
        }
    
        public void DoSpawn()
        {
            if (TrackedLimit >= limit) return;
        
            var spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation,
                parentToMe ? transform : null);
            if (Unit)
            {
                var teamHolder = spawnedObject.GetComponent<TeamHolder>();
                if (!teamHolder) teamHolder = spawnedObject.AddComponent<TeamHolder>();
                teamHolder.team = Unit.Team;
            }
        
            TrackedLimit++;
        
            if (removeFromLimitIfObjectDestroyed)
            {
                var spawner = spawnedObject.GetComponent<UnitSpawner>();
                if (spawner)
                {
                    spawner.spawnUnitAction += delegate(GameObject unit)
                    {
                        unit.AddComponent<HasBeenDestroyed>().parent = this;
                    };
                }
                else
                {
                    spawnedObject.AddComponent<HasBeenDestroyed>().parent = this;
                }
            }
        }

        private int TrackedLimit;
        private Unit Unit;
    
        public GameObject objectToSpawn;

        public int limit = 15;
        public bool removeFromLimitIfObjectDestroyed = true;
        public bool parentToMe;

        public class HasBeenDestroyed : MonoBehaviour
        {
            private void OnDestroy()
            {
                parent.TrackedLimit--;
            }

            public SpawnObjectWithLimit parent;
        }
    }
}