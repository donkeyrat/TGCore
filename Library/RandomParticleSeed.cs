using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class RandomParticleSeed : MonoBehaviour
    {
        public void Randomize()
        {
            var seed = (uint)Random.Range(0, 999999);
            foreach (var part in particleSystems)
            {
                part.randomSeed = seed;
                part.Play();
            }
        }

        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    }
}