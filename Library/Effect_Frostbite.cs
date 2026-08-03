using System.Collections;
using TFBGames;
using UnityEngine;
using UnityEngine.Events;

namespace TGCore.Library;

public class Effect_Frostbite : UnitEffectBase
{
	private DataHandler Data;
	private MovementHandler Movement;
	private RotationHandler Rotation;
	private UnitColorHandler ColorHandler;
	private bool Done;
	private CurveAnimation[] AllRigs;
	private MeleeWeapon[] Weapons;
	private float[] DefaultForceMultipliers;
	private float[] DefaultWeaponForces;
	private float DefaultMovementMultiplier;
	private float DefaultTurnMultiplier;
	private float Amount;
	private float HealthMultiplier;
	
	private INetworkService NetworkService;

	private NetworkBattleController m_networkBattle;

	private NetworkBattleController NetworkBattle
	{
		get
		{
			if (m_networkBattle == null)
			{
				m_networkBattle = ServiceLocator.GetService<NetworkBattleController>();
			}
			return m_networkBattle;
		}
	}

	public float coldAmount = 40f;
	public float slowPerAmount;
	public float removeSlowSpeed = 0.1f;
	public float weaponMultiplier = 1f;

	public UnitColorInstance color;
	
	[Header("Freezing")]

	public Material iceMaterial;
	public UnityEvent freezeEvent;
	public float frozenMassMultiplier = 1f; 
	public float frozenDrag = 1f;
	public float frozenSinkTime = 5f;
	
	private void Start()
	{
		Data = transform.root.GetComponentInChildren<DataHandler>();
		Weapons = transform.root.GetComponentsInChildren<MeleeWeapon>();
		AllRigs = transform.root.GetComponentsInChildren<CurveAnimation>();
		Movement = Data.GetComponent<MovementHandler>();
		Rotation = Data.GetComponent<RotationHandler>();
		ColorHandler = Data.GetComponent<UnitColorHandler>();
	    
		DefaultForceMultipliers = new float[AllRigs.Length];
		for (var i = 0; i < AllRigs.Length; i++)
		{
			DefaultForceMultipliers[i] = AllRigs[i].multiplier;
		}
		DefaultWeaponForces = new float[Weapons.Length];
		for (var i = 0; i < Weapons.Length; i++)
		{
			DefaultWeaponForces[i] = Weapons[i].curveForce;
		}

		DefaultMovementMultiplier = Movement.multiplier;
		DefaultTurnMultiplier = Rotation.rotationMultiplier;
	    
		HealthMultiplier = 1f / Mathf.Clamp(Data.maxHealth * 0.01f, 1f, float.PositiveInfinity);
	}
    
	private void Update()
	{
		if (Done || (Data.Dead && !base.ShouldSkipDeadTests))
		{
			return;
		}
		var slowAboveDefault = false;
		for (var i = 0; i < AllRigs.Length; i++)
		{
			if (AllRigs[i].multiplier < DefaultForceMultipliers[i])
			{
				slowAboveDefault = true;
				AllRigs[i].multiplier += Time.deltaTime * removeSlowSpeed;
			}
		}
		for (var i = 0; i < Weapons.Length; i++)
		{
			if (Weapons[i].curveForce < DefaultWeaponForces[i])
			{
				slowAboveDefault = true;
				Weapons[i].curveForce += Time.deltaTime * removeSlowSpeed * weaponMultiplier;
			}
		}
		if (Movement.multiplier < DefaultMovementMultiplier)
		{
			slowAboveDefault = true;
			Movement.multiplier += Time.deltaTime * removeSlowSpeed;
		}
		if (Rotation.rotationMultiplier < DefaultTurnMultiplier)
		{
			slowAboveDefault = true;
			Rotation.rotationMultiplier += Time.deltaTime * removeSlowSpeed;
		}
	    
		if (slowAboveDefault)
		{
			Amount -= Time.deltaTime * removeSlowSpeed * slowPerAmount;
		}
		if (ColorHandler != null)
		{
			ColorHandler.SetColor(color, Mathf.Clamp(Amount * 0.03f, 0f, 1f));
		}
	}

