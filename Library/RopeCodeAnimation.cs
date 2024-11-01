using UnityEngine;

namespace TGCore.Library
{
    public class RopeCodeAnimation : MonoBehaviour
    {
        private void Start()
        {
            InitialNoise = noise;
        }
        
        private void Update()
        {
            noise = InitialNoise * animation.animationValue;
            rope.noise = noise;
        }

        public Rope rope;
        public CodeAnimation animation;

        public float noise = 0.5f;
        private float InitialNoise;
    }
}