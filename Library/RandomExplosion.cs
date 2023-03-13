using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class RandomExplosion : MonoBehaviour
    {
        public void Start()
        {
            Randomize();
        }

        public void Randomize()
        {
            if (GetComponent<SpawnObject>()) GetComponent<SpawnObject>().objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
        }

        public List<GameObject> objectsToSpawn = new List<GameObject>();
    }
}