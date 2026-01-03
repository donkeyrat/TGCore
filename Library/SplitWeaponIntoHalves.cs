using System.Collections;
using System.Collections.Generic;
using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class SplitWeaponIntoHalves : MonoBehaviour
{
    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
    }
    
    public void SplitWeapons()
    {
        var hold = OwnUnit.holdingHandler;
        
        if (!hold || !OwnUnit.WeaponHandler) return;
        
        OwnUnit.WeaponHandler.fistRefernce = null;

        if (hold.rightObject)
        {
            hold.LetGoOfWeapon(hold.rightObject.gameObject); 
            //Destroy(hold.rightObject);
            hold.rightObject = null;
        }
        else if (hold.leftObject)
        {
            hold.LetGoOfWeapon(hold.leftObject.gameObject);
            //Destroy(hold.leftObject);
            hold.leftObject = null;
        }
        
        ownWeapon.transform.SetParent(null);

        rightWeapon.isKinematic = false;
        rightWeapon.GetComponent<Holdable>().enabled = true;
        leftWeapon.isKinematic = false;
        leftWeapon.GetComponent<Holdable>().enabled = true;
        
        var equippedRight = OwnUnit.unitBlueprint.SetWeapon(OwnUnit, OwnUnit.Team, rightWeapon.gameObject, new PropItemData(), HoldingHandler.HandType.Right,
            OwnUnit.data.mainRig.rotation, new List<GameObject>());
        foreach (var joint in equippedRight.GetComponentsInChildren<ConfigurableJoint>())
        {
            Destroy(joint);
        }
        equippedRight.rigidbody.mass *= OwnUnit.unitBlueprint.massMultiplier;

        var equippedLeft = OwnUnit.unitBlueprint.SetWeapon(OwnUnit, OwnUnit.Team, leftWeapon.gameObject, new PropItemData(), HoldingHandler.HandType.Left,
            OwnUnit.data.mainRig.rotation, new List<GameObject>());
        foreach (var joint in equippedLeft.GetComponentsInChildren<ConfigurableJoint>())
        {
            Destroy(joint);
        }
        
        equippedLeft.rigidbody.mass *= OwnUnit.unitBlueprint.massMultiplier;

        if (lerp)
        {
            StartCoroutine(DoLerp(equippedRight.gameObject, equippedLeft.gameObject));
        }
        
        OwnUnit.m_AttackDistance = equippedRight.maxRange;
        OwnUnit.m_PreferedDistance = equippedRight.maxRange - 0.3f;
                
        Destroy(ownWeapon);
    }

    private IEnumerator DoLerp(GameObject right, GameObject left)
    {
        var counter = 0f;

        for (var i = 0; i < rightLerp.transform.childCount; i++)
        {
            rightLerp.transform.GetChild(i).gameObject.layer = layerDuringMovement;
        }
        for (var i = 0; i < leftLerp.transform.childCount; i++)
        {
            leftLerp.transform.GetChild(i).gameObject.layer = layerDuringMovement;
        }
        
        var rightStartPos = rightLerp.transform.position;
        var leftStartPos = leftLerp.transform.position;
        var rightStartRot = rightLerp.transform.rotation;
        var leftStartRot = leftLerp.transform.rotation;
        while (counter < 1f)
        {
            rightLerp.transform.position = Vector3.Lerp(rightStartPos, right.transform.position, lerpOverTime.Evaluate(counter));
            leftLerp.transform.position = Vector3.Lerp(leftStartPos, left.transform.position, lerpOverTime.Evaluate(counter));
            
            rightLerp.transform.rotation = Quaternion.Lerp(rightStartRot, right.transform.rotation, lerpOverTime.Evaluate(counter));
            leftLerp.transform.rotation = Quaternion.Lerp(leftStartRot, left.transform.rotation, lerpOverTime.Evaluate(counter));
            
            counter += Time.deltaTime;
            yield return null;
        }
        
        for (var i = 0; i < rightLerp.transform.childCount; i++)
        {
            rightLerp.transform.GetChild(i).gameObject.layer = layerOnEquip;
        }
        for (var i = 0; i < leftLerp.transform.childCount; i++)
        {
            leftLerp.transform.GetChild(i).gameObject.layer = layerOnEquip;
        }
        
        rightLerp.transform.SetParent(right.transform);
        leftLerp.transform.SetParent(left.transform);
        rightLerp.transform.localPosition = Vector3.zero;
        rightLerp.transform.localRotation = Quaternion.identity;
        leftLerp.transform.localPosition = Vector3.zero;
        leftLerp.transform.localRotation = Quaternion.identity;
    }

    private Unit OwnUnit;

    public GameObject ownWeapon;
    public Rigidbody rightWeapon;
    public Rigidbody leftWeapon;

    [Header("Lerp")] 
    
    public bool lerp;
    public AnimationCurve lerpOverTime;

    public GameObject rightLerp;
    public GameObject leftLerp;

    [Header("Layer")] 
    
    public int layerDuringMovement;
    public int layerOnEquip;

}