using System;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class LeftRightEvent : MonoBehaviour
{
    private HoldingHandler.HandType HandType;

    public UnityEvent awakeEvent;
    public UnityEvent leftHandEvent;
    public UnityEvent rightHandEvent;

    public UnityEvent triggerableLeftEvent;
    public UnityEvent triggerableRightEvent;
    
    private void Awake()
    {
        awakeEvent.Invoke();
    }

    private void Start()
    {
        var holdable = GetComponent<Holdable>();

        if (!holdable) return;

        var holdingHandler = transform.root.GetComponentInChildren<HoldingHandler>();
        var holdingHandlerMulti = transform.root.GetComponentInChildren<HoldingHandlerMulti>();
        
        if (holdingHandler)
        {
            if (holdingHandler.leftObject == holdable)
            {
                HandType = HoldingHandler.HandType.Left;
                leftHandEvent.Invoke();
            }
            else if (holdingHandler.rightObject == holdable)
            {
                HandType = HoldingHandler.HandType.Right;
                rightHandEvent.Invoke();
            }
        }
        else if (holdingHandlerMulti)
        {
            if (holdingHandlerMulti.leftWeapons.Contains(gameObject))
            {
                HandType = HoldingHandler.HandType.Left;
                leftHandEvent.Invoke();
            }
            else if (holdingHandlerMulti.rightWeapons.Contains(gameObject))
            {
                HandType = HoldingHandler.HandType.Right;
                rightHandEvent.Invoke();
            }
        }
    }

    public void CallEvent()
    {
        switch (HandType)
        {
            case HoldingHandler.HandType.Left:
                triggerableLeftEvent.Invoke();
                break;
            case HoldingHandler.HandType.Right:
                triggerableRightEvent.Invoke();
                break;
        }
    }
}