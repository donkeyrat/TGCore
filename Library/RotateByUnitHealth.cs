using System;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class RotateByUnitHealth : MonoBehaviour
{
    private Unit OwnUnit;
    
    public enum Axis
    {
        X,
        Y,
        Z
    }
    
    public Axis axis;
    public float endRotation;

    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        OwnUnit.WasDealtDamageAction += Rotate;
    }

    public void Rotate(float damage)
    {
        var rotationAmount = Mathf.Lerp(endRotation, 0f, OwnUnit.data.health / OwnUnit.data.maxHealth);
        var rotation = transform.localEulerAngles;
        switch (axis)
        {
            case Axis.X:
                rotation.x = rotationAmount;
                break;
            case Axis.Y:
                rotation.y = rotationAmount;
                break;
            case Axis.Z:
                rotation.z = rotationAmount;
                break;
        }

        transform.localEulerAngles = rotation;
    }
}