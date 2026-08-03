using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using TGCore.Library;

namespace TGCore.HarmonyPatches
{
    [HarmonyPatch(typeof(Mount), "LockJoints")]
    internal class MountHandPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Mount __instance, ref MountPos mountPos, ref ConfigurableJoint[] ___joints, ref DataHandler ___data)
        {
            var attachHandMount = mountPos.GetComponent<AttachHandMount>();
            if (attachHandMount)
            {
                __instance.StartCoroutine(AttachJoint(__instance, mountPos, ___joints, ___data, attachHandMount));
            }
        }

        private static IEnumerator AttachJoint(Mount __instance, MountPos mountPos, ConfigurableJoint[] ___joints, DataHandler ___data, AttachHandMount attachHandMount)
        {
            yield return new WaitForSeconds(0.1f);
            
            var jointList = new List<ConfigurableJoint>(___joints);
            
            var leftHand = ___data.leftHand.GetComponent<Rigidbody>();
            var rightHand = ___data.rightHand.GetComponent<Rigidbody>();
            ConfigurableJoint handJoint = null;
            if (attachHandMount.attachLeftHand)
            {
                handJoint = JointActions.AttachJoint(leftHand,
                    attachHandMount.rigToHold,
                    mountPos.angles, 25f * mountPos.angularJointStrength,
                    lockRot: false, lockPos: true, 100f * mountPos.jointStrength);
                jointList.Add(handJoint);
            }
            if (attachHandMount.attachRightHand)
            {
                handJoint = JointActions.AttachJoint(rightHand,
                    attachHandMount.rigToHold,
                    mountPos.angles, 25f * mountPos.angularJointStrength,
                    lockRot: false, lockPos: true, 100f * mountPos.jointStrength);
                jointList.Add(handJoint);
            }
            
            if (handJoint)
            {
                handJoint.autoConfigureConnectedAnchor = false;
                handJoint.connectedAnchor = attachHandMount.pivot.localPosition;
            }
            
            foreach (var joint in jointList)
            {
                if (!joint) continue;
                joint.breakTorque = mountPos.breakForce;
                joint.breakForce = mountPos.breakForce;
                joint.enableCollision = mountPos.EnableCollision;
            }

            __instance.SetField("joints", jointList.ToArray());
        }
    }
}