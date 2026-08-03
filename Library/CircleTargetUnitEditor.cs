using Landfall.TABS.AI.Components.Modifiers;
using Unity.Entities;

namespace TGCore.Library;

public struct CircleTargetUnitEditor : IMovementComponent, IComponentData {
		
    public float minCircleDistance;
    public float maxCircleDistance;
}