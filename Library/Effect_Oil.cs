using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class Effect_Oil : UnitEffectBase
{
    private Unit Unit;
    private UnitColorHandler ColorHandler;
    private float CurrentDamageAmp;
    private Balance Balance;
    private Rigidbody TorsoRig;
    private Rigidbody LeftFootRig;
    private Rigidbody RightFootRig;
    private float Counter;

    public UnitColorInstance oilColor;
    public float duration = 6f;
    
    [Header("Damage amp")]
    public float initialDamageAmpPercentage = 0.1f;
    public float damageAmpPerStack = 0.05f;
    public float maxDamageAmp = 1f;
    public UnityEvent didAmpEvent;
    
    [Header("Slipping")]
    public float slipChancePerSecond = 0.2f;
    public float footSlipForce;
    public float torsoSlipForce;

    private void Update()
    {
        if (Random.value < slipChancePerSecond * Time.deltaTime && Balance && Balance.Fall())
        {
            var rand = Random.insideUnitSphere;
            LeftFootRig.AddForce(rand * footSlipForce, ForceMode.VelocityChange);
            RightFootRig.AddForce(rand * footSlipForce, ForceMode.VelocityChange);
            TorsoRig.AddForce(rand * torsoSlipForce, ForceMode.VelocityChange);
        }
        
        Counter += Time.deltaTime;
        if (Counter > duration)
        {
            ColorHandler.colors.Remove(oilColor);
            Destroy(gameObject);
        }
    }
    
    public override void DoEffect()
    {
        Unit = transform.root.GetComponent<Unit>();
        ColorHandler = Unit.data.GetComponent<UnitColorHandler>();
        GetBalanceComponents();
        
        Unit.WasDealtDamageAction += TakeMoreDamage;

        CurrentDamageAmp = initialDamageAmpPercentage;
        ColorHandler.SetColor(oilColor, CurrentDamageAmp / maxDamageAmp);
    }

    public override void Ping()
    {
        if (CurrentDamageAmp < maxDamageAmp) Counter = 0f;
        
        CurrentDamageAmp = Mathf.Clamp(CurrentDamageAmp + damageAmpPerStack, initialDamageAmpPercentage, maxDamageAmp);
        ColorHandler.SetColor(oilColor, CurrentDamageAmp / maxDamageAmp);
    }

    public void TakeMoreDamage(float damageAmount)
    {
        if (damageAmount <= 0f) return;
        Unit.data.health -= damageAmount * CurrentDamageAmp;
        didAmpEvent.Invoke();
    }
    
    private void GetBalanceComponents()
    {
        if (Unit.data != null)
        {
            Balance = Unit.data.GetComponent<Balance>();
            if (Balance != null)
            {
                LeftFootRig = Unit.data.footLeft.GetComponent<Rigidbody>();
                RightFootRig = Unit.data.footRight.GetComponent<Rigidbody>();
                TorsoRig = Unit.data.torso.GetComponent<Rigidbody>();
            }
        }
    }

    private void OnDestroy()
    {
        if (ColorHandler)
        {
            ColorHandler.colors.Remove(oilColor);
        }
    }
}