using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class FollowSpawnedUnit : MonoBehaviour
{
    private Transform Target;
    private Rigidbody Rig;

    public float force;
    public float drag;
    public float angularForce;
    public Vector3 offset;
    public Vector3 worldOffset;
    public UnitSpawner spawner;

    private void Start()
    {
        Rig = GetComponent<Rigidbody>();
        spawner.spawnUnitAction += SetTarget;
    }

    public void SetTarget(GameObject unitObject)
    {
        var unit = unitObject.GetComponent<Unit>();
        Target = unit.data.head;
    }
    
    private void FixedUpdate()
    {
        if (Target)
        {
            Rig.AddForce(force * (Target.TransformPoint(offset) + worldOffset - transform.position), ForceMode.Acceleration);
            Rig.velocity *= drag;
            Rig.angularVelocity *= drag;
            Rig.AddTorque(angularForce * Vector3.Angle(transform.forward, Target.forward) * Vector3.Cross(transform.forward, Target.forward).normalized, ForceMode.Acceleration);
        }
        else
        {
            Rig.angularVelocity *= drag;
        }
        Rig.AddTorque(angularForce * Vector3.Angle(transform.up, Vector3.up) * Vector3.Cross(transform.up, Vector3.up).normalized, ForceMode.Acceleration);
    }
}