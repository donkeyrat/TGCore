using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using Landfall.TABS.RuntimeCleanup;
using UnityEngine;

namespace TGCore.Library
{
    public class AddMountOnStart : MonoBehaviour
    {
        private void Start()
        {
            foreach (var pos in mountPositions) pos.SetActive(true);
            
            var ownUnit = GetComponent<Unit>();

            var extraObjects = new List<GameObject>();
            
            if (ownUnit.unitBlueprint && ownUnit.unitBlueprint.UnitRiders != null && ownUnit.unitBlueprint.UnitRiders.Length > 0)
            {
                var riderHolder = gameObject.AddComponent<RiderHolder>();
                var riders = new List<GameObject>();

                for (var i = 0; i < ownUnit.unitBlueprint.UnitRiders.Length; i++)
                {
                    var riderObjects = ownUnit.unitBlueprint.UnitRiders[i].Spawn(transform.position, transform.rotation,
                        ownUnit.Team, out var unit, 1f, false, false);
                    
                    if (ownUnit.CampaignUnit) unit.SetCampaignUnit();

                    riders.Add(riderObjects[0]);
                    
                    extraObjects.AddRange(riderObjects);
                    unit.gameObject.AddComponent<Mount>().EnterMount(null, ownUnit, i);
                    extraObjects.AddRange(riderObjects);
                    
                    unit.IsRider = true;
                    
                    if (ownUnit.RemoteInstanceId != 0) unit.IsRiderWithLinkedMount = true;
                }

                riderHolder.Addriders(riders);
            }

            ServiceLocator.GetService<RuntimeGarbageCollector>().AddGameObjects(extraObjects.ToArray());

            var spawnedObjects = ownUnit.spawnedObjects.ToList();
            spawnedObjects.AddRange(extraObjects);
            ownUnit.spawnedObjects = spawnedObjects.ToArray();

            var mount = GetComponent<Mount>();
            if (mount && mount.OtherData && mount.IsMounted)
            {
                var rootSpawnedObjects = mount.OtherData.unit.spawnedObjects.ToList();
                rootSpawnedObjects.AddRange(ownUnit.spawnedObjects);
                mount.OtherData.unit.spawnedObjects = rootSpawnedObjects.ToArray();
            }
        }

        public List<GameObject> mountPositions = new List<GameObject>();
    }
}