	public override void DoEffect()
	{
		if (!Data) Start();
		Add(coldAmount);
	}
    
	public override void Ping()
	{
		Add(coldAmount);
	}
    
	private void Add(float a)
	{
		if ((Data.Dead && !base.ShouldSkipDeadTests) || Done)
		{
			return;
		}
        
		a *= HealthMultiplier;
		Amount += a;
            
		for (var i = 0; i < AllRigs.Length; i++)
		{
			AllRigs[i].multiplier = DefaultForceMultipliers[i] / (1f + Amount * slowPerAmount);
		}
		for (var i = 0; i < Weapons.Length; i++)
		{
			Weapons[i].curveForce = DefaultWeaponForces[i] / (1f + Amount * slowPerAmount * weaponMultiplier);
		}
		Movement.multiplier = DefaultMovementMultiplier / (1f + Amount * slowPerAmount);
		Rotation.rotationMultiplier = DefaultTurnMultiplier / (1f + Amount * slowPerAmount);
    		
		if (Amount * 0.03f >= 1f)
		{
			FreezeUnit();
		}
	}
    
	private void FreezeUnit()
	{
		if (!Done && (!Data || !Data.Dead || base.ShouldSkipDeadTests))
		{
			freezeEvent.Invoke();
			Done = true;
			StartCoroutine(DelayFreeze());
		}
	}
    
	private IEnumerator DelayFreeze()
	{
		yield return new WaitForSeconds(0.1f);
		if (Data != null && Data.healthHandler != null && Data.healthHandler.DiedLocally)
		{
			yield break;
		}
		if (NetworkBattle != null && NetworkService != null && NetworkService.IsClient)
		{
			if (ColorHandler != null)
			{
				ColorHandler.SetMaterial(iceMaterial);
			}
			ShouldSkipDeadTests = false;
			Data.healthHandler.DiedLocally = true;
			Destroy(gameObject);
		}
		else
		{
			ShouldSkipDeadTests = false;
			StandardDeath();
		}
	}
    
	private void StandardDeath()
	{
		var joints = transform.root.GetComponentsInChildren<ConfigurableJoint>();
		foreach (var joint in joints)
		{
			Destroy(joint);
		}
	    
		var newMass = 0f;
		var mainRigVelocity = Data.mainRig.velocity;
		var totalVelocity = Vector3.zero;
		foreach (var rig in transform.root.GetComponentsInChildren<Rigidbody>())
		{
			newMass += rig.mass;
			totalVelocity += rig.velocity / Data.allRigs.AllDrags.Length;
			Destroy(rig);
		}
		//foreach (var rig in Data.allRigs.AllRigs)
		//{
		//    if (rig != null)
		//    {
		//	    newMass += rig.mass;
		//	    zero += rig.velocity / Data.allRigs.AllDrags.Length;
		//	    Destroy(rig);
		//    }
		//}
		if (ColorHandler)
		{
			ColorHandler.SetMaterial(iceMaterial);
		}
	    
		var rootRig = Data.transform.root.gameObject.AddComponent<Rigidbody>();
		Data.mainRig = rootRig;
		rootRig.mass = newMass * frozenMassMultiplier;
		rootRig.velocity = mainRigVelocity;
		var sink = rootRig.gameObject.AddComponent<SinkOnDeath>();
		sink.time = frozenSinkTime;
		sink.Sink();
		rootRig.drag = frozenDrag;
		rootRig.angularDrag = frozenDrag;
		rootRig.interpolation = RigidbodyInterpolation.Interpolate;
	    
		var rootCollider = rootRig.GetComponent<Collider>();
		if (rootCollider)
		{
			Destroy(rootCollider);
		}
		rootRig.ResetCenterOfMass();
	    
		Data.healthHandler.Die();
		Destroy(gameObject);
	}
}