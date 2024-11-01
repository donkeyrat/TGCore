using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class ConditionalProjectileHitEvent : MonoBehaviour
    {
        private void Awake()
        {
            Hit = GetComponent<ProjectileHit>();
        }
        
        public void AddEvent()
        {
            var hitEvents = new List<HitEvents>(Hit.hitEvents)
            {
                new HitEvents
                {
                    hitEvent = hitEvent,
                    eventDelay = delay
                }
            };
            Hit.hitEvents = hitEvents.ToArray();
        }

        private ProjectileHit Hit;

        public UnityEvent hitEvent = new UnityEvent();
        public float delay;
    }
}