using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class ShootPositionCycle : MonoBehaviour
    {
        public void Switch()
        {
            shootPosition.transform.localPosition = positions[CurrentIndex];
            CurrentIndex++;
            if (CurrentIndex == positions.Count) CurrentIndex = 0;
        }

        private int CurrentIndex;

        public Transform shootPosition;

        public List<Vector3> positions;
    }
}