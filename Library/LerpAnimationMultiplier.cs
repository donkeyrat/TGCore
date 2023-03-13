using System.Collections;
using System.Linq;
using UnityEngine;

namespace TGCore.Library
{
    public class LerpAnimationMultiplier : MonoBehaviour
    {
        public void Lerp(float speed)
        {
            if (!animation) return;

            StartCoroutine(DoLerp(speed));
        }

        private IEnumerator DoLerp(float speed)
        {
            var t = 0f;
            var startMultipliers = animation.animations.Select(x => x.multiplier).ToArray();
            var startSpeed = animation.animationSpeed;
            var startDirections = animation.animations.Select(x => x.direction).ToArray();
            while (t < 1f)
            {
                t += Time.deltaTime;
                var clampedTime = Mathf.Clamp(t, 0f, 1f);

                if (lerpMultiplier)
                {
                    for (var i = 0; i < animation.animations.Length; i++)
                    {
                        animation.animations[i].multiplier = Mathf.Lerp(startMultipliers[i], value, clampedTime);
                    }
                }
                else if (lerpSpeed)
                {
                    animation.animationSpeed = Mathf.Lerp(startSpeed, value, clampedTime);
                }
                else if (lerpDirection)
                {
                    for (var i = 0; i < animation.animations.Length; i++)
                    {
                        animation.animations[i].direction = Vector3.Lerp(startDirections[i], Vector3.one * value, clampedTime);
                    }
                }

                yield return null;
            }
        }

        public bool lerpMultiplier;
        public bool lerpSpeed;
        public bool lerpDirection;

        public CodeAnimation animation;

        public float value;
    }
}