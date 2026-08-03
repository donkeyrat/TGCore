using System.Collections;
using UnityEngine;

namespace TGCore.Library;

public class HoldableAnimation : MonoBehaviour
{
    private Holdable Holdable;
    private HoldableDataInstance DefaultHoldingData;
    private float DefaultForceMultiplier;
    private float DefaultRotationMultiplier;
    
    public HoldableDataInstance animationHoldingData;

    public float startDelay = 0.1f;
    public float endDelay = 1f;

    public bool animateRotation;
    public bool animatePosition;
    public float forceMultiplier = 1f;
    
    private void Start()
    {
        Holdable = GetComponent<Holdable>();
        DefaultHoldingData = Holdable.holdableData;
        DefaultForceMultiplier = Holdable.pidData.holdingForceMultiplier;
        DefaultRotationMultiplier = Holdable.pidData.holdingTorqueMultiplier;
        
        if (!animateRotation)
        {
            animationHoldingData.forwardRotation = Holdable.holdableData.forwardRotation;
            animationHoldingData.upRotation = Holdable.holdableData.upRotation;
        }
        if (!animatePosition)
        {
            animationHoldingData.relativePosition = Holdable.holdableData.relativePosition;
        }
        animationHoldingData.leftHand = Holdable.holdableData.leftHand;
        animationHoldingData.rightHand = Holdable.holdableData.rightHand;
    }

    public void Play()
    {
        StopAllCoroutines();
        
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Holdable.holdableData = DefaultHoldingData;
        
        yield return new WaitForSeconds(startDelay);
        Holdable.holdableData = animationHoldingData;
        Holdable.pidData.holdingForceMultiplier = forceMultiplier;
        Holdable.pidData.holdingTorqueMultiplier = forceMultiplier;
        
        yield return new WaitForSeconds(endDelay);
        Holdable.holdableData = DefaultHoldingData;
        Holdable.pidData.holdingForceMultiplier = DefaultForceMultiplier;
        Holdable.pidData.holdingTorqueMultiplier = DefaultRotationMultiplier;
    }
}