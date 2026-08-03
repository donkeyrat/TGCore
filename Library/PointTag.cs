using Unity.Entities;
using Unity.Mathematics;

namespace TGCore.Library
{
    public struct PointTag : IComponentData
    {
        public float3 Value;
    }
}