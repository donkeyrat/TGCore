using Unity.Entities;

namespace TGCore.Library;

public struct PointPathSettings : IComponentData
{
    public float RepathRate;

    public float CurrentRate;
}