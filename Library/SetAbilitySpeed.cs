using System.Linq;
using UnityEngine;

namespace TGCore.Library
{
    public class SetAbilitySpeed : MonoBehaviour
    {
        public void SetAttackSpeed(float multiplier)
        {
            var conditionalEvents = transform.root.GetComponentsInChildren<ConditionalEvent>().SelectMany(x => x.events);
            foreach (var conditionalEvent in conditionalEvents)
            {
                foreach (var condition in conditionalEvent.conditions)
                {
                    if (condition.conditionType == EventCondition.ConditionType.Cooldown)
                    {
                        condition.value /= multiplier;
                    }
                }
            }
        }
    }
}