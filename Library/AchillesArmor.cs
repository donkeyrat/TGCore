using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library
{
    public class AchillesArmor : MonoBehaviour
    {
        private Unit Unit;
        private UnitIsArmored ArmoredUnit;
        private AchillesArmorEvent[] ArmorListeners;
        private bool HealthBarEnabled;
        private bool ArmorDisabled;
        private float ArmorDisabledCounter;
        private float MaxArmorHealth;
        private ShieldBar CurrentShieldBar;

        public GameObject healthBar;
        
        [Header("Armor Settings")]

        public UnityEvent armorDisableEvent = new UnityEvent();
        public UnityEvent armorEnableEvent = new UnityEvent();
        
        public float armorDisabledTime = 3f;
        
        [Header("Hit Settings")]
        
        public GameObject projectileHitEffect;
        public GameObject weaponHitEffect;
        
        public float parryForce;
        public float parryPower;
        
        public float blockPower;
        
        [Header("Health Settings")]
        
        public float armorHealth = 500f;

        public float maxDamageToBlock = 9999f;
        
        public bool healthRegenerate;
        public float healthRegenerationRate = 50f;
        
        public void Start()
        {
            Unit = transform.root.GetComponent<Unit>();
            Unit.WasDealtDamageAction += Armor;
            
            ArmoredUnit = Unit.gameObject.AddComponent<UnitIsArmored>();
            ArmoredUnit.projectileHitEffect = projectileHitEffect;
            ArmoredUnit.weaponHitEffect = weaponHitEffect;
            ArmoredUnit.parryForce = parryForce;
            ArmoredUnit.parryPower = parryPower;
            ArmoredUnit.blockPower = blockPower;
            ArmoredUnit.armorHealth = armorHealth;
            ArmoredUnit.startingArmorHealth = armorHealth;
            
            MaxArmorHealth = armorHealth;
            ArmorListeners = Unit.GetComponentsInChildren<AchillesArmorEvent>();
            
            HealthBarEnabled = ServiceLocator.GetService<GlobalSettingsHandler>()
                .GetSettingsInstance("GAMEPLAY_HEALTHBARS").currentValue == 1;
            var isShielded = Unit.GetComponent<ShieldBar.Shielded>();
            switch (HealthBarEnabled)
            {
                case true when !isShielded:
                    CurrentShieldBar = Instantiate(healthBar).GetComponent<ShieldBar>();
                    CurrentShieldBar.Init(Unit, null, ArmoredUnit);
                    break;
                case true when isShielded:
                    CurrentShieldBar = isShielded.bar;
                    CurrentShieldBar.SetArmor(ArmoredUnit);
                    break;
            }
        }

        public void Armor(float damage)
        {
            if (ArmorDisabled)
            {
                return;
            }

            if (damage > maxDamageToBlock)
            {
                armorHealth = 0f;
            }
            else
            {
                Unit.data.health += damage;
                armorHealth -= damage;
                armorHealth = Mathf.Clamp(armorHealth, 0f, MaxArmorHealth);
            }
            
            if (armorHealth <= 0f)
            {
                ArmorDisabled = true;
                armorDisableEvent.Invoke();
                
                foreach (var armor in ArmorListeners) armor.OnArmorDeactivated();
                ArmoredUnit.armorActive = false;
            }

            ArmoredUnit.armorHealth = armorHealth;
        }

        public void Update()
        {
            if (ArmorDisabled && (!Unit.data.Dead || Unit.data.healthHandler.willBeRewived))
            {
                ArmorDisabledCounter += Time.deltaTime;
                if (ArmorDisabledCounter >= armorDisabledTime)
                {
                    ArmorDisabledCounter = 0f;
                    ArmorDisabled = false;
                    armorHealth = MaxArmorHealth;
                    armorEnableEvent.Invoke();
                    
                    foreach (var armor in ArmorListeners) armor.OnArmorActivated();
                    ArmoredUnit.armorActive = true;
                    ArmoredUnit.armorHealth = MaxArmorHealth;
                    
                    var isShielded = Unit.GetComponent<ShieldBar.Shielded>();
                    switch (HealthBarEnabled)
                    {
                        case true when !isShielded:
                            CurrentShieldBar = Instantiate(healthBar).GetComponent<ShieldBar>();
                            CurrentShieldBar.Init(Unit, null, ArmoredUnit);
                            break;
                        case true when isShielded:
                            CurrentShieldBar = isShielded.bar;
                            CurrentShieldBar.SetArmor(ArmoredUnit);
                            break;
                    }
                }
            }
            else
            {
                if (healthRegenerate)
                {
                    Unit.data.health += Time.deltaTime * healthRegenerationRate;
                    Unit.data.health = Mathf.Clamp(Unit.data.health, 0f, Unit.data.maxHealth);
                }
            }
        }
        
        private void OnDestroy()
        {
            if (ArmoredUnit) Destroy(ArmoredUnit);
        }

        public class UnitIsArmored : MonoBehaviour
        {
            public bool armorActive = true;
            
            public GameObject projectileHitEffect;
            public GameObject weaponHitEffect;

            public float parryForce;
            public float parryPower;

            public float blockPower;

            public float armorHealth;
            public float startingArmorHealth;
        }
    }
}