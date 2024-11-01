using System;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class UnitPose : MonoBehaviour
    {
        private void Start()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
            
            foreach (var pose in poses)
            {
                switch (pose.unitPart)
                {
                    case UnitPoseInstance.BodyPart.Head:
                        pose.rig = OwnUnit.data.head.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.Torso:
                        pose.rig = OwnUnit.data.torso.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.Hip:
                        pose.rig = OwnUnit.data.hip.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.ArmLeft:
                        pose.rig = OwnUnit.data.leftArm.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.ArmRight:
                        pose.rig = OwnUnit.data.rightArm.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.ElbowLeft:
                        pose.rig = OwnUnit.data.leftHand.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.ElbowRight:
                        pose.rig = OwnUnit.data.rightHand.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.LegLeft:
                        pose.rig = OwnUnit.data.legLeft.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.LegRight:
                        pose.rig = OwnUnit.data.legRight.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.KneeLeft:
                        pose.rig = OwnUnit.data.footLeft.GetComponent<Rigidbody>();
                        break;
                    case UnitPoseInstance.BodyPart.KneeRight:
                        pose.rig = OwnUnit.data.footRight.GetComponent<Rigidbody>();
                        break;
                }
            }
            
            if (beginPoseOnStart) BeginPose();
        }

        private void Update()
        {
            transform.rotation = OwnUnit.data.groundedMovementDirectionObject.rotation;
            
            if (!Pose) return;

            foreach (var pose in poses)
            {
                var forward = pose.posePart.forward;
                var up = pose.posePart.up;
                var a = -Vector3.Angle(forward, pose.rig.transform.forward) *
                        Vector3.Cross(forward, pose.rig.transform.forward).normalized;
                var b = -Vector3.Angle(up, pose.rig.transform.up) *
                        Vector3.Cross(up, pose.rig.transform.up).normalized;
                pose.rig.AddTorque(Time.deltaTime * OwnUnit.data.muscleControl * animationForce * (a + b), ForceMode.Acceleration);
                pose.rig.angularVelocity -= pose.rig.angularVelocity * (animationDrag * Time.deltaTime);
            }
        }
        
        public void BeginPose()
        {
            Pose = true;
        }
        
        public void EndPose()
        {
            Pose = true;
        }

        private Unit OwnUnit;
        private bool Pose;
        
        public List<UnitPoseInstance> poses = new();

        public bool beginPoseOnStart;

        public float animationForce;
        public float animationDrag;
    }
}