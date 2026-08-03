using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class SpawnFacingTarget : MonoBehaviour
{
    private Unit OwnUnit;

    public GameObject objectToSpawn;
    
    private void Start()
    {
        var rootUnit = transform.root.GetComponent<Unit>();
        var teamHolder = GetComponent<TeamHolder>();
        if (rootUnit)
        {
            OwnUnit = rootUnit;
        }
        else if (teamHolder)
        {
            OwnUnit = teamHolder.spawner.GetComponent<Unit>();
        }
    }
    
    public void SpawnTowardsTargetWithoutY()
    {
        var vector = transform.forward;
        if (OwnUnit)
        {
            var targetData = OwnUnit.data.targetData;
            vector = new Vector3(targetData.mainRig.position.x - OwnUnit.data.mainRig.position.x, 0f, targetData.mainRig.position.z - OwnUnit.data.mainRig.position.z);
        }
        
        DoSpawn(transform.position, Quaternion.LookRotation(vector.normalized));
    }

    private void DoSpawn(Vector3 position, Quaternion rotation)
    {
        var spawnedObject = Instantiate(objectToSpawn, position, rotation);
        TeamHolder.AddTeamHolder(spawnedObject, OwnUnit ? OwnUnit.gameObject : null);
    }
}