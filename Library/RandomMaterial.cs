using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class RandomMaterial : MonoBehaviour
    {
        private void Start()
        {
            rend = GetComponent<Renderer>();
            delay = Random.Range(minDelay, maxDelay);
            
            if (randomizeOnStart) ReplaceMaterials();
        }

        public void ReplaceMaterials()
        {
            StopAllCoroutines();
            StartCoroutine(DoMatReplacing());
        }

        private IEnumerator DoMatReplacing()
        {
            yield return new WaitForSeconds(delay);

            rend.materials[index].CopyPropertiesFromMaterial(matsToReplaceWith[Random.Range(0, matsToReplaceWith.Count)]);
        }

        private Renderer rend;

        private float delay;
        
        public bool randomizeOnStart;
        
        public int index;

        public List<Material> matsToReplaceWith;

        public float minDelay;
        public float maxDelay;
    }
}