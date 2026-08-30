using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class IKLegPart : MonoBehaviour
{
    private Unit OwnUnit;
    private Vector3 OldPosition;
    private Vector3 NewPosition;
    private float StepSpeed;
    private float StepHeight;
    private bool UseHeight = true;
    public float Counter;
    
    public bool CanStopThisStep { get; private set; }
    public bool LegDisabled { get; private set; }
    public bool Disabling { get; private set; }
    public bool ChangingPosition { get; private set; }

    public bool Stepping { get; private set; }
    
    private Color IndicatorColor;

    public Animator animator;

    public UnityEvent beginStepEvent;
    public UnityEvent endStepEvent;
    public UnityEvent disableEvent;
    public UnityEvent enableEvent;

    public LimbIK legIK;
    public float cooldown;
    public float idleHoldDistance = -0.5f;
    public float checkHeightDistance = -0.5f;

    [Header("Rig follow")] 
    public Rigidbody[] rigs;
    public Transform[] rigFollows;
    public float force = 10f;
    public float rotationForce = 10f;
    public float drag = 0.1f;
    public float disabledMultiplier = 1f;
    public float minimumControl = 0.3f;
    
    [Header("IK goals")]
    public Transform target;
    public Transform rest;
    public Renderer indicator;
    
    private void Start()
    {
        CanStopThisStep = true;
        OwnUnit = transform.root.GetComponent<Unit>();
    }
    
    private void Update()
    {
        Counter += Time.deltaTime;
        
        if (LegDisabled && !Disabling) target.transform.position = transform.TransformPoint(new Vector3(0f, idleHoldDistance, 0f));
        
        if (indicator) indicator.material.color = IndicatorColor;
    }

    private void FixedUpdate()
    {
        if (OwnUnit.data.Dead) return;
        
        var disabledFlag = LegDisabled && !Disabling;
        
        for (var i = 0; i < rigs.Length; i++)
        {
            if (!rigs[i]) continue;
             
            var muscleControl = Mathf.Clamp(OwnUnit.data.muscleControl, minimumControl, 1f);
            var forward = rigFollows[i].forward;
            var up = rigFollows[i].up;
            var vector = (0f - Vector3.Angle(forward, rigs[i].transform.forward)) * Vector3.Cross(forward, rigs[i].transform.forward).normalized;
            var vector2 = (0f - Vector3.Angle(up, rigs[i].transform.up)) * Vector3.Cross(up, rigs[i].transform.up).normalized;
        
            rigs[i].AddTorque((vector + vector2) * (Time.fixedDeltaTime * muscleControl * 200f * rotationForce * (disabledFlag ? disabledMultiplier : 1f)), ForceMode.Acceleration);
            rigs[i].angularVelocity -= rigs[i].angularVelocity * (drag * muscleControl);
        
            var position = rigFollows[i].position;
            var vector3 = position - rigs[i].transform.position;
            rigs[i].AddForce(vector3 * (200f * muscleControl * force * Time.fixedDeltaTime * (disabledFlag ? disabledMultiplier : 1f)), ForceMode.Acceleration);
            rigs[i].velocity -= rigs[i].velocity * (drag * muscleControl);
        }
    }
    
    private IEnumerator DoStep()
    {
        if (UseHeight)
        {
            var maxHeight = transform.TransformPoint(new Vector3(0f, checkHeightDistance, 0f)).y;
            var predictedHeight = OldPosition.y + StepHeight;
            if (predictedHeight >= maxHeight)
            {
                UseHeight = false;
            }
        }
        var counter = 0f;
        while (counter < 1)
        {
            counter += Time.deltaTime * StepSpeed;
            var footPosition = Vector3.Lerp(OldPosition, NewPosition, counter);

            if (UseHeight) footPosition.y += Mathf.Sin(counter * Mathf.PI) * StepHeight;
            
            target.transform.position = footPosition;
            yield return null;
        }
        endStepEvent.Invoke();
        DoneStepping();
    }
    
    private IEnumerator DoLegDisable()
    {
        var counter = 0f;
        while (counter < 1)
        {
            counter += Time.deltaTime * StepSpeed;
            var newPosition = transform.TransformPoint(new Vector3(0f, idleHoldDistance, 0f));
            target.transform.position = Vector3.Lerp(OldPosition, newPosition, counter);
            yield return null;
        }
        endStepEvent.Invoke();
        DoneDisabling();
    }

    public void TakeStep(Vector3 pos, float stepSpeed, float stepHeight, bool canBeStopped)
    {
        if (animator) animator.speed = stepSpeed;

        if (canBeStopped) beginStepEvent.Invoke();
        else enableEvent.Invoke();
        
        Stepping = true;

        CanStopThisStep = canBeStopped;
        LegDisabled = false;
        OldPosition = target.transform.position;
        NewPosition = pos;
        StepSpeed = stepSpeed;
        StepHeight = stepHeight;
        UseHeight = true;
        Counter = 0f;
        
        StartCoroutine(DoStep());
    }
    
    public void DisableLeg(float stepSpeed)
    {
        StopAllCoroutines();
        if (animator) animator.speed = stepSpeed;
        disableEvent.Invoke();

        Stepping = false;
        Disabling = true;
        LegDisabled = true;
        CanStopThisStep = true;
        
        OldPosition = target.transform.position;
        NewPosition = target.transform.position;
        UseHeight = false;
        StepSpeed = stepSpeed;
        
        StartCoroutine(DoLegDisable());
    }

    private void DoneStepping()
    {
        CanStopThisStep = true;
        Stepping = false;
        ChangingPosition = false;
        OldPosition = Vector3.zero;
        NewPosition = Vector3.zero;
        StepSpeed = 0f;
        StepHeight = 0f;
        IndicatorColor = Color.yellow;
    }
    
    private void DoneDisabling()
    {
        Disabling = false;
        LegDisabled = true;
        OldPosition = Vector3.zero;
        StepSpeed = 0f;
        StepHeight = 0f;
        IndicatorColor = Color.white;
    }

    public bool DistanceChanged(Vector3 position, float distance)
    {
        return Vector3.Distance(position, rest.transform.position) > distance;
    }

    public void ChangePosition(Vector3 pos, float stepHeight, float stepSpeed)
    {
        CanStopThisStep = true;
        OldPosition = target.transform.position;
        NewPosition = pos;
        UseHeight = false;
        StepHeight = stepHeight;
        StepSpeed = stepSpeed;
        Stepping = true;
        ChangingPosition = true;
        
        StopAllCoroutines();
        StartCoroutine(DoStep());
    }

    public void SetNewPosition(Vector3 pos)
    {
        NewPosition = pos;
    }

    public bool IsOnCooldown()
    {
        return Counter < cooldown;
    }

    public void SetIndicatorColor(Color color)
    {
        IndicatorColor = color;
    }
    
    public void SetLegDisabled(bool legDisabled)
    {
        LegDisabled = legDisabled;
    }
    
    public void SetStepping(bool stepping)
    {
        Stepping = stepping;
    }
}
