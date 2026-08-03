using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class NoFallDamage : MonoBehaviour
{
    private Unit OwnUnit;

    public bool goOnStart;
    
    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        if (goOnStart)
        {
            SetCanTakeFallDamage(false);
        }
    }

    public void SetCanTakeFallDamage(bool boolean)
    {
        OwnUnit.data.takeFallDamage = boolean;
    }
}