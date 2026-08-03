using System.Linq;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class Effect_Tase : UnitEffectBase
{
    private float TaserEffect;
    private Unit OwnUnit;
    private RigidbodyHolder AllRigs;
    private GroundChecker[] GroundCheckers;
    private Wings[] Wings;
    private bool DoForces = true;
    
    public UnityEvent doEffectEvent;
    public UnityEvent pingEvent;
    
    public float initialTaseAmount = 2f;
    public float extraTaseAmountPerHit = 1f;
    public float maxTaseAmount = 4f;
    public float taseForceMultiplier = 1f;
    
    public override void DoEffect()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        AllRigs = OwnUnit.data.GetComponent<RigidbodyHolder>();
        GroundCheckers = OwnUnit.GetComponentsInChildren<GroundChecker>().Where(x => x.isActive).ToArray();
        Wings = OwnUnit.GetComponentsInChildren<Wings>();

        if (!OwnUnit)
        {
            Destroy(gameObject);
            return;
        }

        if (!AllRigs || OwnUnit.unitType == Unit.UnitType.Warmachine || OwnUnit.Entity.Name.Contains("Whale") ||
            OwnUnit.Entity.Name.Contains("Crab") || OwnUnit.Entity.Name.Contains("Scorpion"))
        {
            DoForces = false;
        }

        TaserEffect = initialTaseAmount;
        OwnUnit.data.fallTime = TaserEffect;
        
        foreach (var groundChecker in GroundCheckers)
        {
            groundChecker.isActive = false;
        }
        foreach (var wings in Wings)
        {
            wings.enabled = false;
        }
        
        doEffectEvent.Invoke();
    }

    public override void Ping()
    {
        TaserEffect += extraTaseAmountPerHit;
        TaserEffect = Mathf.Clamp(TaserEffect, 0f, maxTaseAmount);
        OwnUnit.data.fallTime = Mathf.Clamp(OwnUnit.data.fallTime, extraTaseAmountPerHit, OwnUnit.data.fallTime);
        pingEvent.Invoke();
    }

    public void FixedUpdate()
    {
        if (TaserEffect > 0f && !OwnUnit.data.Dead)
        {
            TaserEffect -= Time.fixedDeltaTime;
            if (DoForces)
            {
                foreach (var rig in AllRigs.AllRigs)
                {
                    if (!rig) continue;
                
                    rig.AddTorque(Random.insideUnitSphere * (100f * Mathf.Cos(Time.time * 15f) * taseForceMultiplier * Time.deltaTime), ForceMode.Force);
                    rig.AddTorque(Random.insideUnitSphere * (2000f * Mathf.Cos(Time.time * 15f) * taseForceMultiplier * Time.deltaTime), ForceMode.Acceleration);
                }
            }
        }
        else if (TaserEffect <= 0f)
        {
            Destroy(gameObject);
            if (OwnUnit.data.Dead) return;
            
            foreach (var groundChecker in GroundCheckers)
            {
                groundChecker.isActive = true;
            }
            foreach (var wings in Wings)
            {
                wings.enabled = true;
            }
        }
    }
}