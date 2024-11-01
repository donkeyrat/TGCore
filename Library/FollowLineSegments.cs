using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TGCore.Library
{
    public class FollowLineSegments : MonoBehaviour
    {
        private void Start()
        {
            Line = GetComponent<LineRenderer>();
        }
        
        private void Update()
        {
            var followerIndex = 0;
            for (var i = 0; i < Line.positionCount; i++)
            {
                if (i + 1 == Line.positionCount / followers.Count * (followerIndex + 1))
                {
                    var pos = Line.GetPosition(i);
                    followers[followerIndex].transform.position = pos;
                    followers[followerIndex].transform.rotation =
                        Quaternion.LookRotation(Line.GetPosition(i - 1) - Line.GetPosition(i));
                    followerIndex++;
                }
            }
        }
        
        private LineRenderer Line;
        
        public List<GameObject> followers;
    }
}