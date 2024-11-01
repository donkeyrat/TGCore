using System;
using UnityEngine;

namespace TGCore.Library;

public class UnitPoseInstance : MonoBehaviour
{
    public enum BodyPart
    {
        Head,
        Torso,
        Hip,
        ArmLeft,
        ArmRight,
        ElbowLeft,
        ElbowRight,
        LegLeft,
        LegRight,
        KneeLeft,
        KneeRight
    }

    public BodyPart unitPart;
    public Transform posePart;
            
    [HideInInspector]
    public Rigidbody rig;
}