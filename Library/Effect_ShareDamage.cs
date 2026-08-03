using System.Collections;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using UnityEngine;

namespace TGCore.Library;

public class Effect_ShareDamage : UnitEffectBase
{
    private UnitColorHandler ColorHandler;
    private GameModeService GameModeService;
    private SettingsInstance BugUnitsDying;
    private bool ChangingColor;
    private bool Done;
    private float Counter;
    
    [HideInInspector]
    public Unit unit;
    [HideInInspector]
    public Effect_ShareDamage[] connectedUnits;

    public UnitColorInstance damageColor;
    public AnimationCurve colorCurve;

    private void Update()
    {
        if (Done)
        {
            Counter += Time.deltaTime;
            if (Counter > 1f) Destroy(gameObject);
        }
    }
    
    public override void DoEffect()
    {
        unit = transform.root.GetComponent<Unit>();
        ColorHandler = unit.data.GetComponent<UnitColorHandler>();
        GameModeService = ServiceLocator.GetService<GameModeService>();
        BugUnitsDying = ServiceLocator.GetService<GlobalSettingsHandler>().GetSettingsInstance("BUG_UNITS_NOT_DYING");
        
        
        unit.WasDealtDamageAction += SendDamageToUnits;
    }

    public override void Ping()
    {
        Debug.LogError("Share damage has been pinged??????");
    }
    
    private IEnumerator DoColor()
    {
        if (!ColorHandler) yield break;
        ChangingColor = true;
        
        var counter = 0f;
        while (counter < colorCurve.keys[colorCurve.keys.Length - 1].time)
        {
            ColorHandler.SetColor(damageColor, colorCurve.Evaluate(counter));
            counter += Time.deltaTime;
            yield return null;
        }

        ChangingColor = false;
    }

    public void SendDamageToUnits(float damage)
    {
        if (Done) return;
        foreach (var connectedUnit in connectedUnits)
        {
            if (connectedUnit != null && connectedUnit != this)
            {
                connectedUnit.ReceiveDamage(damage, damageMultiplier);
            }
        }
    }

    public void ReceiveDamage(float damage, float multiplier)
    {
        if (damage > 0f && unit.data.immunityForSeconds <= 0f && !(unit.data.lifeTime < 0.3f) && !unit.WasDamaged(null, null) && !unit.data.Dead)
        {
            var dealDamage = true;
            if (Bugs._DLC_ACTIVATED && BugUnitsDying.currentValue == 1)
            {
                dealDamage = GameModeService.IsGameModeRestricted();
            }
            if (dealDamage)
            {
                unit.data.health -= damage * multiplier;
                unit.data.health = Mathf.Clamp(unit.data.health, float.NegativeInfinity, unit.data.maxHealth);
            }
            if (unit.data.health <= 0f)
            {
                unit.data.healthHandler.Die();
            }

            if (!ChangingColor) StartCoroutine(DoColor());
        }
    }

    public void Finish()
    {
        Done = true;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
        if (ColorHandler)
        {
            ColorHandler.colors.RemoveAll(x => x.colorName == damageColor.colorName);
        }
    }
}