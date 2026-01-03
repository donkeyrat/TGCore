using Landfall.TABS;
using Landfall.TABS.GameState;
using UnityEngine;

namespace TGCore.Library;

public class Effect_Shield : UnitEffectBase
{
    private Unit Unit;
    private UnitColorHandler ColorHandler;
    private GameStateManager m_gameStateManager;
    private float CurrentShield;
    
    public float shieldAmount = 50f;
    public float shieldDecaySpeed = 5f;
    public float maxShield = 500f;
    public float maxAbsorbedDamage = 1000f;

    public UnitColorInstance shieldColor;
    public AnimationCurve colorByHealthCurve;

    private void Update()
    {
        ModifyCurrentShield(-Time.deltaTime * shieldDecaySpeed);
        if (CurrentShield < maxShield)
        {
            ColorHandler.SetColor(shieldColor, colorByHealthCurve.Evaluate(CurrentShield / maxShield));
        }
    }
    
    public override void DoEffect()
    {
        Unit = transform.root.GetComponent<Unit>();
        ColorHandler = Unit.data.GetComponent<UnitColorHandler>();
        m_gameStateManager = ServiceLocator.GetService<GameStateManager>();
        Unit.WasDealtDamageAction += ProtectAgainstDamage;
        
        HealForAmount(shieldAmount);
    }

    public override void Ping()
    {
        HealForAmount(shieldAmount);
    }

    public void HealForAmount(float amount)
    {
        if (!Unit || (m_gameStateManager != null && m_gameStateManager.GameState == GameState.BattleState)) return;
        
        Unit.WasDamaged(-shieldAmount);

        ModifyCurrentShield(amount);
    }

    public void ProtectAgainstDamage(float damage)
    {
        if (CurrentShield <= 0) return;
        Unit.data.health += Mathf.Clamp(damage, 0f, maxAbsorbedDamage);
        ModifyCurrentShield(-damage);
    }

    public void ModifyCurrentShield(float amount)
    {
        CurrentShield += amount;
        CurrentShield = Mathf.Clamp(CurrentShield, 0f, maxShield);
    }
}