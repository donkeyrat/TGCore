using System.Collections;
using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class Effect_Bleed : UnitEffectBase
{
    private Unit Unit;
    private UnitColorHandler ColorHandler;
    private float Counter;
    private float CounterPerStack;
    private int Stacks;
    private float TotalBleedDamage;
    private float TotalDuration;
    private bool Done;

    public float bleedDamage = 10f;
    public float baseDuration = 5f;
    public float durationPerStack = 1f;
    public int maxStacks = 5;
    public UnitColorInstance bleedColor;
    public AnimationCurve colorPerStack;
    public UnityEvent stackEvent;
    public UnityEvent tickEvent;

    private void Update()
    {
        if (Unit.data.Dead && !Done) Done = true;
        if (Done) return;
        
        Counter += Time.deltaTime;
        
        CounterPerStack += Time.deltaTime;
        if (CounterPerStack >= 1f)
        {
            Unit.data.healthHandler.TakeDamage(TotalBleedDamage, Vector3.zero);
            StartCoroutine(DoColor());
            tickEvent.Invoke();

            CounterPerStack = 0f;
            if (Counter >= TotalDuration)
            {
                Done = true;
            }
        }
    }

    public override void DoEffect()
    {
        Unit = transform.root.GetComponent<Unit>();
        ColorHandler = Unit.data.GetComponent<UnitColorHandler>();
        TotalDuration = baseDuration;
        AddStack();
    }

    public override void Ping()
    {
        AddStack();
    }

    public void AddStack()
    {
        if (Stacks == maxStacks || Done) return;
        
        stackEvent.Invoke();
        Stacks++;
        TotalBleedDamage += bleedDamage;
        TotalDuration += durationPerStack;
    }
    
    private IEnumerator DoColor()
    {
        if (!ColorHandler) yield break;
        
        var counter = 0f;
        while (counter < colorPerStack.keys[colorPerStack.keys.Length - 1].time)
        {
            ColorHandler.SetColor(bleedColor, colorPerStack.Evaluate(counter));
            counter += Time.deltaTime;
            yield return null;
        }

        if (Done) Destroy(gameObject);
    }
}