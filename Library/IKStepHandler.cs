using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using RootMotion.FinalIK;
using UnityEngine;

namespace TGCore.Library
{
    public class IKStepHandler : MonoBehaviour
    {
        private void Start()
        {
            ownUnit = transform.root.GetComponent<Unit>();
            anim = ownUnit.data.GetComponent<AnimationHandler>();
            mainRig = ownUnit.data.mainRig;
            
            if (!forwardNormal) forwardNormal = ownUnit.data.characterForwardObject;
            
            for (var i = 0; i < followerHolder.childCount; i++)
            {
                originalFollowerPositions.Add(followerHolder.GetChild(i).localPosition);
            }

            for (var i = 0; i < legs.Count; i++) GroundLeg(i, 5f, 0f);
        }
    
        private void Update()
        {
            if (ownUnit.data.Dead)
            {
                ResetLegs();
                return;
            }

            standingCounter += Time.deltaTime;

            if (anim.currentState == 0)
            {
                if (standingCounter > standingDelay * 2.5f && distanceToStand <= AverageLegDistance())
                {
                    standingCounter = 0f;
                    StartCoroutine(GroundLegsWithDelay());
                }
                return;
            }
    
            var raycast = Physics.Raycast(mainRig.position, -mainRig.transform.up, distanceFromGround, groundMask);
            
            switch (raycast)
            {
                case false when followerHolder.parent != mainRig.transform:
                    ResetLegs();
                    break;
                case true when followerHolder.parent != ownUnit.transform:
                    followerHolder.SetParent(ownUnit.transform);
                    break;
            }
    
            counter += Time.deltaTime;

            var distance = Vector3.Distance(legs[currentLegIndex].leg.solver.target.position,
                targetPositions[currentLegIndex].position);
            if (legs[currentLegIndex].counter > legs[currentLegIndex].cooldown && distanceBetweenSteps <= distance && cooldown <= counter)
            {
                counter = 0f;
                
                StartCoroutine(DoStep(currentLegIndex));
            }
            currentLegIndex++;
            if (currentLegIndex >= legs.Count) currentLegIndex = 0;
        }

        private void FixedUpdate()
        {
            if (ownUnit.data.isGrounded)
            {
                var torso = ownUnit.data.mainRig;
                
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
                var currentPos = legs[i].leg.solver.target.position;
                var targetPos = targetPositions[i].position;
                distances.Add(Vector3.Distance(new Vector3(currentPos.x, currentPos.y, currentPos.z), new Vector3(targetPos.x, currentPos.y, targetPos.z)));
            }

            return distances.Sum(x => x) / distances.Count;
        }
    
        private IEnumerator DoStep(int legIndex, float moveMultiplier = 1f, float speedMultiplier = 1f, float upMultiplier = 1f)
        {
            var leg = legs[legIndex];

            var extraMultiplier = ownUnit.data.input.inputDirection.x != 0f || ownUnit.data.input.inputDirection.z < 0f ? 0.5f : 1f;

            var targetDirection =
                (forwardNormal.TransformPoint(ownUnit.data.input.inputDirection) - forwardNormal.position).normalized;
            var targetPos = targetPositions[legIndex].position + targetDirection * (stepDistance * extraMultiplier * moveMultiplier);
            var origin = new Vector3(targetPos.x, ownUnit.data.mainRig.position.y, targetPos.z);

            if (!Physics.Raycast(origin, -mainRig.transform.up, out var hitInfo, distanceFromGround, groundMask))
            {
                yield break;
            }
            
            leg.stepping = true;
            leg.counter = 0f;

            var t = 0f;
            var startPos = leg.leg.solver.target.position;
            var endTime = stepUpCurve.keys[stepUpCurve.keys.Length - 1].time;

            while (t < endTime)
            {
                t += Time.deltaTime * stepSpeed * speedMultiplier;
                t = Mathf.Clamp(t, 0f, 1f);

                leg.leg.solver.target.position =
                    Vector3.Lerp(startPos, hitInfo.point + Vector3.up * stepUpCurve.Evaluate(t), t * upMultiplier);
                
                yield return null;
            }

            leg.stepping = false;
        }

        private IEnumerator GroundLegsWithDelay()
        {
            for (var i = 0; i < legs.Count; i++)
            {
                StartCoroutine(DoStep(i, 0f, 2f, 0.35f));
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
                
            followerHolder.SetParent(mainRig.transform);
            followerHolder.localPosition = Vector3.zero;
            followerHolder.localRotation = Quaternion.identity;
                
            for (int i = 0; i < followerHolder.childCount; i++)
            {
                followerHolder.GetChild(i).position = savedPositions[i];
                StartCoroutine(LerpTransformLocally(followerHolder.GetChild(i), originalFollowerPositions[i], 2f));
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
    
        private Unit ownUnit;
        private AnimationHandler anim;
        private Rigidbody mainRig;
        
        private float counter;
    
        private int currentLegIndex;
    
        [Header("Walking Settings")]
        
        public List<IKLeg> legs = new List<IKLeg>();
        
        public List<Transform> targetPositions = new List<Transform>();
        
        public Transform followerHolder;
        private List<Vector3> originalFollowerPositions = new List<Vector3>();
        
        public Transform forwardNormal;
    
        public float distanceBetweenSteps = 2f;
        public float cooldown = 0.1f;
    
        [Header("Leg Settings")] 
        
        public float stepDistance;
        public AnimationCurve stepUpCurve = new AnimationCurve();
        public float stepSpeed;
        public float distanceFromGround = 1f;
    
        public LayerMask groundMask;

        [Header("Standing Settings")] 
        
        public float standingDelay = 0.15f;

        public float distanceToStand = 1f;

        public float torsoDampen = 0.8f;
        public float torsoAdjust = 0.5f;

        private float standingCounter;
    }
}