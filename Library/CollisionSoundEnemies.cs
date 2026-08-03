using System;
using Landfall.TABS;
using TFBGames;
using UnityEngine;

namespace TGCore.Library;

public class CollisionSoundEnemies : CollisionWeaponEffect, IValidatable
{
    private bool HasPlayedSoundThisSwing;
    private MeleeWeapon MeleeWeapon;
    private Unit OwnUnit;
    private Rigidbody Rig;
    private SoundPlayer MSoundPlayer;
    
    [SerializeField]
    private string soundEffectRef = "";

    [SerializeField]
    private AudioPathData soundEffectPathData;
    
    public float impactMultiplier = 1f;
    public bool ignoreTeammates = true;

    public string SoundEffectRef
    {
        get => soundEffectRef;
        set
        {
            soundEffectRef = value;
            AudioPathData.ValidateAndAssignPathData(soundEffectRef, ref soundEffectPathData, base.gameObject);
        }
    }

    public bool Validate()
    {
        return AudioPathData.ValidateAndAssignPathData(soundEffectRef, ref soundEffectPathData, base.gameObject);
    }

    private void Awake()
    {
        MSoundPlayer = ServiceLocator.GetService<SoundPlayer>();
    }

    private void Start()
    {
        OwnUnit = transform.root.GetComponent<Unit>();
        Rig = GetComponent<Rigidbody>();
        MeleeWeapon = GetComponent<MeleeWeapon>();
        if (MeleeWeapon)
        {
            MeleeWeapon.swingAction = (Action)Delegate.Combine(MeleeWeapon.swingAction, new Action(Swing));
        }
    }

    public void Swing()
    {
        HasPlayedSoundThisSwing = false;
    }

    public override void DoEffect(Transform hitTransform, Collision collision)
    {
        var multiplier = Mathf.Clamp(collision.impulse.magnitude / (Rig.mass + 10f) * 0.3f * impactMultiplier, 0f, 2f) * 0.5f;
        
        var hitUnit = hitTransform.root.GetComponent<Unit>();
        if (hitUnit && multiplier >= 0.1f && collision.rigidbody && (!MeleeWeapon || !HasPlayedSoundThisSwing) && (!ignoreTeammates || hitUnit.Team != OwnUnit.Team))
        {
            MSoundPlayer.PlaySoundEffectNonAlloc(soundEffectPathData, multiplier, base.transform.position, SoundEffectVariations.GetMaterialType(collision.gameObject, collision.rigidbody));
        }
    }
}