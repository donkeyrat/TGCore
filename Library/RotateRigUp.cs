using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class RotateRigUp : MonoBehaviour
{
    private Rigidbody Rig;
    private Unit OwnUnit;
    
    public float torsoDampen = 0.8f;
    public float torsoAdjust = 15f;
    
    private void Start()
    {
        Rig = GetComponent<Rigidbody>();
        OwnUnit = transform.root.GetComponent<Unit>();
    }

    private void FixedUpdate()
    {
        if (!Rig) return;
        
        if (OwnUnit.data.isGrounded && !OwnUnit.data.Dead)
        {
            var fromTo = Quaternion.FromToRotation(Rig.transform.up, Vector3.up);
            fromTo.ToAngleAxis(out var angle, out var axis);
            Rig.AddTorque(-Rig.angularVelocity * torsoDampen, ForceMode.Acceleration);
            Rig.AddTorque(axis.normalized * (angle * torsoAdjust), ForceMode.Acceleration);
        }
    }
    
}