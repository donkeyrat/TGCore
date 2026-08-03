using Landfall.TABS;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class DestroyIfWarMachine : MonoBehaviour
{
    public UnityEvent succeedEvent;
    
    public void Check()
    {
        var unit = transform.root.GetComponent<Unit>();
        if (!unit) return;
        if (unit.unitType == Unit.UnitType.Warmachine)
        {
            Destroy(gameObject);
        }
        else 
        {
            succeedEvent.Invoke();
        }
    }
}