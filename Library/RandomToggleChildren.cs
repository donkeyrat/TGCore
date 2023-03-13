using UnityEngine;

namespace TGCore.Library
{
    public class RandomToggleChildren : MonoBehaviour
    {
        public void Awake()
        {
            if (randomizeOnAwake) Randomize();
        }
        
        public void Start()
        {
            if (randomizeOnStart) Randomize();
        }

        public void Randomize()
        {
            transform.GetChild(Random.Range(0, transform.childCount)).gameObject.SetActive(toggle);
        }

        public bool toggle = true;

        public bool randomizeOnStart = true;
        public bool randomizeOnAwake;
    }
}