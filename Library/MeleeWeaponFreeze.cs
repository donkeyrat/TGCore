using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class MeleeWeaponFreeze : CollisionWeaponEffect
{
    public Effect_Frostbite effect;
    public float freezeAmount;
    
    public bool ignoreTeamMates;
    
    public override void DoEffect(Transform hitTransform, Collision collision)
    {
        if (ignoreTeamMates)
        {
            var rootUnit = base.transform.root.GetComponent<Unit>();
            var hitUnit = hitTransform.root.GetComponent<Unit>();
            if (rootUnit && hitUnit && rootUnit.Team == hitUnit.Team)
            {
                return;
            }
        }
        var type = effect.GetType();
        var unitEffectBase = (UnitEffectBase)hitTransform.root.GetComponentInChildren(type);
        if (unitEffectBase == null)
        {
            var spawnedEffect = Instantiate(effect.gameObject, hitTransform.root);
            spawnedEffect.transform.position = hitTransform.root.position;
            spawnedEffect.GetComponent<UnitEffectBase>().DoEffect();
        }
        else
        {
            unitEffectBase.GetComponent<Effect_Frostbite>().coldAmount = freezeAmount;
            unitEffectBase.Ping();
        }
    }
}