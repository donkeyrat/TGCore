using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class IKLegSystem : MonoBehaviour
{
    private Unit OwnUnit;
    private Rigidbody MainRig;
    private float Counter;
    private int CurrentLegIndex;
    
    public List<IKLegPart> legParts;
    
    [Header("Ground detection")]
    public float distanceAboveGround = 3f;
    public float stepRadius = 3f;
    public LayerMask groundLayer;
    
    [Header("Step distance")]
    public float stepDistance = 3f;
    public float distanceRequiredToStep = 3f;
    public float distanceToAdjustStep = 5f;
    
    [Header("Step speed")]
    public float adjustStepSpeed = 3f;
    public float disableSpeed = 3f;
    public float stepSpeed = 2f;
    public float randomSpeed = 0.3f;
    public float stepHeight = 2f;
    
    [Header("Step amount")]
    public int maxConsecutiveSteps = 2;

    public float stepCooldown;
    
    [Header("Angle")]
    public float maxAngle = 70f;
    public UnityEvent exceedAngleEvent;
    
    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        MainRig = OwnUnit.data.mainRig;
        
        foreach (var leg in legParts)
        {
            var sphereCast = Physics.SphereCast(leg.transform.position, stepRadius, -MainRig.transform.up, out var restHit,
                distanceAboveGround, groundLayer);
            if (sphereCast && !restHit.rigidbody)
            {
                leg.rest.position = leg.target.position = restHit.point;
                leg.SetIndicatorColor(Color.yellow);
            }
            else
            {
                leg.rest.position = leg.target.position = leg.transform.TransformPoint(new Vector3(0f, -.5f, 0f));
                leg.SetIndicatorColor(Color.white);
            }
        }
    }

    private void Update()
    {
        if (OwnUnit.data.Dead)
        {
            foreach (var deadLeg in legParts)
            {
                deadLeg.legIK.enabled = false;
            }
            Destroy(this);
        }
        if (Vector3.Angle(Vector3.up, MainRig.transform.up) > maxAngle) exceedAngleEvent.Invoke();

        foreach (var idleLeg in legParts)
        {
            var sphereCast = Physics.SphereCast(idleLeg.transform.position, stepRadius, -MainRig.transform.up, out var restHit,
                distanceAboveGround, groundLayer);
            if (sphereCast && !restHit.rigidbody)
            {
                idleLeg.rest.position = restHit.point;
                if (!idleLeg.Disabling && idleLeg.LegDisabled)
                {
                    idleLeg.SetLegDisabled(false);
                    idleLeg.TakeStep(restHit.point, disableSpeed, 0f, false);
                    idleLeg.SetIndicatorColor(Color.black);
                    idleLeg.SetStepping(true);
                }
            }
            else
            {
                idleLeg.rest.position = idleLeg.transform.TransformPoint(new Vector3(0f, -.5f, 0f));
                if (!idleLeg.LegDisabled && idleLeg.CanStopThisStep) idleLeg.DisableLeg(adjustStepSpeed);
            }
        }

        Counter += Time.deltaTime;
        
        var currentConsecutiveSteps = legParts.Sum(x => x.Stepping ? 1 : 0);
        var sortedList = legParts.OrderByDescending(x => x.Counter).ToList();

        var inputDirection = OwnUnit.Input.inputDirection;
        var distanceToStep = stepDistance;
        if (inputDirection.x != 0f || inputDirection.z < 0f) distanceToStep *= 0.25f;
        foreach (var leg in sortedList)
        {
            var sphereCast = Physics.SphereCast(
                leg.transform.position + MainRig.transform.TransformDirection(inputDirection) * distanceToStep, stepRadius,
                -MainRig.transform.up, out var hit,
                distanceAboveGround, groundLayer);
            if (sphereCast && !hit.rigidbody)
            {
                var distanceFlag = Vector3.Distance(leg.target.position, leg.rest.position) > distanceRequiredToStep;// && Vector3.Distance(leg.target.position, hit.point) > distanceRequiredToStep;
                if (distanceFlag && !leg.Disabling && !leg.Stepping && !leg.IsOnCooldown() && currentConsecutiveSteps < maxConsecutiveSteps && Counter >= stepCooldown)
                {
                    leg.TakeStep(hit.point, Random.Range(stepSpeed-randomSpeed, stepSpeed+randomSpeed), stepHeight, true);
                    Counter = 0f;
                    leg.SetIndicatorColor(Color.blue);
                }
                else if (!leg.Disabling && leg.DistanceChanged(leg.target.position, distanceToAdjustStep))
                {
                    leg.ChangePosition(leg.rest.position, stepHeight, adjustStepSpeed);
                    leg.SetIndicatorColor(Color.red);
                }
            }
        }

        //CurrentLegIndex++;
        //if (CurrentLegIndex >= legParts.Count) CurrentLegIndex = 0;
    }

    private void OnDrawGizmos()
    {
        if (!MainRig || !OwnUnit) return;
        foreach (var leg in legParts)
        {
            Gizmos.DrawLine(leg.transform.position, leg.transform.position + Vector3.down * distanceAboveGround);
        }
    }
}
