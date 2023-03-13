using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TGCore.Library
{
    public class FollowLineSegments : MonoBehaviour
    {
        private void Start()
        {
            line = GetComponent<LineRenderer>();
        }
        
        private void Update()
        {
            var followerIndex = 0;
            for (var i = 0; i < line.positionCount; i++)
            {
                if (i + 1 == line.positionCount / followers.Count * (followerIndex + 1))
                {
                    var pos = line.GetPosition(i);
                    followers[followerIndex].transform.position = pos;
                    followers[followerIndex].transform.rotation =
                        Quaternion.LookRotation(line.GetPosition(i - 1) - line.GetPosition(i));
                    followerIndex++;
                }
            }
        }
        
        private LineRenderer line;
        
        public List<GameObject> followers;
    }
}