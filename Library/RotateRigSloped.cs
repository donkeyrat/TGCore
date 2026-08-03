using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class RotateRigSloped : MonoBehaviour
{
    private Unit OwnUnit;
    private Rigidbody Rig;
    
    public float drag = 0.8f;
    public float torque = 15f;
    public float upTorque = 5f;
    public float upForce = 5f;
    public float maxAngle = 60f;

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
            var fromTo = Quaternion.FromToRotation(Rig.transform.up, OwnUnit.data.groundNormal);
            fromTo.ToAngleAxis(out var angle, out var axis);
            if (angle > maxAngle)
            {
                fromTo = Quaternion.FromToRotation(Rig.transform.up, Vector3.up);
                fromTo.ToAngleAxis(out angle, out axis);
            }
            Rig.AddTorque(-Rig.angularVelocity * drag, ForceMode.Acceleration);
            Rig.AddTorque(axis.normalized * (angle * torque), ForceMode.Acceleration);
            if (angle > 5f)
            {
                fromTo = Quaternion.FromToRotation(Rig.transform.up, Vector3.up);
                fromTo.ToAngleAxis(out angle, out axis);
                Rig.AddTorque(axis.normalized * (angle * upTorque), ForceMode.Acceleration);
                Rig.AddForce(Vector3.up * upForce, ForceMode.Acceleration);
            }
        }
    }
}