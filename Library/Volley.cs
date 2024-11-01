using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class Volley : MonoBehaviour 
    {
        private void Start() 
        {
            Spawn = GetComponent<SpawnObject>();
            TeamHolder = GetComponent<TeamHolder>();
            
            SetTarget();
            StartCoroutine(SpawnArrows());
        }
    	
        private IEnumerator SpawnArrows() 
        {
            yield return new WaitForSeconds(delay);
    		
            for (var l = 0; l < totalSpawns; l++) 
            {
                SetTarget();
                for (var m = 0; m < projectilesPerSpawn; m++) 
                {
                    var vector = transform.position + Vector3.up * spawnHeight + Random.insideUnitSphere * spawnRadius;
                    var direction = transform.position - vector + Random.insideUnitSphere * 1f;
                    if (Random.value > 0.1f && NearbyUnits.Count > 0) 
                    {
                        var index = Random.Range(0, NearbyUnits.Count);
                        if (NearbyUnits[index] && NearbyUnits[index].data && NearbyUnits[index].data.mainRig)
                        {
                            direction = NearbyUnits[index].data.mainRig.position + NearbyUnits[index].data.mainRig.velocity * 0.25f - vector;
                        }
                    }
    				
                    Spawn.Spawn(vector, direction);
                }
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    
        public void SetTarget() 
        {
    		
            var hits = Physics.SphereCastAll(transform.position, targetingRadius, Vector3.up, 0.1f, LayerMask.GetMask("MainRig"));
            NearbyUnits = hits
                .Select(hit => hit.transform.root.GetComponent<Unit>())
                .Where(x => x && !x.data.Dead && x.Team != TeamHolder.team)
                .OrderBy(x => (x.data.mainRig.transform.position - transform.position).magnitude)
                .Distinct()
                .ToList();
        }
    
        private List<Unit> NearbyUnits = new List<Unit>();
        private TeamHolder TeamHolder;
        private SpawnObject Spawn;
        
        public float delay = 0.3f;

        public float targetingRadius = 8f;

        public float spawnHeight = 10f;
        public float spawnRadius = 3f;
    
        public float timeBetweenSpawns = 0.05f;
        public int totalSpawns = 25;
        public int projectilesPerSpawn = 2;
    }
}