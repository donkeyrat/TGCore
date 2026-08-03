using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class MultiplyRigByUnit : MonoBehaviour
{
    public void Start()
    {
        var unit = transform.root.GetComponent<Unit>();
        GetComponent<Rigidbody>().mass *= unit.unitBlueprint.massMultiplier * unit.transform.localScale.x * unit.transform.localScale.x;
    }
}