using UnityEngine;

namespace TGCore.Library
{
    public class RotationShakeCodeAnimation : MonoBehaviour
    {
        private void Start()
        {
            Shake = GetComponent<RotationShake>();
            if (!animation) animation = GetComponent<CodeAnimation>();
            InitialShakeAmount = shakeAmount;
        }
        
        private void Update()
        {
            shakeAmount = InitialShakeAmount * animation.animationValue;
            Shake.AddForce(Random.onUnitSphere * (shakeAmount * Time.deltaTime));
        }

        private RotationShake Shake;

        public CodeAnimation animation;

        public float shakeAmount = 1f;
        private float InitialShakeAmount = 1f;
    }
}