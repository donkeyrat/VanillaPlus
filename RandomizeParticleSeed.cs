using System.Collections.Generic;
using UnityEngine;

namespace VanillaPlus
{
    public class RandomizeParticleSeed : MonoBehaviour
    {
        public void Randomize()
        {
            var seed = (uint)Random.Range(0, 999999);
            foreach (var part in parts)
            {
                part.randomSeed = seed;
                part.Play();
            }
        }

        public List<ParticleSystem> parts = new List<ParticleSystem>();
    }
}