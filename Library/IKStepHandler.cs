using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library
{
    public class IKStepHandler : MonoBehaviour
    {
        private void Start()
        {
            OwnUnit = transform.root.GetComponent<Unit>();
            Anim = OwnUnit.data.GetComponent<AnimationHandler>();
            MainRig = OwnUnit.data.mainRig;
            
            if (!forwardNormal) forwardNormal = OwnUnit.data.characterForwardObject;
            
            for (var i = 0; i < followerHolder.childCount; i++)
            {
                OriginalFollowerPositions.Add(followerHolder.GetChild(i).localPosition);
            }

            for (var i = 0; i < legs.Count; i++) GroundLeg(i, 5f, 0f);
        }
    
        private void Update()
        {
            if (OwnUnit.data.Dead)
            {
                ResetLegs();
                return;
            }

            StandingCounter += Time.deltaTime;

            if (Anim.currentState == 0)
            {
                if (StandingCounter > standingDelay * 2.5f && distanceToStand <= AverageLegDistance())
                {
                    StandingCounter = 0f;
                    StartCoroutine(GroundLegsWithDelay());
                }
                return;
            }
    
            var raycast = Physics.Raycast(MainRig.position, Vector3.down, distanceFromGround, groundMask);
            var angle = Vector3.Angle(Vector3.down, -MainRig.transform.up);

            if (!raycast && followerHolder.parent != MainRig.transform || angle > maxAngleToGround)
                ResetLegs();
            else if (raycast && followerHolder.parent != OwnUnit.transform) followerHolder.SetParent(OwnUnit.transform);
    
            Counter += Time.deltaTime;

            var dist1 = legs[CurrentLegIndex].target.position;
            dist1.y = 0f;
            var dist2 = targetPositions[CurrentLegIndex].position;
            dist2.y = 0f;
            var distance = Vector3.Distance(dist1, dist2);
            if (legs[CurrentLegIndex].counter > legs[CurrentLegIndex].cooldown && distanceBetweenSteps <= distance && cooldown <= Counter)
            {
                Counter = 0f;
                
                StartCoroutine(DoStep(CurrentLegIndex));
                if (!changeLegEveryFrame) CurrentLegIndex++;
            }
            if (changeLegEveryFrame) CurrentLegIndex++;
            if (CurrentLegIndex >= legs.Count) CurrentLegIndex = 0;
        }

        private void FixedUpdate()
        {
            if (OwnUnit.data.isGrounded)
            {
                var torso = OwnUnit.data.mainRig;
                
                var fromTo = Quaternion.FromToRotation(torso.transform.up, Vector3.up);
                fromTo.ToAngleAxis(out var angle, out var axis);
                torso.AddTorque(-torso.angularVelocity * torsoDampen, ForceMode.Acceleration);
                torso.AddTorque(axis.normalized * (angle * torsoAdjust), ForceMode.Acceleration);
            }
        }

        private float AverageLegDistance()
        {
            var distances = new List<float>();
            
            for (var i = 0; i < legs.Count; i++)
            {
                var currentPos = legs[i].target.position;
                var targetPos = targetPositions[i].position;
                distances.Add(Vector3.Distance(new Vector3(currentPos.x, currentPos.y, currentPos.z), new Vector3(targetPos.x, currentPos.y, targetPos.z)));
            }

            return distances.Sum(x => x) / distances.Count;
        }
    
        private IEnumerator DoStep(int legIndex, float moveMultiplier = 1f, float speedMultiplier = 1f, float upMultiplier = 1f)
        {
            var leg = legs[legIndex];

            var extraMultiplier = OwnUnit.data.input.inputDirection.x != 0f || OwnUnit.data.input.inputDirection.z < 0f ? 0.5f : 1f;

            var targetDirection =
                (forwardNormal.TransformPoint(OwnUnit.data.input.inputDirection) - forwardNormal.position).normalized;
            var targetPos = targetPositions[legIndex].position + targetDirection * (stepDistance * extraMultiplier * moveMultiplier);
            var origin = new Vector3(targetPos.x, OwnUnit.data.mainRig.position.y, targetPos.z);

            var raycast = Physics.Raycast(origin, Vector3.down, out var hitInfo, distanceFromGround, groundMask);
            if (!raycast || Vector3.Angle(-MainRig.transform.up, Vector3.down) > maxAngleToGround)
            {
                yield break;
            }
            
            leg.stepping = true;
            leg.counter = 0f;

            var t = 0f;
            var startPos = leg.target.position;
            var endTime = stepUpCurve.keys[stepUpCurve.keys.Length - 1].time;

            while (t < endTime)
            {
                t += Time.deltaTime * stepSpeed * speedMultiplier;
                t = Mathf.Clamp(t, 0f, 1f);

                leg.target.position =
                    Vector3.Lerp(startPos, hitInfo.point + Vector3.up * stepUpCurve.Evaluate(t), t * upMultiplier);
                
                yield return null;
            }

            leg.stepping = false;
        }

        private IEnumerator GroundLegsWithDelay()
        {
            for (var i = 0; i < legs.Count; i++)
            {
                StartCoroutine(DoStep(i, 0f, standingAdjustSpeedMultiplier, standingUpMultiplier));
                yield return new WaitForSeconds(standingDelay);
            }
        }

        private void GroundLeg(int legIndex, float speed, float up)
        {
            StartCoroutine(DoStep(legIndex, 0f, speed, up));
        }
        
        private void ResetLegs()
        {
            var savedPositions = new List<Vector3>();
            for (var i = 0; i < followerHolder.childCount; i++)
            {
                savedPositions.Add(followerHolder.GetChild(i).position);
            }
                
            followerHolder.SetParent(MainRig.transform);
            followerHolder.localPosition = Vector3.zero;
            followerHolder.localRotation = Quaternion.identity;
                
            for (var i = 0; i < followerHolder.childCount; i++)
            {
                followerHolder.GetChild(i).position = savedPositions[i];
                StartCoroutine(LerpTransformLocally(followerHolder.GetChild(i), OriginalFollowerPositions[i], 2f));
            }
        }
        
        private static IEnumerator LerpTransformLocally(Transform target, Vector3 endPos, float speed)
        {
            var t = 0f;
            var startPos = target.localPosition;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                target.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Clamp(t, 0f, 1f));
                yield return null;
            }
        }
    
        private Unit OwnUnit;
        private AnimationHandler Anim;
        private Rigidbody MainRig;
        
        private float Counter;
    
        private int CurrentLegIndex;
    
        [Header("Walking Settings")]
        
        public List<IKLeg> legs = new();
        
        public List<Transform> targetPositions = new();
        
        public Transform followerHolder;
        private List<Vector3> OriginalFollowerPositions = new();
        
        public Transform forwardNormal;

        public bool changeLegEveryFrame = true;
        public float distanceBetweenSteps = 2f;
        public float maxAngleToGround = 45f;
        public float cooldown = 0.1f;
    
        [Header("Leg Settings")] 
        
        public float stepDistance;
        public AnimationCurve stepUpCurve = new AnimationCurve();
        public float stepSpeed;
        public float distanceFromGround = 1f;
    
        public LayerMask groundMask;

        [Header("Standing Settings")]

        public float standingDelay = 0.15f;

        public float standingAdjustSpeedMultiplier = 2f;
        public float standingUpMultiplier = 0.35f;

        public float distanceToStand = 1f;

        public float torsoDampen = 0.8f;
        public float torsoAdjust = 0.5f;

        private float StandingCounter;
    }
}