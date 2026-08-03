using UnityEngine;

namespace TGCore.Library
{
    public class StunConditionalEvents : MonoBehaviour
    {
        
        public void StunFor(float seconds)
        {
            var events = transform.root.GetComponentsInChildren<ConditionalEvent>();
            foreach (var conditionalEvent in events)
            {
                conditionalEvent.StunAllOfMyMovesFor(seconds);
            }
        }
    }
}