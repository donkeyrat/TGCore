using System.Collections.Generic;
using UnityEngine;

namespace TGCore.Library
{
    public class ShootPositionCycle : MonoBehaviour
    {
        public void Switch()
        {
            shootPosition.transform.localPosition = positions[currentIndex];
            currentIndex++;
            if (currentIndex == positions.Count) currentIndex = 0;
        }

        private int currentIndex;

        public Transform shootPosition;

        public List<Vector3> positions;
    }
}