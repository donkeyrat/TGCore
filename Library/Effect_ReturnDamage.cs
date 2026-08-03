using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Landfall.TABS;
using Landfall.TABS.GameMode;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class Effect_ReturnDamage : UnitEffectBase
{
     private List<UnitColorHandler> ColorHandlers =  new List<UnitColorHandler>();

    public UnityEvent returnDamageEvent;
    
    public float damageReturnPercentage = 0.5f;
    public UnitColorInstance damageColor;
    public AnimationCurve colorCurve;

    public override void DoEffect()
    {
    }

    public override void Ping()
    {
    }
    
    private IEnumerator DoColor(UnitColorHandler colorHandler)
    {
        if (!ColorHandlers.Contains(colorHandler)) ColorHandlers.Add(colorHandler);
        
        var counter = 0f;
        while (counter < colorCurve.keys[colorCurve.keys.Length - 1].time)
        {
            colorHandler.SetColor(damageColor, colorCurve.Evaluate(counter));
            counter += Time.deltaTime;
            yield return null;
        }
    }
    
    public void ReturnDamage(Unit unit, float damage, SettingsInstance m_bugUnitsDying, GameModeService gameModeService)
    {
        if (damage > 0f && unit.data.immunityForSeconds <= 0f && !(unit.data.lifeTime < 0.3f) && !unit.WasDamaged(null, null) && !unit.data.Dead)
        {
            var dealDamage = true;
            if (Bugs._DLC_ACTIVATED && m_bugUnitsDying.currentValue == 1)
            {
                dealDamage = gameModeService.IsGameModeRestricted();
            }
            if (dealDamage)
            {
                unit.data.health -= damage * damageReturnPercentage;
                unit.data.health = Mathf.Clamp(unit.data.health, float.NegativeInfinity, unit.data.maxHealth);
            }
            if (unit.data.health <= 0f)
            {
                unit.data.healthHandler.Die();
            }

            var colorHandler = unit.data.GetComponent<UnitColorHandler>();
            if (colorHandler && colorHandler.colors.All(x => x.colorName != damageColor.colorName || x.currentValue == 0f))
            {
                StartCoroutine(DoColor(colorHandler));
            }

            returnDamageEvent.Invoke();
        }
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
        foreach (var colorHandler in ColorHandlers.Where(colorHandler => colorHandler))
        {
            colorHandler.colors.RemoveAll(x => x.colorName == damageColor.colorName);
        }
    }
}