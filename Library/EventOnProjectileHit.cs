using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class EventOnProjectileHit : MonoBehaviour
    {
        private float Counter;
    
        public UnityEvent eventToTrigger;
        public float cooldown;
        public bool startOnCooldown;

        private void Start()
        {
            if (!startOnCooldown) Counter = cooldown;
        }

        private void Update()
        {
            Counter += Time.deltaTime;
        }

        public void TriggerEvent()
        {
            if (cooldown > Counter) return;
        
            eventToTrigger.Invoke();
            Counter = 0f;
        }
    }
}