using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class SetTargetingPriority : MonoBehaviour
{
    private Unit OwnUnit;
    private float OriginalTargetingPriority;
    
    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        OriginalTargetingPriority = OwnUnit.targetingPriorityMultiplier;
    }

    public void SetPriority(float priority)
    {
        if (!OwnUnit) Start();

        OwnUnit.targetingPriorityMultiplier = priority;
        OwnUnit.api.UpdateECSValues();
    }

    public void ResetPriority()
    {
        OwnUnit.targetingPriorityMultiplier = OriginalTargetingPriority;
        OwnUnit.api.UpdateECSValues();
    }
}