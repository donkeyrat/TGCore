using System.Collections;
using UnityEngine;

namespace TGCore.Library
{
    public class LineWidthOverTime : MonoBehaviour
    {
        private void Start()
        {
            Line = GetComponent<LineRenderer>();
            
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
                
                Line.widthMultiplier = widthCurve.Evaluate(t);
                yield return null;
            }
        }

        private LineRenderer Line;
        
        public AnimationCurve widthCurve;
        
        public bool playOnStart = true;
    }
}