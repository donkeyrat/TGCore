using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class SpawnedUnitRise : MonoBehaviour
{
    public void Start()
        {
            Spawner = GetComponent<UnitSpawner>();
            Spawner.spawnUnitAction += MakeRise;
        }

        public void MakeRise(GameObject spawnedUnit)
        {
            var riseOnSpawn = spawnedUnit.AddComponent<RiseOnSpawn>();
            riseOnSpawn.moveMultiplier = moveMultiplier;
            riseOnSpawn.setRigsKinematic = setRigsKinematic;
            riseOnSpawn.setArmsKinematic = setArmsKinematic;
        }

        private UnitSpawner Spawner;

        public float moveMultiplier = 25f;
        public bool setRigsKinematic = true;
        public bool setArmsKinematic;

}