using UnityEngine;

namespace TGCore.Library
{
    public class RotationShakeCodeAnimation : MonoBehaviour
    {
        private void Start()
        {
            shake = GetComponent<RotationShake>();
            if (!animation) animation = GetComponent<CodeAnimation>();
            initialShakeAmount = shakeAmount;
        }
        
        private void Update()
        {
            shakeAmount = initialShakeAmount * animation.animationValue;
            shake.AddForce(Random.onUnitSphere * (shakeAmount * Time.deltaTime));
        }

        private RotationShake shake;

        public CodeAnimation animation;

        public float shakeAmount = 1f;
        private float initialShakeAmount = 1f;
    }
}