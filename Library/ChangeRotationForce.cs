using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class ChangeRotationForce : MonoBehaviour
{
    public float rotationForce = 25f;
    public bool ignoreY;
    
    private void Start()
    {
        var unit = transform.root.GetComponent<Unit>();
        if (!unit) return;
        var rotationHandler = unit.data.GetComponent<RotationHandler>();
        if (rotationHandler)
        {
            rotationHandler.rotationForce = rotationForce;
            rotationHandler.ignoreY = ignoreY ? ignoreY : rotationHandler.ignoreY;
        }
    }
}