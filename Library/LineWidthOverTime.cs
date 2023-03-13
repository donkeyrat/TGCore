using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class LineWidthOverTime : MonoBehaviour
    {
        private void Start()
        {
            line = GetComponent<LineRenderer>();
            
            if (playOnStart) Go();
        }

        public void Go()
        {
            StartCoroutine(DoWidthLerp());
        }

        private IEnumerator DoWidthLerp()
        {
            var t = 0f;
            while (t < widthCurve.keys[widthCurve.keys.Length - 1].time)
            {
                t += Time.deltaTime;
                
                line.widthMultiplier = widthCurve.Evaluate(t);
                yield return null;
            }
        }

        private LineRenderer line;
        
        public AnimationCurve widthCurve;
        
        public bool playOnStart = true;
    }
}