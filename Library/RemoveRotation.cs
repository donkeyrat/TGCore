using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class RemoveRotation : MonoBehaviour
    {
        public void Reset()
        {
            StartCoroutine(DoRotationReset());
        }

        private IEnumerator DoRotationReset()
        {
            var t = 0f;
            var startingAngle = transform.localEulerAngles;
            while (t < 1f)
            {
                t += Time.deltaTime * lerpSpeed;
                t = Mathf.Clamp(t, 0f, 1f);

                transform.localEulerAngles = Vector3.Lerp(startingAngle, Vector3.zero, t);
                yield return null;
            }
        }

        public float lerpSpeed = 1f;
    }
}