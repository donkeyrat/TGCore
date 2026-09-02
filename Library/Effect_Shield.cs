using Landfall.TABS;
using Landfall.TABS.GameState;
using UnityEngine;

namespace TGCore.Library;

public class Effect_Shield : UnitEffectBase
{
    private Unit Unit;
    private UnitColorHandler ColorHandler;
    private GameStateManager m_gameStateManager;
    private bool HealthBarEnabled;
    private ShieldBar CurrentShieldBar;
    
    [HideInInspector]
    public float currentShield;

    public GameObject healthBar;
    
    public float shieldAmount = 50f;
    public float shieldDecaySpeed = 5f;
    public float maxShield = 500f;
    public float maxAbsorbedDamage = 1000f;

    public UnitColorInstance shieldColor;
    public AnimationCurve colorByHealthCurve;

    private void Update()
    {
        ModifyCurrentShield(-Time.deltaTime * shieldDecaySpeed);
        if (currentShield < maxShield)
        {
            ColorHandler.SetColor(shieldColor, colorByHealthCurve.Evaluate(currentShield / maxShield));
        }
    }
    
    public override void DoEffect()
    {
        Unit = transform.root.GetComponent<Unit>();
        ColorHandler = Unit.data.GetComponent<UnitColorHandler>();
        Unit.WasDealtDamageAction += ProtectAgainstDamage;
        
        HealForAmount(shieldAmount);
        
        HealthBarEnabled = ServiceLocator.GetService<GlobalSettingsHandler>()
            .GetSettingsInstance("GAMEPLAY_HEALTHBARS").currentValue == 1;
        var isShielded = Unit.GetComponent<ShieldBar.Shielded>();
        switch (HealthBarEnabled)
        {
            case true when !isShielded:
                CurrentShieldBar = Instantiate(healthBar).GetComponent<ShieldBar>();
                CurrentShieldBar.Init(Unit, this);
                break;
            case true when isShielded:
                CurrentShieldBar = isShielded.bar;
                CurrentShieldBar.SetShield(this);
                break;
        }
    }

    public override void Ping()
    {
        HealForAmount(shieldAmount);

        var isShielded = Unit.GetComponent<ShieldBar.Shielded>();
        switch (HealthBarEnabled)
        {
            case true when !isShielded:
                CurrentShieldBar = Instantiate(healthBar).GetComponent<ShieldBar>();
                CurrentShieldBar.Init(Unit, this);
                break;
            case true when isShielded:
                CurrentShieldBar = isShielded.bar;
                CurrentShieldBar.SetShield(this);
                break;
        }
    }

    public void HealForAmount(float amount)
    {
        if (!Unit || (m_gameStateManager != null && m_gameStateManager.GameState == GameState.BattleState)) return;
        
        //Unit.WasDamaged(-shieldAmount);

        ModifyCurrentShield(amount);
    }

    public void ProtectAgainstDamage(float damage)
    {
        if (currentShield <= 0) return;
        Unit.data.health += Mathf.Clamp(damage, 0f, maxAbsorbedDamage);
        ModifyCurrentShield(-damage);
    }

    public void ModifyCurrentShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0f, maxShield);
    }
}