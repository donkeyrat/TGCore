using UnityEngine;

namespace TGCore.Library;

public class StickyWeb : MonoBehaviour
{
    public ProjectileStick stick1;
    public ProjectileStick stick2;
    public float breakForce = 30000f;

    private ConfigurableJoint Joint;

    private void Update()
    {
        if (Joint && (!stick1 || !stick2 || (stick1 && stick2 && Vector3.Distance(stick1.transform.position, stick1.transform.position) > 8f)))
        {
            Destroy(Joint);
            return;
        }
        
        if (!stick1.target || !stick2.target || Joint)
        {
            return;
        }
        Rigidbody rig1 = null;
        Rigidbody rig2 = null;
        if (stick1.targetRig)
        {
            rig1 = stick1.targetRig;
        }
        if (stick2.targetRig)
        {
            rig2 = stick2.targetRig;
        }
        if ((rig1 || rig2) && rig1 != rig2)
        {
            if (rig1)
            {
                Joint = rig1.gameObject.AddComponent<ConfigurableJoint>();
                if (rig2)
                {
                    Joint.connectedBody = rig2;
                }
                Joint.anchor = rig1.transform.InverseTransformPoint(stick2.transform.position);
            }
            else
            {
                Joint = rig2.gameObject.AddComponent<ConfigurableJoint>();
                Joint.anchor = rig2.transform.InverseTransformPoint(stick1.transform.position);
            }
            Joint.xMotion = ConfigurableJointMotion.Limited;
            Joint.yMotion = ConfigurableJointMotion.Limited;
            Joint.zMotion = ConfigurableJointMotion.Limited;
            
            var linearLimit = Joint.linearLimit;
            linearLimit.limit = 0.01f;
            Joint.linearLimit = linearLimit;
            
            var linearLimitSpring = Joint.linearLimitSpring;
            linearLimitSpring.spring = 10f;
            Joint.linearLimitSpring = linearLimitSpring;
            
            var xDrive = Joint.xDrive;
            xDrive.positionSpring = 2000f;
            xDrive.positionDamper = 100f;
            Joint.xDrive = xDrive;
            Joint.yDrive = xDrive;
            Joint.zDrive = xDrive;

            Joint.breakForce = breakForce;
            Joint.breakTorque = breakForce;
        }
        else
        {
            enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (Joint)
        {
            Destroy(Joint);
        }
    }
}