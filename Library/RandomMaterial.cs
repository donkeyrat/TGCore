using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class RandomMaterial : MonoBehaviour
    {
        private void Start()
        {
            Rend = GetComponent<Renderer>();
            Delay = Random.Range(minDelay, maxDelay);
            
            if (randomizeOnStart) ReplaceMaterials();
        }

        public void ReplaceMaterials()
        {
            StopAllCoroutines();
            StartCoroutine(DoMatReplacing());
        }

        private IEnumerator DoMatReplacing()
        {
            yield return new WaitForSeconds(Delay);

            Rend.materials[index].CopyPropertiesFromMaterial(matsToReplaceWith[Random.Range(0, matsToReplaceWith.Count)]);
        }

        private Renderer Rend;

        private float Delay;
        
        public bool randomizeOnStart;
        
        public int index;

        public List<Material> matsToReplaceWith;

        public float minDelay;
        public float maxDelay;
    }
}