using UnityEngine;

namespace TGCore.Library
{
    public class StunConditionalEvents : MonoBehaviour
    {
        private ConditionalEvent[] Events;

        private void Start()
        {
            Events = transform.root.GetComponentsInChildren<ConditionalEvent>();
        }
    
        public void StunFor(float seconds)
        {
            foreach (var conditionalEvent in Events)
            {
                conditionalEvent.StunAllOfMyMovesFor(seconds);
            }
        }
    }
}