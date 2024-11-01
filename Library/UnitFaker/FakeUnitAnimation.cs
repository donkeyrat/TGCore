using System;
using UnityEngine;

namespace TGCore.Library.UnitFaker
{
    public class FakeUnitAnimation : MonoBehaviour
    {
        public enum RigidBodyToMove
        {
            Torso,
            AllRigs,
            This,
            Specific
        }

        public enum ForceDirection
        {
            Up,
            TowardsTarget,
            AwayFromTargetWeapon,
            CharacterForward,
            CharacterRight,
            CrossUpAndAwayFromAttacker,
            CrossUpAndTowardsUnitTarget,
            RotateTowardsTarget,
            AwayFromTargetObject,
            CrossUpAndAwayFromTargetObject,
            TowardsTargetHead,
            RotateTowardsTargetHead,
            InWalkDirection,
            RotateTowardsWalkDirection,
            RigUp,
            TowardTargetWithoutY
        }

        public float force;

        public float torque;

        public AnimationCurve forceCurve;

        public RigidBodyToMove rigidbodyToMove;

        public Rigidbody specificRig;

        public ForceDirection forceDirection;

        public bool setDirectionContinuous = true;

        public bool ignoreY = true;

        public float predictionAmount = 0.2f;

        public bool randomizeDirection;

        public bool normalize = true;

        [HideInInspector]
        public float randomMultiplier = 1f;

        public AnimationCurve randomCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    }
}