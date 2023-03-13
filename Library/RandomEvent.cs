using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class RandomEvent : MonoBehaviour
    {
        private void Start()
        {
            if (randomizeOnStart) Randomize();
        }

        public void Randomize()
        {
            if (Random.value < randomChance) randomizedEvent.Invoke();
        }
        
        public bool randomizeOnStart;

        public UnityEvent randomizedEvent = new UnityEvent();

        public float randomChance;
    }
}